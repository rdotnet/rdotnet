using System;
using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter.Graphics
{
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
   public struct GraphicsEngineDeviceDescription
   {
      public IntPtr dev;

      [MarshalAs(UnmanagedType.Bool)]
      public bool displayListOn;

      public IntPtr displayList;
      public IntPtr DLlastElt;
      public IntPtr savedSnapshot;

      [MarshalAs(UnmanagedType.Bool)]
      public bool dirty;

      [MarshalAs(UnmanagedType.Bool)]
      public bool recordGraphics;

      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
      public IntPtr[] gesd;

      [MarshalAs(UnmanagedType.Bool)]
      public bool ask;
   }
}
