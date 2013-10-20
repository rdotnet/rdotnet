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
         var platform = Environment.OSVersion.Platform;
         if (platform != PlatformID.Unix)
         {
            return platform;
         }
         try
         {
               var kernelName = ExecCommand("uname", "-s");
               return kernelName == "Darwin" ? PlatformID.MacOSX : platform;
         }
         catch (Win32Exception) // probably no PATH to uname.
         {
            return platform;
         }
      }

      /// <summary>
      /// Execute a command in a new process
      /// </summary>
      /// <param name="processName">Process name e.g. "uname"</param>
      /// <param name="arguments">Arguments e.g. "-s"</param>
      /// <returns>The output of the command to the standard output stream</returns>
      public static string ExecCommand(string processName, string arguments)
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
         if (rPath != null) CheckDirExists(rPath);
         if (rHome != null) CheckDirExists(rHome);

         if (rPath == null)
            rPath = FindRPath();
         SetenvPrependToPath(rPath);

         if (string.IsNullOrEmpty(rHome))
            rHome = Environment.GetEnvironmentVariable("R_HOME");
         if (string.IsNullOrEmpty(rHome))
         {
            // R_HOME is neither specified by the user nor as an environmental variable. Rely on default locations specific to platforms
            var platform = Environment.OSVersion.Platform;
            switch (platform)
            {
               case PlatformID.Win32NT:
                  break; // R on Windows seems to have a way to deduce its R_HOME if its R.dll is in the PATH
               case PlatformID.MacOSX:
                  rHome = "/Library/Frameworks/R.framework/Resources";
                  break;
               case PlatformID.Unix:
                  rHome = "/usr/lib/R";
                  break;
               default:
                  throw new NotSupportedException(platform.ToString());
            }
         }
         if (!string.IsNullOrEmpty(rHome))
            Environment.SetEnvironmentVariable("R_HOME", rHome);
      }

      private static void CheckDirExists(string rPath)
      {
         if (!Directory.Exists(rPath))
            throw new ArgumentException(string.Format("Specified directory not found: '{0}'", rPath));
      }

      public static string FindRPath()
      {
         switch (Environment.OSVersion.Platform)
         {
            case PlatformID.Win32NT:
               return FindRPathFromRegistry();
            case PlatformID.MacOSX: // TODO: is there a way to detect installations on MacOS
               return "/Library/Frameworks/R.framework/Libraries";
            case PlatformID.Unix:  // TODO: perform a 'which R' command to locate local installations
               return "/usr/lib";
            default:
               throw new NotSupportedException(Environment.OSVersion.Platform.ToString());
         }
      }

      private static void SetenvPrependToPath(string rPath)
      {
         Environment.SetEnvironmentVariable("PATH", PrependToPath(rPath));
      }

      private static string PrependToPath(string rPath)
      {
         return rPath + Path.PathSeparator + Environment.GetEnvironmentVariable("PATH");
      }

      public static string FindRPathFromRegistry()
      {
         if(Environment.OSVersion.Platform != PlatformID.Win32NT)
            throw new NotSupportedException("This method is supported only on the Windows platform");
         var rCore = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\R-core");
         if (rCore == null)
         {
            throw new ApplicationException("Windows Registry key 'SOFTWARE\\R-core' not found.");
         }
         var is64Bit = Environment.Is64BitProcess;
         var subKey = is64Bit ? "R64" : "R";
         var r = rCore.OpenSubKey(subKey);
         if (r == null)
         {
            throw new ApplicationException("Windows Registry sub-key '" + subKey + "' not found.");
         }
         var currentVersion = new Version((string)r.GetValue("Current Version"));
         var installPath = (string)r.GetValue("InstallPath");
         var bin = Path.Combine(installPath, "bin");
         // Up to 2.11.x, DLLs are installed in R_HOME\bin.
         // From 2.12.0, DLLs are installed in the one level deeper directory.
         return currentVersion < new Version(2, 12) ? bin : Path.Combine(bin, is64Bit ? "x64" : "i386");
      }

      /// <summary>
      /// Gets the default file name of the R library on the supported platforms.
      /// </summary>
      /// <returns>R dll file name</returns>
      public static string GetRDllFileName()
      {
         switch (GetPlatform())
         {
            case PlatformID.Win32NT:
               return "R.dll";

            case PlatformID.MacOSX:
               return "libR.dylib";

            case PlatformID.Unix:
               return "libR.so";

            default:
               throw new NotSupportedException();
         }
      }
   }
}