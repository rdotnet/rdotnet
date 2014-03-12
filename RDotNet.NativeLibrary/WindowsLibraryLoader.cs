using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ComponentModel;

namespace RDotNet.NativeLibrary
{
   internal class WindowsLibraryLoader : IDynamicLibraryLoader
   {
      public IntPtr LoadLibrary(string filename)
      {
         return InternalLoadLibrary(filename);
      }

      public string GetLastError()
      {
         // see for instance http://blogs.msdn.com/b/shawnfa/archive/2004/09/10/227995.aspx 
         /// and http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx
         // TODO: does this work as expected with Mono+Windows stack?
         return new Win32Exception().Message;
      }

      public bool FreeLibrary(IntPtr handle)
      {
         return InternalFreeLibrary(handle);
      }

      public IntPtr GetFunctionAddress(IntPtr hModule, string lpProcName)
      {
         return InternalGetProcAddress(hModule, lpProcName);
      }

      [DllImport("kernel32.dll", EntryPoint = "LoadLibrary",  SetLastError=true)]
      private static extern IntPtr InternalLoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

      [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool InternalFreeLibrary(IntPtr hModule);

      [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
      private static extern IntPtr InternalGetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

      [DllImport("kernel32.dll", EntryPoint = "GetLastError")]
      [return: MarshalAs(UnmanagedType.LPStr)]
      private static extern string InternalGetLastError();

   }
}
	  
