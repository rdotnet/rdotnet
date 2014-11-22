using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;

namespace RDotNet.R.Adapter.Windows
{
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    internal class WindowsPlatformManager : IPlatformManager
    {
        private DynamicLibrarySafeHandle _handle;

        public void Initialize()
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            var handle = Win32.LoadLibrary(GetRLibraryFileName());
            if (handle == IntPtr.Zero)
            {
                var error = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                Console.WriteLine(error);
            }

            _handle = new DynamicLibrarySafeHandle(handle, this);
        }

        public bool FreeLibrary(SafeHandle handle)
        {
            return Win32.FreeLibrary(handle);
        }

        public PlatformID GetPlatform()
        {
            return Environment.OSVersion.Platform;
        }

        public string GetRLibraryFileName()
        {
            return "R.dll";
        }

        public string GetLastError()
        {
            // see for instance http://blogs.msdn.com/b/shawnfa/archive/2004/09/10/227995.aspx 
            // and http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx
            // TODO: does this work as expected with Mono+Windows stack?
            return new Win32Exception().Message;
        }

        public IntPtr GetProcAddress(string lpProcName)
        {
            return Win32.GetProcAddress(_handle, lpProcName);
        }

        public void DoPlatformSpecificPreStart(InstanceStartupParameter startupParameter, RInstance instance)
        {
            var setParams = instance.GetFunction<R_SetParams_Windows>("R_SetParams");
            setParams(ref startupParameter.RStartupParameter);
        }

        public void DoPlatformSpecificPostStart()
        { }

        public string GetProcessBitness()
        {
            return Environment.Is64BitProcess ? "x64" : "i386";
        }

        public string GetLoadLibError()
        {
            return string.Empty;
        }

        public InstanceStartupParameter ConfigureCharacterDevice(InstanceStartupParameter parameter, ICharacterDevice device)
        {
            parameter.RStartupParameter.ReadConsole = device.ReadConsole;
            parameter.RStartupParameter.WriteConsole = device.WriteConsole;
            parameter.RStartupParameter.WriteConsoleEx = device.WriteConsoleEx;
            parameter.RStartupParameter.CallBack = device.Callback;
            parameter.RStartupParameter.ShowMessage = device.ShowMessage;
            parameter.RStartupParameter.YesNoCancel = device.Ask;
            parameter.RStartupParameter.Busy = device.Busy;

            return parameter;
        }

        public InstanceStartupParameter DoPlatformSpecificPreInitialization(InstanceStartupParameter parameter)
        {
            //TODO: [RMEL] Show these as warnings.
            var environmentDependentMaxSize = Environment.Is64BitProcess ? UInt64.MaxValue : UInt32.MaxValue;
            parameter.MaxCellSize = parameter.MaxCellSize < environmentDependentMaxSize ? parameter.MaxCellSize : environmentDependentMaxSize;
            parameter.MinCellSize = parameter.MinCellSize < environmentDependentMaxSize ? parameter.MinCellSize : environmentDependentMaxSize;
            parameter.MaxMemorySize = parameter.MaxMemorySize < environmentDependentMaxSize ? parameter.MaxMemorySize : environmentDependentMaxSize;
            parameter.MinMemorySize = parameter.MinMemorySize < environmentDependentMaxSize ? parameter.MinMemorySize : environmentDependentMaxSize;

            parameter.Interactive = false;
            parameter.CharacterMode = UiMode.LinkDll;

            return parameter;
        }

        public string GetDllVersion()
        {
            var version = GetProcAddress("getDLLVersion");
            return Marshal.PtrToStringAnsi(version);
        }

        public string FindRHome(string unused = null)
        {
            var platform = GetPlatform();
            if (platform != PlatformID.Win32NT) throw new PlatformNotSupportedException();

            var rHome = GetRHomeFromRegistry();
            return rHome;
        }

        public string ConstructRPath(string rHome)
        {
            var platform = GetPlatform();
            if (platform != PlatformID.Win32NT) throw new PlatformNotSupportedException();

            var rPath = Path.Combine(rHome, "bin");
            var rVersion = GetRVersionFromRHomeFolder(rHome);
            if (rVersion == null)
            {
                rVersion = GetRVersionFromRegistry();
                if (rVersion == null) throw new InvalidOperationException("Could not find R Version in R_HOME or registry.");
            }

            if (rVersion.Major > 2 || (rVersion.Major == 2 && rVersion.Minor >= 12))
            {
                rPath = Path.Combine(rPath, GetProcessBitness());
            }
            return rPath;
        }

        private static Version GetRVersionFromRegistry()
        {
            try
            {
                var rCoreKey = GetRCoreRegistryKey();
                var version = rCoreKey.GetValue("Current Version") as string;
                if (string.IsNullOrEmpty(version)) return null;
                return new Version(version);
            }
            catch (UnauthorizedAccessException)
            {
                throw new InvalidOperationException("Could not read R version from registry.");
            }
            catch (SecurityException)
            {
                throw new InvalidOperationException("Could not read R version from registry.");
            }
        }

        private static Version GetRVersionFromRHomeFolder(string rHome)
        {
            var versionFile = Path.Combine(rHome, "VERSION");
            if (!File.Exists(versionFile)) return null;

            try
            {
                using (var file = new StreamReader(versionFile))
                {
                    var version = file.ReadToEnd();
                    return new Version(version);
                }
            }
            catch (FormatException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }

        private static string GetRHomeFromRegistry()
        {
            var rCoreKey = GetRCoreRegistryKey();
            var path = rCoreKey.GetValue("InstallPath") as string;
            return GetShortPath(path);
        }

        private static string GetShortPath(string path)
        {
            var shortPath = new StringBuilder(Win32.MaxPathLength);
            Win32.GetShortPathName(path, shortPath, Win32.MaxPathLength);
            return shortPath.ToString();
        }

        private static RegistryKey GetRCoreRegistryKey()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT) return null;

            var rCore = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\R-core");
            if (rCore == null)
            {
                rCore = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\R-core");
                if (rCore == null) return null;
            }

            var subKey = Environment.Is64BitProcess ? "R64" : "R";
            var r = rCore.OpenSubKey(subKey);
            return r;
        }

        public void Dispose()
        {
            if (_handle != null)
            {
                _handle.Dispose();
                _handle = null;
            }
        }
    }

    public static class Win32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(SafeHandle hModule);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(SafeHandle hModule,
            [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        public const int MaxPathLength = 248; //MaxPath is 248. MaxFileName is 260.

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName(
            [MarshalAs(UnmanagedType.LPTStr)] string path,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder shortPath,
            int shortPathLength);
    }
}
