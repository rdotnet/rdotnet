using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet.R.Adapter.Unix
{
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    internal class UnixPlatformManager : IPlatformManager
    {
        private DynamicLibrarySafeHandle _handle;

        public void Initialize()
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            _handle = InternalLoadLibrary(GetRLibraryFileName());
        }

        /// <summary>
        /// Gets the last error. NOTE: according to http://tldp.org/HOWTO/Program-Library-HOWTO/dl-libraries.html, returns NULL if called more than once after dlopen.
        /// </summary>
        /// <returns>The last error.</returns>
        public string GetLastError()
        {
            return dlerror();
        }

        public bool FreeLibrary(SafeHandle hModule)
        {
            // according to the manual page on a Debian box
            // The function dlclose() returns 0 on success, and nonzero on error.
            var status = dlclose(hModule);
            return status == 0;
        }

        public IntPtr GetProcAddress(string lpProcName)
        {
            return dlsym(_handle, lpProcName);
        }

        internal static DynamicLibrarySafeHandle InternalLoadLibrary(string filename)
        {
            const int rtldLazy = 0x1;
            if (filename.StartsWith("/"))
            {
                return dlopen(filename, rtldLazy);
            }
            var searchPaths = (Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(Path.PathSeparator);
            var dll = searchPaths.Select(directory => Path.Combine(directory, filename)).FirstOrDefault(File.Exists);
            if (dll == null)
            {
                throw new DllNotFoundException("Could not find the file: " + filename +
                                               " on the search path.  Checked these directories:\n "
                                               + String.Join("\n", searchPaths));
            }
            return dlopen(dll, rtldLazy);
        }

        public PlatformID GetPlatform()
        {
            var platform = PlatformID.Unix;
            try
            {
                var kernelName = ExecCommand("uname", "-s");
                platform = (kernelName == "Darwin" ? PlatformID.MacOSX : platform);
            }
            catch (Win32Exception)
            { }
           
            return platform;

        }

        public string GetRLibraryFileName()
        {
            var p = GetPlatform();
            if (p == PlatformID.MacOSX) return "libR.dylib";

            return "libR.so";
        }

        public void DoPlatformSpecificPreStart(InstanceStartupParameter startupParameter, RInstance instance)
        {
            var setParams = instance.GetFunction<R_SetParams_Unix>("R_SetParams");
            setParams(ref startupParameter.RStartupParameter.Common);
        }

        public string GetDllVersion()
        {
            return "unknown";
        }

        public string FindRHome(string rPath = null)
        {
            var platform = GetPlatform();
            string rHome;
            switch (platform)
            {
                case PlatformID.MacOSX:
                    rHome = "/Library/Frameworks/R.framework/Resources";
                    break;
                case PlatformID.Unix:
                    // if rPath is e.g. /usr/local/lib/R/lib/ , 
                    rHome = Path.GetDirectoryName(rPath);
                    if (!rHome.EndsWith("R"))
                        // if rPath is e.g. /usr/lib/ (symlink)  then default 
                        rHome = "/usr/lib/R";
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
            return rHome;
        }

        public string ConstructRPath(string rHome)
        {
            var shlibFilename = GetRLibraryFileName();
            var platform = GetPlatform();
            switch (platform)
            {
                case PlatformID.MacOSX: // TODO: is there a way to detect installations on MacOS
                    return "/Library/Frameworks/R.framework/Libraries";

                case PlatformID.Unix:
                    var rexepath = ExecCommand("which", "R"); // /usr/bin/R,  or /usr/local/bin/R
                    if (string.IsNullOrEmpty(rexepath)) return "/usr/lib";
                    var bindir = Path.GetDirectoryName(rexepath); //   /usr/local/bin
                    // Trying to emulate the startupParameter of the R shell script
                    // /usr/local/lib/R/lib/libR.so
                    var libdir = Path.Combine(Path.GetDirectoryName(bindir), "lib", "R", "lib");
                    if (File.Exists(Path.Combine(libdir, shlibFilename)))
                        return libdir;
                    libdir = Path.Combine(Path.GetDirectoryName(bindir), "lib64", "R", "lib");
                    if (File.Exists(Path.Combine(libdir, shlibFilename)))
                        return libdir;
                    return "/usr/lib";

                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public void DoPlatformSpecificPostStart()
        {
            var symbol = GetProcAddress("R_SignalHandlers");
            Marshal.WriteInt32(symbol, 0);
        }

        public string GetProcessBitness()
        {
            return "i386";
        }

        public string GetLoadLibError()
        {
            const string sampleldLibPaths = "/usr/local/lib/R/lib:/usr/local/lib:/usr/lib/jvm/java-7-openjdk-amd64/jre/lib/amd64/server";
            var ldLibPathEnv = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
            string msg = Environment.NewLine + Environment.NewLine;
            if (string.IsNullOrEmpty(ldLibPathEnv))
                msg = msg + "The environment variable LD_LIBRARY_PATH is not set.";
            else
                msg = msg + string.Format("The environment variable LD_LIBRARY_PATH is set to {0}.", ldLibPathEnv);

            msg = msg + string.Format(" For some Unix-like operating systems you may need to set it BEFORE launching the application. For instance to {0}.", sampleldLibPaths);
            msg = msg + " You can get the value as set by the R console application for your system, with the statement Sys.getenv('LD_LIBRARY_PATH'). For instance from your shell prompt:";
            msg = msg + Environment.NewLine;
            msg = msg + "Rscript -e \"Sys.getenv('LD_LIBRARY_PATH')\"";
            msg = msg + Environment.NewLine;
            msg = msg + "export LD_LIBRARY_PATH=/usr/the/paths/you/just/got/from/Rscript";
            msg = msg + Environment.NewLine + Environment.NewLine;

            return msg;
        }

        public InstanceStartupParameter DoPlatformSpecificPreInitialization(InstanceStartupParameter parameter)
        {
            var environmentDependentMaxSize = Environment.Is64BitProcess ? UInt64.MaxValue : UInt32.MaxValue;
            parameter.MaxCellSize = parameter.MaxCellSize < environmentDependentMaxSize ? parameter.MaxCellSize : environmentDependentMaxSize;
            parameter.MinCellSize = parameter.MinCellSize < environmentDependentMaxSize ? parameter.MinCellSize : environmentDependentMaxSize;
            parameter.MaxMemorySize = parameter.MaxMemorySize < environmentDependentMaxSize ? parameter.MaxMemorySize : environmentDependentMaxSize;
            parameter.MinMemorySize = parameter.MinMemorySize < environmentDependentMaxSize ? parameter.MinMemorySize : environmentDependentMaxSize;

            return parameter;
        }

        public InstanceStartupParameter ConfigureCharacterDevice(InstanceStartupParameter parameter, ICharacterDevice device)
        {
/*            IntPtr suicidePointer = GetProcAddress("ptr_R_Suicide");
            IntPtr newSuicide = Marshal.GetFunctionPointerForDelegate((ptr_R_Suicide)device.Suicide);
            Marshal.WriteIntPtr(suicidePointer, newSuicide);
            IntPtr showMessagePointer = GetProcAddress("ptr_R_ShowMessage");
            IntPtr newShowMessage = Marshal.GetFunctionPointerForDelegate((ptr_R_ShowMessage)device.ShowMessage);
            Marshal.WriteIntPtr(showMessagePointer, newShowMessage);
            IntPtr readConsolePointer = GetProcAddress("ptr_R_ReadConsole");
            IntPtr newReadConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_ReadConsole)device.ReadConsole);
            Marshal.WriteIntPtr(readConsolePointer, newReadConsole);
            IntPtr writeConsolePointer = GetProcAddress("ptr_R_WriteConsole");
            IntPtr newWriteConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_WriteConsole)device.WriteConsole);
            Marshal.WriteIntPtr(writeConsolePointer, newWriteConsole);
            IntPtr writeConsoleExPointer = GetProcAddress("ptr_R_WriteConsoleEx");
            IntPtr newWriteConsoleEx = Marshal.GetFunctionPointerForDelegate((ptr_R_WriteConsoleEx)device.WriteConsoleEx);
            Marshal.WriteIntPtr(writeConsoleExPointer, newWriteConsoleEx);
            IntPtr resetConsolePointer = GetProcAddress("ptr_R_ResetConsole");
            IntPtr newResetConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_ResetConsole)device.ResetConsole);
            Marshal.WriteIntPtr(resetConsolePointer, newResetConsole);
            IntPtr flushConsolePointer = GetProcAddress("ptr_R_FlushConsole");
            IntPtr newFlushConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_FlushConsole)device.FlushConsole);
            Marshal.WriteIntPtr(flushConsolePointer, newFlushConsole);
            IntPtr clearerrConsolePointer = GetProcAddress("ptr_R_ClearerrConsole");
            IntPtr newClearerrConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_ClearerrConsole)device.ClearErrorConsole);
            Marshal.WriteIntPtr(clearerrConsolePointer, newClearerrConsole);
            IntPtr busyPointer = GetProcAddress("ptr_R_Busy");
            IntPtr newBusy = Marshal.GetFunctionPointerForDelegate((ptr_R_Busy)device.Busy);
            Marshal.WriteIntPtr(busyPointer, newBusy);
            IntPtr cleanUpPointer = GetProcAddress("ptr_R_CleanUp");
            IntPtr newCleanUp = Marshal.GetFunctionPointerForDelegate((ptr_R_CleanUp)device.CleanUp);
            Marshal.WriteIntPtr(cleanUpPointer, newCleanUp);
            IntPtr showFilesPointer = GetProcAddress("ptr_R_ShowFiles");
            IntPtr newShowFiles = Marshal.GetFunctionPointerForDelegate((ptr_R_ShowFiles)device.ShowFiles);
            Marshal.WriteIntPtr(showFilesPointer, newShowFiles);
            IntPtr chooseFilePointer = GetProcAddress("ptr_R_ChooseFile");
            IntPtr newChooseFile = Marshal.GetFunctionPointerForDelegate((ptr_R_ChooseFile)device.ChooseFile);
            Marshal.WriteIntPtr(chooseFilePointer, newChooseFile);
            IntPtr editFilePointer = GetProcAddress("ptr_R_EditFile");
            IntPtr newEditFile = Marshal.GetFunctionPointerForDelegate((ptr_R_EditFile)device.EditFile);
            Marshal.WriteIntPtr(editFilePointer, newEditFile);
            IntPtr loadHistoryPointer = GetProcAddress("ptr_R_loadhistory");
            IntPtr newLoadHistory = Marshal.GetFunctionPointerForDelegate((ptr_R_loadhistory)device.LoadHistory);
            Marshal.WriteIntPtr(loadHistoryPointer, newLoadHistory);
            IntPtr saveHistoryPointer = GetProcAddress("ptr_R_savehistory");
            IntPtr newSaveHistory = Marshal.GetFunctionPointerForDelegate((ptr_R_savehistory)device.SaveHistory);
            Marshal.WriteIntPtr(saveHistoryPointer, newSaveHistory);
            IntPtr addHistoryPointer = GetProcAddress("ptr_R_addhistory");
            IntPtr newAddHistory = Marshal.GetFunctionPointerForDelegate((ptr_R_addhistory)device.AddHistory);
            Marshal.WriteIntPtr(addHistoryPointer, newAddHistory);*/

            return parameter;
        }

        private static string ExecCommand(string processName, string arguments)
        {
            using (var uname = new Process())
            {
                uname.StartInfo.FileName = processName;
                uname.StartInfo.Arguments = arguments;
                uname.StartInfo.RedirectStandardOutput = true;
                uname.StartInfo.UseShellExecute = false;
                uname.StartInfo.CreateNoWindow = true;
                uname.Start();
                var kernelName = uname.StandardOutput.ReadLine();
                uname.WaitForExit(1000);
                return kernelName;
            }
        }

        public void Dispose()
        {
            if (_handle != null)
            {
                _handle.Dispose();
                _handle = null;
            }
        }

        [DllImport("libdl")]
        private static extern DynamicLibrarySafeHandle dlopen([MarshalAs(UnmanagedType.LPStr)] string filename, int flag);

        [DllImport("libdl")]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string dlerror();

        [DllImport("libdl", EntryPoint = "dlclose")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern int dlclose(SafeHandle hModule);

        [DllImport("libdl", EntryPoint = "dlsym")]
        private static extern IntPtr dlsym(SafeHandle hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);
    }
}
