using System;
using System.Runtime.InteropServices;

namespace RDotNet.Internals.Unix
{
   [StructLayout(LayoutKind.Sequential)]
   internal struct RStart
   {
      [MarshalAs(UnmanagedType.Bool)]
      public bool R_Quiet;

      [MarshalAs(UnmanagedType.Bool)]
      public bool R_Slave;

      [MarshalAs(UnmanagedType.Bool)]
      public bool R_Interactive;

      [MarshalAs(UnmanagedType.Bool)]
      public bool R_Verbose;

      [MarshalAs(UnmanagedType.Bool)]
      public bool LoadSiteFile;

      [MarshalAs(UnmanagedType.Bool)]
      public bool LoadInitFile;

      [MarshalAs(UnmanagedType.Bool)]
      public bool DebugInitFile;

      public StartupRestoreAction RestoreAction;
      public StartupSaveAction SaveAction;
      internal UIntPtr vsize;
      internal UIntPtr nsize;
      internal UIntPtr max_vsize;
      internal UIntPtr max_nsize;
      internal UIntPtr ppsize;

      [MarshalAs(UnmanagedType.Bool)]
      public bool NoRenviron;
   }
}