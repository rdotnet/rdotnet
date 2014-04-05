using System;
using System.Runtime.InteropServices;

namespace RDotNet.Internals
{
   [StructLayout(LayoutKind.Sequential)]
   internal struct SEXPREC
   {
      private SEXPREC_HEADER header;
      private u u;

      public sxpinfo sxpinfo
      {
         get { return this.header.sxpinfo; }
      }

      public IntPtr attrib
      {
         get { return this.header.attrib; }
      }

      public IntPtr gengc_next_node
      {
         get { return this.header.gengc_next_node; }
      }

      public IntPtr gengc_prev_node
      {
         get { return this.header.gengc_prev_node; }
      }

      internal primsxp primsxp
      {
         get { return this.u.primsxp; }
      }

      internal symsxp symsxp
      {
         get { return this.u.symsxp; }
      }

      internal listsxp listsxp
      {
         get { return this.u.listsxp; }
      }

      internal envsxp envsxp
      {
         get { return this.u.envsxp; }
      }

      internal closxp closxp
      {
         get { return this.u.closxp; }
      }

      internal promsxp promsxp
      {
         get { return this.u.promsxp; }
      }
   }

   [StructLayout(LayoutKind.Explicit)]
   internal struct u
   {
      [FieldOffset(0)]
      internal primsxp primsxp;

      [FieldOffset(0)]
      internal symsxp symsxp;

      [FieldOffset(0)]
      internal listsxp listsxp;

      [FieldOffset(0)]
      internal envsxp envsxp;

      [FieldOffset(0)]
      internal closxp closxp;

      [FieldOffset(0)]
      internal promsxp promsxp;
   }

   [StructLayout(LayoutKind.Sequential)]
   internal struct vecsxp
   {
      public int length;
      public int truelength;
   }

   [StructLayout(LayoutKind.Sequential)]
   internal struct primsxp
   {
      public int offset;
   }

   [StructLayout(LayoutKind.Sequential)]
   internal struct symsxp
   {
      public IntPtr pname;
      public IntPtr value;
      public IntPtr @internal;
   }

   [StructLayout(LayoutKind.Sequential)]
   internal struct listsxp
   {
      public IntPtr carval;
      public IntPtr cdrval;
      public IntPtr tagval;
   }

   [StructLayout(LayoutKind.Sequential)]
   internal struct envsxp
   {
      public IntPtr frame;
      public IntPtr enclos;
      public IntPtr hashtab;
   }

   [StructLayout(LayoutKind.Sequential)]
   internal struct closxp
   {
      public IntPtr formals;
      public IntPtr body;
      public IntPtr env;
   }

   [StructLayout(LayoutKind.Sequential)]
   internal struct promsxp
   {
      public IntPtr value;
      public IntPtr expr;
      public IntPtr env;
   }
}