using System;
using System.Runtime.InteropServices;
using RDotNet.R.Adapter.Unix;

namespace RDotNet.R.Adapter.Windows
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RStartupParameter
    {
        public Common Common;
        public IntPtr rhome;
        public IntPtr home;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public ReadConsoleDelegate ReadConsole;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WriteConsoleDelegate WriteConsole;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public CallbackDelegate CallBack;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public ShowMessageDelegate ShowMessage;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public YesNoCancelDelegate YesNoCancel;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public BusyDelegate Busy;

        public UiMode CharacterMode;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WriteConsoleExDelegate WriteConsoleEx;
    }
}
