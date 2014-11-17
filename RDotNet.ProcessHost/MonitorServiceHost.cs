using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;

namespace RDotNet.ProcessHost
{
    public class MonitorServiceHost : IDisposable
    {
        private ServiceHost _host;
        private ProcessMonitor _monitor;
        private Timer _timer;

        public MonitorServiceHost(int id, Uri uri, Action killCallback)
        {            
            _monitor = new ProcessMonitor(id, ServiceState.Initializing, killCallback);
            _host = new ServiceHost(_monitor, new Uri(uri + "ProcessMonitor/" + id));
            var serviceBehavior = _host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            serviceBehavior.InstanceContextMode = InstanceContextMode.Single;
            serviceBehavior.IncludeExceptionDetailInFaults = true;

            _host.Description.Behaviors.Add(new ServiceMetadataBehavior());
            _host.AddDefaultEndpoints();
            _host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), "mex");
            _host.Open();

            _timer = new Timer(_ => { if (CheckTimeout()) killCallback();}, null, 100, 100);
        }

        private bool CheckTimeout()
        {
            const int timeout = 30;
            if (DateTime.Now.Subtract(_monitor.LastAliveCheck).TotalMinutes > timeout)
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            _monitor = null;

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            if (_host != null)
            {
                _host.Close();
                _host = null;
            }
        }

        public void SetState(ServiceState state)
        {
            _monitor.State = state;
        }
    }
}
