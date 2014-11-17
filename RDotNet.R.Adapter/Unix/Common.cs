using System;
using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter.Unix
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Common
    {
        [MarshalAs(UnmanagedType.Bool)] public bool R_Quiet;
        [MarshalAs(UnmanagedType.Bool)] public bool R_Slave;
        [MarshalAs(UnmanagedType.Bool)] public bool R_Interactive;
        [MarshalAs(UnmanagedType.Bool)] public bool R_Verbose;
        [MarshalAs(UnmanagedType.Bool)] public bool LoadSiteFile;
        [MarshalAs(UnmanagedType.Bool)] public bool LoadInitFile;
        [MarshalAs(UnmanagedType.Bool)] public bool DebugInitFile;

        public StartupRestoreAction RestoreAction;
        public StartupSaveAction SaveAction;
        public UIntPtr vsize;
        public UIntPtr nsize;
        public UIntPtr max_vsize;
        public UIntPtr max_nsize;
        public UIntPtr ppsize;

        [MarshalAs(UnmanagedType.Bool)] public bool NoRenviron;
    }
}
