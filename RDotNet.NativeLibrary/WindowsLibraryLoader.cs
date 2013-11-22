using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
namespace RDotNet.NativeLibrary
{
   internal class WindowsLibraryLoader
   {
      [DllImport("kernel32.dll")]
      internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
      // <summary>
      /// Adds a directory to the search path used to locate DLLs for the application.
      /// </summary>
      /// <remarks>
      /// Calls <c>SetDllDirectory</c> in the kernel32.dll on Windows.
      /// </remarks>
      /// <param name="dllDirectory">
      /// The directory to be added to the search path.
      /// If this parameter is an empty string (""), the call removes the current directory from the default DLL search order.
      /// If this parameter is NULL, the function restores the default search order.
      /// </param>
      /// <returns>If the function succeeds, the return value is nonzero.</returns>
      [Obsolete("Set environment variable 'PATH' instead.")]
      [DllImport("kernel32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool SetDllDirectory([MarshalAs(UnmanagedType.LPStr)] string dllDirectory);

      [DllImport("kernel32.dll")]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool FreeLibrary(IntPtr hModule);
      [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
      internal static extern IntPtr GetFunctionAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

   }
}
	  
