using System;
using System.Runtime.InteropServices;

namespace RDotNet.Graphics.Internals
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct GEDevDesc
    {
        internal IntPtr dev;

        [MarshalAs(UnmanagedType.Bool)]
        internal bool displayListOn;

        internal IntPtr displayList;
        internal IntPtr DLlastElt;
        internal IntPtr savedSnapshot;

        [MarshalAs(UnmanagedType.Bool)]
        internal bool dirty;

        [MarshalAs(UnmanagedType.Bool)]
        internal bool recordGraphics;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        internal IntPtr[] gesd;

        [MarshalAs(UnmanagedType.Bool)]
        internal bool ask;
    }
}