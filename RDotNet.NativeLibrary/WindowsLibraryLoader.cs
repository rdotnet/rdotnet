using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Security.Permissions;
using System.Text;

namespace RDotNet.NativeLibrary
{
   [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
   public class WindowsLibraryLoader : IDynamicLibraryLoader
   {
      public IntPtr LoadLibrary(string filename)
      {
         return InternalLoadLibrary(filename);
      }

      public string GetLastError()
      {
         // see for instance http://blogs.msdn.com/b/shawnfa/archive/2004/09/10/227995.aspx 
         // and http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx
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

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool SetDllDirectory(string lpPathName);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern int GetDllDirectory(int nBufferLength, StringBuilder lpPathName);
       
       const int MAX_PATH_LENGTH = 255;

      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      private static extern int GetShortPathName(
          [MarshalAs(UnmanagedType.LPTStr)]
         string path,
          [MarshalAs(UnmanagedType.LPTStr)]
         StringBuilder shortPath,
          int shortPathLength
          );

      /// <summary>
      /// Gets the old style DOS short path (8.3 format) given a path name
      /// </summary>
      /// <param name="path">A path</param>
      /// <returns>The short path name according to the Windows kernel32 API</returns>
      internal protected static string GetShortPath(string path)
      {
         var shortPath = new StringBuilder(MAX_PATH_LENGTH);
         GetShortPathName(path, shortPath, MAX_PATH_LENGTH);
         return shortPath.ToString();
      }

   }
}
	  
