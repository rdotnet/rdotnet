using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using RDotNet.Server;

namespace RDotNet.Client
{
    public class REngineFactory : IDisposable
    {
        private readonly IProcessFactory _processFactory;
        private ILocalInstance _localInstance;
        private readonly IHostConfiguration _hostConfiguration;
        private readonly IAPIFactory _apiFactory;

        private readonly Random _hostIdGenerator = new Random((int)DateTime.Now.Ticks);
        private int _localInstanceId = -1;
        private readonly object _lock = new object();

        public REngineFactory()
            : this(new LocalInstance(), new ProcessFactory(), new HostConfiguration(), new RRemoteAPIFactory(new HostConfiguration()))
        { }

        public REngineFactory(ILocalInstance localInstance, IProcessFactory processFactory, IHostConfiguration hostConfiguration, IAPIFactory apiFactory)
        {
            if (localInstance == null) throw new ArgumentNullException("localInstance");
            if (processFactory == null) throw new ArgumentNullException("processFactory");
            if (apiFactory == null) throw new ArgumentNullException("apiFactory");

            _localInstance = localInstance;
            _processFactory = processFactory;
            _hostConfiguration = hostConfiguration;
            _apiFactory = apiFactory;
        }

        private class RProcessInfo
        {
            public int Id;
            public bool InitializeEngine;
        }

        public REngine CreateInstance(int id)
        {
            var processType = _hostConfiguration.GetProcessType();

            RProcessInfo info;
            switch (processType)
            {
                case "Remote":
                    throw new NotImplementedException();

                case "OutOfProcess":
                    info = CreateOutOfProcessInstance(id);
                    break;

                default:
                    info = CreateLocalInstance();
                    break;

            }

            var engine = ConnectClientToServer(info);
            return engine;
        }

        public void Stop(int id)
        {
            var status = CheckHostStatus(id);
            Stop(id, status);
        }

        private enum HostState
        {
            Invalid,
            Unresponsive,
            InvalidRState,
            Alive
        }

        private RProcessInfo CreateOutOfProcessInstance(int id)
        {
            lock (_lock)
            {
                var status = CheckHostStatus(id);
                if (status != HostState.Alive)
                {
                    Stop(id, status);

                    var hostPath = _hostConfiguration.GetHostPath();
                    var fullUri = _hostConfiguration.CreateUriFromConfiguration();
                    var envVariables = _hostConfiguration.GetEnvironmentVariables();
                    id = id == 0 ? _hostIdGenerator.Next() : id;
                    var args = string.Format("--uri={0} --id={1}", fullUri.AbsoluteUri, id);

                    var process = _processFactory.Create(hostPath, args, envVariables);
                    if (process == null || process.HasExited) throw new InvalidOperationException("R host could not startupParameter.");

                    var retries = 10;
                    while ((process.MainWindowTitle.Contains("Initializing") || !process.MainWindowTitle.Contains("Ready")) && retries-- != 0)
                    {
                        Thread.Sleep(100);
                    }

                    if (retries == 0 || process.HasExited) throw new InvalidOperationException("R host failed to initialize.");

                    return new RProcessInfo { Id = id, InitializeEngine = true };
                }

                return new RProcessInfo { Id = id, InitializeEngine = false };
            }
        }

        private RProcessInfo CreateLocalInstance()
        {
            lock (_lock)
            {
                if (_localInstanceId != -1) return new RProcessInfo { Id = _localInstanceId, InitializeEngine = false };

                var fullUri = _hostConfiguration.CreateUriFromConfiguration();
                _localInstanceId = _hostIdGenerator.Next();
                _localInstance.Create(fullUri, _localInstanceId);

                return new RProcessInfo { Id = _localInstanceId, InitializeEngine = true };
            }
        }


        private void Stop(int id, HostState state)
        {
            try
            {
                if (id == 0) return;

                switch (state)
                {
                    case HostState.Alive:
                    case HostState.InvalidRState:
/*
 * FIXME: [RMEL] Need to wait for the process to terminate. Seems to take a long time though. 
                        var monitor = _apiFactory.CreateProcessMonitorAPI(id);
                        monitor.Kill();
                        break;
*/

                    case HostState.Unresponsive:
                        TryKillLocalProcess(id);
                        break;

                }
            }
            catch (TimeoutException)
            { }
            catch (CommunicationException)
            { }
        }

        private static void TryKillLocalProcess(int id)
        {
            var hosts = Process.GetProcessesByName("RDotNet.ProcessHost");
            if (hosts.Length > 0)
            {
                var filteredById = hosts.Where(s => s.MainWindowTitle.Contains(id.ToString(CultureInfo.InvariantCulture))).ToList();
                if (filteredById.Count == 0) return;
                filteredById.ForEach(f => f.Kill());
            }
        }

        private HostState CheckHostStatus(int id)
        {
            if (id == 0) return HostState.Invalid;

            try
            {
                var management = _apiFactory.CreateRManagementAPI(id);
                var status = management.IsAlive();

                return status ? HostState.Alive : HostState.InvalidRState;
            }
            catch (TimeoutException)
            {
                return HostState.Unresponsive;
            }
            catch (CommunicationException)
            {
                return HostState.Unresponsive;
            }
        }

        private REngine ConnectClientToServer(RProcessInfo info)
        {
            if (info.Id == 0) throw new InvalidOperationException("Id cannot be the empty id of 0.");

            var m = _apiFactory.CreateRManagementAPI(info.Id);
            var engine = new REngine(info.Id,
                m,
                _apiFactory.CreateRObjectAPI(info.Id),
                _apiFactory.CreateROutputAPI(info.Id));

            if (info.InitializeEngine)
            {
                m.Start(new StartupParameter { LoadInitFile = true, LoadSiteFile = true });
            }

            return engine;
        }

        public void Dispose()
        {
            if (_localInstance != null)
            {
                _localInstance.Dispose();
                _localInstance = null;
            }
        }
    }
}
