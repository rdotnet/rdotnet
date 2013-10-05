using System;
using System.ComponentModel;
using System.Diagnostics;

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
            using (var uname = new Process())
            {
               uname.StartInfo.FileName = "uname";
               uname.StartInfo.Arguments = "-s";
               uname.StartInfo.RedirectStandardOutput = true;
               uname.StartInfo.UseShellExecute = false;
               uname.StartInfo.CreateNoWindow = true;
               uname.Start();
               var kernelName = uname.StandardOutput.ReadLine();
               uname.WaitForExit();
               return kernelName == "Darwin" ? PlatformID.MacOSX : platform;
            }
         }
         catch (Win32Exception) // probably no PATH to uname.
         {
            return platform;
         }
      }
   }
}