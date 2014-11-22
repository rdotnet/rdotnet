using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using RDotNet.R.Adapter.Graphics;
using RDotNet.R.Adapter.Unix;
using RDotNet.R.Adapter.Windows;

namespace RDotNet.R.Adapter
{
    public class RInstance
    {
        private readonly IPlatformManager _platformManager;
        private readonly DelegateCache _delegateCache;
        private readonly ICharacterDevice _defaultCharacterDevice = new NullCharacterDevice();
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private InstanceStartupParameter _parameter;

        public RInstance()
        {
            //This should be the only platform check,
            //all other platform code should be in the IPlatformManager implementations.
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                _platformManager = new UnixPlatformManager();
            }
            else
            {
                _platformManager = new WindowsPlatformManager();
            }

            SetEnvironmentVariables();

            try
            {
                _platformManager.Initialize();
            }
            catch (FileLoadException)
            {
                var error = string.Format("This {0}-bit process failed to load the library '{1}'.{2}{3}",
                    _platformManager.GetProcessBitness(),
                    _platformManager.GetRLibraryFileName(),
                    string.Format(" Native error message is '{0}'.", _platformManager.GetLastError()),
                    _platformManager.GetLoadLibError());
                throw new FileLoadException(error);
            }

            _delegateCache = new DelegateCache(_platformManager);
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        public ICharacterDevice CharacterDevice { get; private set; }

        public List<IGraphicsDevice> GraphicsDevices
        {
            get { return _graphicsDeviceManager.Devices; }
        }


        public IntPtr GetPointer(string name)
        {
            var pointer = _platformManager.GetProcAddress(name);
            return pointer;
        }

        public T GetFunction<T>(string name) where T : class
        {
            return _delegateCache.GetFunction<T>(name);
        }

        public T GetFunction<T>() where T : class
        {
            return _delegateCache.GetFunction<T>(typeof (T).Name);
        }

        public void Start(InstanceStartupParameter parameter, ICharacterDevice characterDevice = null)
        {
            SetHomeDirectory(parameter);

            GetFunction<R_setStartTime>()();

            parameter = _platformManager.DoPlatformSpecificPreInitialization(parameter);
            var args = parameter.BuildArguments();
            var status = GetFunction<Rf_initialize_R>()(args.Count, args.ToArray());
            if (status != 0)
            {
                var message =
                    string.Format("Error while trying to initialize the unmanaged R component. Error Code: {0}",
                        status);
                throw new InvalidOperationException(message);
            }

            CharacterDevice = characterDevice ?? _defaultCharacterDevice;
            _platformManager.ConfigureCharacterDevice(parameter, CharacterDevice);

            _platformManager.DoPlatformSpecificPreStart(parameter, this);
            GetFunction<setup_Rmainloop>()();

            DisableStackLimitChecking();
            _platformManager.DoPlatformSpecificPostStart();

            SetMemoryLimit(parameter.MaxMemorySize);

            _parameter = parameter;
        }

        private void SetMemoryLimit(ulong maxMemorySize)
        {
            var rLimitInMb = maxMemorySize/1048576UL;
            var s = GetFunction<Rf_mkString>()(string.Format("memory.limit({0})\n", rLimitInMb));
            GetFunction<Rf_protect>()(s);

            ParseStatus status;
            var parsed = GetFunction<R_ParseVector>()(s, -1, out status, Marshal.ReadIntPtr(GetPointer("R_NilValue")));
            if (status != ParseStatus.Ok) throw new InvalidOperationException("Could not set R memory.limit");
            GetFunction<Rf_protect>()(parsed);

            var vector = GetFunction<Rf_coerceVector>()(parsed, SymbolicExpressionType.ExpressionVector);
            if (vector == IntPtr.Zero) throw new InvalidOperationException("Could not set R memory.limit");
            GetFunction<Rf_protect>()(vector);

            bool error;
            var expression = IntPtr.Add(vector, Marshal.SizeOf(typeof (VECTOR_SEXPREC)));
            var result = GetFunction<R_tryEval>()(Marshal.ReadIntPtr(expression), Marshal.ReadIntPtr(GetPointer("R_GlobalEnv")), out error);
            if (error || result == IntPtr.Zero) throw new InvalidOperationException("Could not set R memory.limit");
            GetFunction<Rf_unprotect>()(3);
        }

        public void AddGraphicsDevice(IGraphicsDevice graphicsDevice)
        {
            _graphicsDeviceManager.AddDevice(graphicsDevice);
        }

        public void Terminate()
        {
            GetFunction<R_RunExitFinalizers>()();
            GetFunction<R_CleanTempDir>()();
            GetFunction<Rf_CleanEd>();
        }

        private void SetEnvironmentVariables(string rHome = null)
        {
            if (string.IsNullOrEmpty(rHome))
            {
                rHome = Environment.GetEnvironmentVariable("R_HOME");
                if (string.IsNullOrEmpty(rHome))
                {
                    rHome = _platformManager.FindRHome(rHome);
                }

                if (string.IsNullOrEmpty(rHome))
                    throw new InvalidOperationException("Could not find R_HOME in current environment or registry.");
            }
            var rPath = _platformManager.ConstructRPath(rHome);

            if (!Directory.Exists(rHome))
                throw new DirectoryNotFoundException(string.Format("R_HOME directory does not exist: '{0}'", rHome));

            if (!Directory.Exists(rPath))
                throw new DirectoryNotFoundException(
                    string.Format("Directory for R binaries does not exist: '{0}'", rPath));

            Environment.SetEnvironmentVariable("R_HOME", rHome);
            Environment.SetEnvironmentVariable("PATH", rPath + ";" + Environment.GetEnvironmentVariable("PATH"));
        }

        private void SetHomeDirectory(InstanceStartupParameter parameter)
        {
            if (string.IsNullOrEmpty(parameter.RHome))
            {
                var rhome = Environment.GetEnvironmentVariable("R_HOME");
                parameter.RStartupParameter.rhome = Marshal.StringToHGlobalAnsi(ConvertSeparatorToUnixStylePath(rhome));
            }
            if (string.IsNullOrEmpty(parameter.Home))
            {
                var user = GetRUser();
                parameter.RStartupParameter.home = Marshal.StringToHGlobalAnsi(ConvertSeparatorToUnixStylePath(user));
            }
        }

        private static string ConvertSeparatorToUnixStylePath(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }

        private string GetRUser()
        {
            var user = GetFunction<getValue>("getRUser")();
            return Marshal.PtrToStringAnsi(user);
        }

        private void DisableStackLimitChecking()
        {
            var symbol = GetPointer("R_CStackLimit");
            Marshal.WriteInt32(symbol, -1);
        }
    }
}
