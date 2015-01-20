using System.Runtime.InteropServices;
using System.Text;

namespace RDotNet.Internals.Windows
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal delegate bool blah1([In] [MarshalAs(UnmanagedType.LPStr)] string prompt, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer, int length, [MarshalAs(UnmanagedType.Bool)] bool history);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void blah2([In] [MarshalAs(UnmanagedType.LPStr)] string buffer, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void blah3();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void blah4([In] [MarshalAs(UnmanagedType.LPStr)] string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate YesNoCancel blah5([In] [MarshalAs(UnmanagedType.LPStr)] string question);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void blah6(BusyType which);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void blah7([In] [MarshalAs(UnmanagedType.LPStr)] string buffer, int length, ConsoleOutputType outputType);
}