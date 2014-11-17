using System;
using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter
{
   [StructLayout(LayoutKind.Sequential)]
   internal struct SEXPREC_HEADER
   {
      public sxpinfo sxpinfo;
      public IntPtr attrib;
      public IntPtr gengc_next_node;
      public IntPtr gengc_prev_node;
   }
}
