using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

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
            if (platform == PlatformID.Unix)
            {
                try
                {
                  var kernelName = ExecCommand("uname", "-s");
                    platform = (kernelName == "Darwin" ? PlatformID.MacOSX : platform);
         }
                catch (Win32Exception)
                { }
      }

            return platform;
      }

      /// <summary>
        /// Sets the PATH to the R binaries and R_HOME environment variables if needed.
      /// </summary>
      /// <param name="rHome">The path for R_HOME. If null (default), the function checks the R_HOME environment variable. If none is set, 
      /// the function uses platform specific sensible default behaviors.</param>
      /// <remarks>
      /// This function has been designed to limit the tedium for users, while allowing custom settings for unusual installations.
      /// </remarks>
        public static void SetEnvironmentVariables(string rHome = null)
      {
         if (string.IsNullOrEmpty(rHome))
            {
                rHome = Environment.GetEnvironmentVariable("R_HOME");
         if (string.IsNullOrEmpty(rHome))
         {
                    rHome = FindRHome();
         }

                if (string.IsNullOrEmpty(rHome))
                    throw new InvalidOperationException("Could not find R_HOME in current environment or registry.");
      }
            var rPath = ConstructRPath(rHome);

            if (!Directory.Exists(rHome))
                throw new DirectoryNotFoundException(string.Format("R_HOME directory does not exist: '{0}'", rHome));

            if (!Directory.Exists(rPath))
                throw new DirectoryNotFoundException(string.Format("Directory for R binaries does not exist: '{0}'", rPath));

            Environment.SetEnvironmentVariable("R_HOME", rHome);
            Environment.SetEnvironmentVariable("PATH", rPath + ";" + Environment.GetEnvironmentVariable("PATH"));
      }

        private static string FindRHome(string rPath = null)
      {
         var platform = GetPlatform();
         string rHome;
         switch (platform)
         {
            case PlatformID.Win32NT:
                    rHome = WindowsLibraryLoader.GetRHomeFromRegistry();
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

        private static string ConstructRPath(string rHome)
      {
            var shlibFilename = GetRLibraryFileName();
            var platform = GetPlatform();
            switch (platform)
      {
                case PlatformID.Win32NT:
                    var rPath = Path.Combine(rHome, "bin");
                    var rVersion = WindowsLibraryLoader.GetRVersionFromRegistry();
                    if (rVersion.Major > 2 || (rVersion.Major == 2 && rVersion.Minor >= 12))
                    {
                        var bitness = Environment.Is64BitProcess ? "x64" : "i386";
                        rPath = Path.Combine(rPath, bitness);
      }

                    return rPath;

         case PlatformID.MacOSX: // TODO: is there a way to detect installations on MacOS
            return "/Library/Frameworks/R.framework/Libraries";

         case PlatformID.Unix:  
            var rexepath = ExecCommand("which", "R"); // /usr/bin/R,  or /usr/local/bin/R
                    if (string.IsNullOrEmpty(rexepath)) return "/usr/lib";
               var bindir = Path.GetDirectoryName(rexepath);  //   /usr/local/bin
               // Trying to emulate the start of the R shell script
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

        public static string GetRLibraryFileName()
      {
         var p = GetPlatform();
            switch (p)
            {
         case PlatformID.Win32NT:
            return "R.dll";

         case PlatformID.MacOSX:
            return "libR.dylib";

         case PlatformID.Unix:
            return "libR.so";

         default:
                    throw new PlatformNotSupportedException();
         }
      }

      /// <summary>
      /// Is the platform a unix like (Unix or MacOX)
      /// </summary>
        public static bool IsUnix
        {
            get
            {
            var p = GetPlatform();
            return p == PlatformID.MacOSX || p == PlatformID.Unix;
         }
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
                uname.WaitForExit();
                return kernelName;
            }
        }
   }
}