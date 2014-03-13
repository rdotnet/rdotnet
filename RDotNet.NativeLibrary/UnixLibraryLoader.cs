using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Linq;
using System.IO;

namespace RDotNet.NativeLibrary
{
   internal class UnixLibraryLoader : IDynamicLibraryLoader
   {
      public IntPtr LoadLibrary(string filename)
      {
         return InternalLoadLibrary(filename);
      }

      /// <summary>
      /// Gets the last error. NOTE: according to http://tldp.org/HOWTO/Program-Library-HOWTO/dl-libraries.html, returns NULL if called more than once after dlopen.
      /// </summary>
      /// <returns>The last error.</returns>
      public string GetLastError()
      {
         return dlerror();
      }

      public bool FreeLibrary(IntPtr hModule)
      {
         return dlclose(hModule);
      }

      public IntPtr GetFunctionAddress(IntPtr hModule, string lpProcName)
      {
         return dlsym(hModule, lpProcName);
      }

      private const string LibraryPath = "PATH";
      private static readonly string DefaultSearchPath = System.Environment.GetEnvironmentVariable(LibraryPath, EnvironmentVariableTarget.Process);

      internal static IntPtr InternalLoadLibrary(string filename)
      {
         const int RTLD_LAZY = 0x1;
         if (filename.StartsWith("/")) {
            return dlopen(filename, RTLD_LAZY);
         }
         var searchPaths = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator);
         var dll = searchPaths.Select(directory => Path.Combine(directory, filename)).FirstOrDefault(File.Exists);
         if (dll == null) {
            throw new DllNotFoundException("Could not find the file: " + filename + " on the search path.  Checked these directories:\n "
               + String.Join("\n", searchPaths));
         }
         return dlopen(dll, RTLD_LAZY);
      }

      [DllImport("libdl")]
      private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPStr)] string filename, int flag);

      [DllImport("libdl")]
      [return: MarshalAs(UnmanagedType.LPStr)]
      private static extern string dlerror();

      [DllImport("libdl", EntryPoint = "dlclose")]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool dlclose(IntPtr hModule);

      [DllImport("libdl", EntryPoint = "dlsym")]
      private static extern IntPtr dlsym(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);
   }
}
	  
