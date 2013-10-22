using Microsoft.Win32;
using System;
using System.IO;
using RDotNet.NativeLibrary;

namespace RDotNet
{
   public static class Helper
   {
      public static void SetEnvironmentVariables()
      {
         var rhome = Environment.GetEnvironmentVariable("R_HOME");
         var currentPath = Environment.GetEnvironmentVariable("PATH");
         switch (NativeUtility.GetPlatform())
         {
            case PlatformID.Win32NT:
               Environment.SetEnvironmentVariable("PATH", FindRPathFromRegistry() + Path.PathSeparator + currentPath);
               break;

            case PlatformID.MacOSX:
               if (string.IsNullOrEmpty(rhome))
               {
                  Environment.SetEnvironmentVariable("R_HOME", "/Library/Frameworks/R.framework/Resources");
               }
               Environment.SetEnvironmentVariable("PATH", "/Library/Frameworks/R.framework/Libraries" + Path.PathSeparator + currentPath);
               break;

            case PlatformID.Unix:
               if (string.IsNullOrEmpty(rhome))
               {
                  Environment.SetEnvironmentVariable("R_HOME", "/usr/lib/R");
               }
               // TODO: cater for cases where user has build R from source and installed to e.g. /usr/local/lib
               Environment.SetEnvironmentVariable("PATH", "/usr/lib" + Path.PathSeparator + currentPath);
               break;
         }
      }

      public static string FindRPathFromRegistry()
      {
         var rCore = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\R-core");
         if (rCore == null)
         {
            throw new ApplicationException("Registry key is not found.");
         }
         var is64Bit = Environment.Is64BitProcess;
         var r = rCore.OpenSubKey(is64Bit ? "R64" : "R");
         if (r == null)
         {
            throw new ApplicationException("Registry key is not found.");
         }
         var currentVersion = new Version((string)r.GetValue("Current Version"));
         var installPath = (string)r.GetValue("InstallPath");
         var bin = Path.Combine(installPath, "bin");
         // Up to 2.11.x, DLLs are installed in R_HOME\bin.
         // From 2.12.0, DLLs are installed in the one level deeper directory.
         return currentVersion < new Version(2, 12) ? bin : Path.Combine(bin, is64Bit ? "x64" : "i386");
      }
   }
}