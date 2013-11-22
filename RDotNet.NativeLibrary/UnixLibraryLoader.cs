using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Linq;
using System.IO;
namespace RDotNet.NativeLibrary
{
   internal static class UnixLibraryLoader
   {
      private const string LibraryPath = "PATH";
      private static readonly string DefaultSearchPath = System.Environment.GetEnvironmentVariable(LibraryPath, EnvironmentVariableTarget.Process);
      internal static IntPtr LoadLibrary(string filename)
      {
         const int RTLD_LAZY = 0x1;
         if (filename.StartsWith("/"))
         {
            return dlopen(filename, RTLD_LAZY);
         }
         var searchPaths = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator);
         var dll = searchPaths.Select(directory => Path.Combine(directory, filename)).FirstOrDefault(File.Exists);
         if (dll == null)
         {
            throw new DllNotFoundException("Could not find the file: " + filename + " on the search path.  Checked these directories:\n "
                + String.Join("\n", searchPaths));
         }
         return dlopen(dll, RTLD_LAZY);
      }
      [DllImport("libdl")]
      private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPStr)] string filename, int flag);
      [DllImport("libdl", EntryPoint = "dlclose")]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool FreeLibrary(IntPtr hModule);
      [DllImport("libdl", EntryPoint = "dlsym")]
      internal static extern IntPtr GetFunctionAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);


   }
}
	  
