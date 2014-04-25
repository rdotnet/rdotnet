using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace RDotNet.NativeLibrary
{
   /// <summary>
   /// Collection of utility methods for operating systems.
   /// </summary>
   public static class NativeUtility
   {
      /// <summary>
      /// Gets the platform on which the current process runs.
      /// </summary>
      /// <remarks>
      /// <see cref="Environment.OSVersion"/>'s platform is not <see cref="PlatformID.MacOSX"/> even on Mac OS X.
      /// This method returns <see cref="PlatformID.MacOSX"/> when the current process runs on Mac OS X.
      /// This method uses UNIX's uname command to check the operating system,
      /// so this method cannot check the OS correctly if the PATH environment variable is changed (will returns <see cref="PlatformID.Unix"/>).
      /// </remarks>
      /// <returns>The current platform.</returns>
      public static PlatformID GetPlatform()
      {
         if (!curPlatform.HasValue) {
            var platform = Environment.OSVersion.Platform;
            if (platform != PlatformID.Unix) {
               curPlatform = platform;
            } else {
               try {
                  var kernelName = ExecCommand("uname", "-s");
                  curPlatform = (kernelName == "Darwin" ? PlatformID.MacOSX : platform);
               } catch (Win32Exception) { // probably no PATH to uname.
                  curPlatform = platform;
               }
            }
         }
         return curPlatform.Value;
      }

      private static PlatformID? curPlatform = null;

      /// <summary>
      /// Execute a command in a new process
      /// </summary>
      /// <param name="processName">Process name e.g. "uname"</param>
      /// <param name="arguments">Arguments e.g. "-s"</param>
      /// <returns>The output of the command to the standard output stream</returns>
      public static string ExecCommand(string processName, string arguments)
      {
         using (var uname = new Process()) {
            uname.StartInfo.FileName = processName;
            uname.StartInfo.Arguments = arguments;
            uname.StartInfo.RedirectStandardOutput = true;
            uname.StartInfo.UseShellExecute = false;
            uname.StartInfo.CreateNoWindow = true;
            uname.Start();
            var kernelName = uname.StandardOutput.ReadLine();
            uname.WaitForExit();
            return kernelName;
         }
      }

      /// <summary>
      /// Sets the PATH and R_HOME environment variables if needed.
      /// </summary>
      /// <param name="rPath">The path of the directory containing the R native library. 
      /// If null (default), this function tries to locate the path via the Windows registry, or commonly used locations on MacOS and Linux</param>
      /// <param name="rHome">The path for R_HOME. If null (default), the function checks the R_HOME environment variable. If none is set, 
      /// the function uses platform specific sensible default behaviors.</param>
      /// <remarks>
      /// This function has been designed to limit the tedium for users, while allowing custom settings for unusual installations.
      /// </remarks>
      public static void SetEnvironmentVariables(string rPath = null, string rHome = null)
      {
         var platform = GetPlatform();
         if (rPath != null)
            CheckDirExists(rPath);
         if (rHome != null)
            CheckDirExists(rHome);

         if (rPath == null)
            rPath = FindRPath();
         SetenvPrependToPath(rPath);

         if (string.IsNullOrEmpty(rHome))
            rHome = GetRHomeEnvironmentVariable();
         if (string.IsNullOrEmpty(rHome)) {
            // R_HOME is neither specified by the user nor as an environmental variable. Rely on default locations specific to platforms
            rHome = FindRHome(rPath);
         }
         if (string.IsNullOrEmpty(rHome))
            throw new NotSupportedException("R_HOME was not provided and could not be found by R.NET");
         else
         {
            // It is highly recommended to use the 8.3 short path format on windows. 
            // See the manual page of R.home function in R. Solves at least the issue R.NET 97.
            if (platform == PlatformID.Win32NT)
               rHome = WindowsLibraryLoader.GetShortPath(rHome);
            if (!Directory.Exists(rHome))
               throw new DirectoryNotFoundException("Directory for R_HOME does not exist");
            Environment.SetEnvironmentVariable("R_HOME", rHome);
         }
         if (platform == PlatformID.Unix) {
            // Let's check that LD_LIBRARY_PATH is set if this is a custom installation of R.
            // Normally in an R session from a custom build/install we get something typically like:
            // > Sys.getenv('LD_LIBRARY_PATH')
            // [1] "/usr/local/lib/R/lib:/usr/local/lib:/usr/lib/jvm/java-7-openjdk-amd64/jre/lib/amd64/server"
            // The R script sets LD_LIBRARY_PATH before it starts the native executable under e.g. /usr/local/lib/R/bin/exec/R
            // This would be useless to set LD_LIBRARY_PATH in the current function:
            // it must be set as en env var BEFORE the process is started (see man page for dlopen)
            // so all we can do is an intelligible error message for the user, explaining he needs to set the LD_LIBRARY_PATH env variable 
            // Let's delay the notification about a missing LD_LIBRARY_PATH till loading libR.so fails, if it does.
         }
      }

      /// <summary>
      /// Gets the value, if any, of the R_HOME environment variable of the current process
      /// </summary>
      /// <returns>The value, or null if not set</returns>
      public static string GetRHomeEnvironmentVariable()
      {
         return Environment.GetEnvironmentVariable("R_HOME");
      }

      /// <summary>
      /// Try to locate the directory path to use for the R_HOME environment variable. This is used by R.NET by default; users may want to use it to diagnose problematic behaviors.
      /// </summary>
      /// <param name="rPath">Optional path to the directory containing the R shared library. This is ignored unless on a Unix platform (i.e. ignored on Windows and MacOS)</param>
      /// <returns>The path that R.NET found suitable as a candidate for the R_HOME environment</returns>
      public static string FindRHome(string rPath=null)
      {
         var platform = GetPlatform();
         string rHome;
         switch (platform)
         {
            case PlatformID.Win32NT:
               // We need here to guess, process and set R_HOME
               // Rf_initialize_R for gnuwin calls get_R_HOME which scans the windows registry and figures out R_HOME; however for 
               // unknown reasons in R.NET we end up with long path names, whereas R.exe ends up with the short, 8.3 path format.
               // Blanks in the R_HOME environment variable cause trouble (e.g. for Rcpp), so we really must make sure 
               // that rHome is a short path format. Here we retrieve the path possibly in long format, and process to short format later on 
               // to capture all possible sources of R_HOME specifications
               // Behavior added to fix issue 
               rHome = GetRhomeWin32NT();
               break;
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
               throw new NotSupportedException(platform.ToString());
         }
         return rHome;
      }

      private static string GetRhomeWin32NT()
      {
         RegistryKey rCoreKey = GetRCoreRegistryKeyWin32();
         return GetRInstallPathFromRCoreKegKey(rCoreKey);
      }

      private static void CheckDirExists(string rPath)
      {
         if (!Directory.Exists(rPath))
            throw new ArgumentException(string.Format("Specified directory not found: '{0}'", rPath));
      }

      /// <summary>
      /// Attempt to find a suitable path to the R shared library. This is used by R.NET by default; users may want to use it to diagnose problematic behaviors.
      /// </summary>
      /// <returns>The path to the directory where the R shared library is expected to be</returns>
      public static string FindRPath()
      {
         var shlibFilename = GetRDllFileName();
         var platform = GetPlatform();
         switch (platform) {
         case PlatformID.Win32NT:
            return FindRPathFromRegistry();
         case PlatformID.MacOSX: // TODO: is there a way to detect installations on MacOS
            return "/Library/Frameworks/R.framework/Libraries";
         case PlatformID.Unix:  
            var rexepath = ExecCommand("which", "R"); // /usr/bin/R,  or /usr/local/bin/R
            if (!string.IsNullOrEmpty(rexepath)) {
               var bindir = Path.GetDirectoryName(rexepath);  //   /usr/local/bin
               // Trying to emulate the start of the R shell script
               // /usr/local/lib/R/lib/libR.so
               var libdir = Path.Combine(Path.GetDirectoryName(bindir), "lib", "R", "lib");
               if (File.Exists(Path.Combine(libdir, shlibFilename)))
                  return libdir;
               libdir = Path.Combine(Path.GetDirectoryName(bindir), "lib64", "R", "lib");
               if (File.Exists(Path.Combine(libdir, shlibFilename)))
                  return libdir;
            }
            return "/usr/lib"; 
         default:
            throw new NotSupportedException(platform.ToString());
         }
      }

      private static void SetenvPrependToPath(string rPath, string envVarName="PATH")
      {
         Environment.SetEnvironmentVariable(envVarName, PrependToPath(rPath, envVarName));
      }

      private static string PrependToPath(string rPath, string envVarName = "PATH")
      {
         var currentPathEnv = Environment.GetEnvironmentVariable(envVarName);
         var paths = currentPathEnv.Split(new[]{Path.PathSeparator}, StringSplitOptions.RemoveEmptyEntries);
         if (paths[0] == rPath)
            return currentPathEnv;
         return rPath + Path.PathSeparator + currentPathEnv;
      }

      /// <summary>
      /// Windows-only function; finds in the Windows registry the path to the most recently installed R binaries.
      /// </summary>
      /// <returns>The path, such as</returns>
      public static string FindRPathFromRegistry()
      {
         CheckPlatformWin32();
         bool is64Bit = Environment.Is64BitProcess;
         RegistryKey rCoreKey = GetRCoreRegistryKeyWin32();
         var installPath = GetRInstallPathFromRCoreKegKey(rCoreKey);
         var currentVersion = new Version((string)rCoreKey.GetValue("Current Version"));
         var bin = Path.Combine(installPath, "bin");
         // Up to 2.11.x, DLLs are installed in R_HOME\bin.
         // From 2.12.0, DLLs are installed in the one level deeper directory.
         return currentVersion < new Version(2, 12) ? bin : Path.Combine(bin, is64Bit ? "x64" : "i386");
      }

      private static string GetRInstallPathFromRCoreKegKey(RegistryKey rCoreKey)
      {
         var installPath = (string)rCoreKey.GetValue("InstallPath");
         return installPath;
      }

      private static void CheckPlatformWin32()
      {
         if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            throw new NotSupportedException("This method is supported only on the Win32NT platform");
      }

      private static RegistryKey GetRCoreRegistryKeyWin32()
      {
         CheckPlatformWin32();
         var rCore = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\R-core");
         if (rCore == null)
         {
            rCore = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\R-core");
            if (rCore == null)
               throw new ApplicationException("Windows Registry key 'SOFTWARE\\R-core' not found in HKEY_LOCAL_MACHINE nor HKEY_CURRENT_USER");
         }
         bool is64Bit = Environment.Is64BitProcess;
         var subKey = is64Bit ? "R64" : "R";
         var r = rCore.OpenSubKey(subKey);
         if (r == null)
         {
            throw new ApplicationException( string.Format(
               "Windows Registry sub-key '{0}' of key '{1}' was not found", subKey , rCore.ToString()));
         }
         return r;
      }

      /// <summary>
      /// Gets the default file name of the R library on the supported platforms.
      /// </summary>
      /// <returns>R dll file name</returns>
      public static string GetRDllFileName()
      {
         var p = GetPlatform();
         switch (p) {
         case PlatformID.Win32NT:
            return "R.dll";

         case PlatformID.MacOSX:
            return "libR.dylib";

         case PlatformID.Unix:
            return "libR.so";

         default:
            throw new NotSupportedException("Platform is not supported: " + p.ToString());
         }
      }

      /// <summary>
      /// Is the platform a unix like (Unix or MacOX)
      /// </summary>
      public static bool IsUnix {
         get {
            var p = GetPlatform();
            return p == PlatformID.MacOSX || p == PlatformID.Unix;
         }
      }
   }
}