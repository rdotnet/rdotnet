using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using RDotNet.R.Adapter;
using RDotNet.Server.CharacterDevices;

namespace RDotNet.Server.Services
{
    public class RManagementService : IRManagementService
    {
        private readonly RInstance _instance;
        private bool _started;

        public RManagementService()
            : this(new RInstanceManager())
        {}

        public RManagementService(IRInstanceManager instanceManager)
        {
            if (instanceManager == null) throw new ArgumentNullException("instanceManager");

            _instance = instanceManager.GetInstance();
        }

        public void Start(StartupParameter parameter)
        {
            var instanceParameter = CreateInstanceParameter(parameter);
            _instance.Start(instanceParameter, new CachingCharacterDevice());

            _started = true;
        }

        public void Terminate()
        {
            _instance.Terminate();
        }

        private DateTime _lastPoke = DateTime.Now;

        public TimeSpan LastPoke()
        {
            return DateTime.Now.Subtract(_lastPoke);
        }

        public bool Poke()
        {
            _lastPoke = DateTime.Now;
            return true;
        }

        public void ForceGarbageCollection()
        {
            _instance.GetFunction<R_gc>()();
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public bool IsAlive()
        {
            try
            {
                _instance.GetFunction<Rf_mkChar>()("Test");
                return true;
            }
            catch (AccessViolationException)
            {
                return false;
            }
        }

        public bool IsStarted()
        {
            return _started;
        }

        public string GetRUser()
        {
            var user = _instance.GetFunction<getValue>("getRUser")();
            return Marshal.PtrToStringAnsi(user);
        }

        private static InstanceStartupParameter CreateInstanceParameter(StartupParameter parameter)
        {
            var result = new InstanceStartupParameter
            {
                Quiet = parameter.Quiet,
                Slave = parameter.Slave,
                Interactive = parameter.Interactive,
                Verbose = parameter.Verbose,
                RestoreAction = StartupRestoreAction.NoRestore,
                SaveAction = StartupSaveAction.NoSave,
                LoadSiteFile = parameter.LoadSiteFile,
                LoadInitFile = parameter.LoadInitFile,
                DebugInitFile = parameter.DebugInitFile,
                MinMemorySize = 6291456,
                MinCellSize = 350000,
                MaxMemorySize = UInt32.MaxValue,
                MaxCellSize = UInt32.MaxValue,
                StackSize = 50000,
                NoRenviron = parameter.NoRenviron,
            };

            return result;
        }
    }
}
