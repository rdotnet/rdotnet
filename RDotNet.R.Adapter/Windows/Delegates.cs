using System.Runtime.InteropServices;
using System.Text;

namespace RDotNet.R.Adapter.Windows
{
   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   [return: MarshalAs(UnmanagedType.Bool)]
   public delegate bool ReadConsoleDelegate([In] [MarshalAs(UnmanagedType.LPStr)] string prompt, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer, int length, [MarshalAs(UnmanagedType.Bool)] bool history);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   public delegate void WriteConsoleDelegate([In] [MarshalAs(UnmanagedType.LPStr)] string buffer, int length);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   public delegate void CallbackDelegate();

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   public delegate void ShowMessageDelegate([In] [MarshalAs(UnmanagedType.LPStr)] string message);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   public delegate YesNoCancel YesNoCancelDelegate([In] [MarshalAs(UnmanagedType.LPStr)] string question);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   public delegate void BusyDelegate(BusyType which);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   public delegate void WriteConsoleExDelegate([In] [MarshalAs(UnmanagedType.LPStr)] string buffer, int length, ConsoleOutputType outputType);
}
