using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RDotNet.Internals.Unix
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ptr_R_Suicide([In] [MarshalAs(UnmanagedType.LPStr)] string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ptr_R_ShowMessage([In] [MarshalAs(UnmanagedType.LPStr)] string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal delegate bool ptr_R_ReadConsole([In] [MarshalAs(UnmanagedType.LPStr)] string prompt, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer, int length, [MarshalAs(UnmanagedType.Bool)] bool history);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ptr_R_WriteConsole([In] [MarshalAs(UnmanagedType.LPStr)] string buffer, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ptr_R_WriteConsoleEx([In] [MarshalAs(UnmanagedType.LPStr)] string buffer, int length, ConsoleOutputType outputType);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ptr_R_ResetConsole();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ptr_R_FlushConsole();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ptr_R_ClearerrConsole();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ptr_R_Busy(BusyType which);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ptr_R_CleanUp(StartupSaveAction saveAction, int status, [MarshalAs(UnmanagedType.Bool)] bool runLast);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal delegate bool ptr_R_ShowFiles(int count, [In] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] files, [In] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] headers, [In] [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.Bool)] bool delete, [In] [MarshalAs(UnmanagedType.LPStr)] string pager);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal delegate int ptr_R_ChooseFile([MarshalAs(UnmanagedType.Bool)] bool create, StringBuilder buffer, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ptr_R_EditFile([MarshalAs(UnmanagedType.LPStr)] string file);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr ptr_R_loadhistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr ptr_R_savehistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr ptr_R_addhistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment);
}