using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RDotNet.R.Adapter.Windows;

namespace RDotNet.R.Adapter
{
    public class InstanceStartupParameter
    {
        public RStartupParameter RStartupParameter;

        public ICharacterDevice CharacterDevice { get; set; }

        public bool Quiet
        {
            get { return RStartupParameter.Common.R_Quiet; }
            set { RStartupParameter.Common.R_Quiet = value; }
        }

        public bool Slave
        {
            get { return RStartupParameter.Common.R_Slave; }
            set { RStartupParameter.Common.R_Slave = value; }
        }

        public bool Interactive
        {
            get { return RStartupParameter.Common.R_Interactive; }
            set { RStartupParameter.Common.R_Interactive = value; }
        }

        public bool Verbose
        {
            get { return RStartupParameter.Common.R_Verbose; }
            set { RStartupParameter.Common.R_Verbose = value; }
        }

        public bool LoadSiteFile
        {
            get { return RStartupParameter.Common.LoadSiteFile; }
            set { RStartupParameter.Common.LoadSiteFile = value; }
        }

        public bool LoadInitFile
        {
            get { return RStartupParameter.Common.LoadInitFile; }
            set { RStartupParameter.Common.LoadInitFile = value; }
        }

        public bool DebugInitFile
        {
            get { return RStartupParameter.Common.DebugInitFile; }
            set { RStartupParameter.Common.DebugInitFile = value; }
        }

        public StartupRestoreAction RestoreAction
        {
            get { return RStartupParameter.Common.RestoreAction; }
            set { RStartupParameter.Common.RestoreAction = value; }
        }

        public StartupSaveAction SaveAction
        {
            get { return RStartupParameter.Common.SaveAction; }
            set { RStartupParameter.Common.SaveAction = value; }
        }

        public ulong MinMemorySize
        {
            get { return RStartupParameter.Common.vsize.ToUInt64(); }
            set
            {
                RStartupParameter.Common.vsize = new UIntPtr(value);
            }
        }

        public ulong MinCellSize
        {
            get { return RStartupParameter.Common.nsize.ToUInt64(); }
            set
            {
                RStartupParameter.Common.nsize = new UIntPtr(value);
            }
        }

        public ulong MaxMemorySize
        {
            get { return RStartupParameter.Common.max_vsize.ToUInt64(); }
            set
            {
                RStartupParameter.Common.max_vsize = new UIntPtr(value);
            }
        }

        public ulong MaxCellSize
        {
            get { return RStartupParameter.Common.max_nsize.ToUInt64(); }
            set
            {
                RStartupParameter.Common.max_nsize = new UIntPtr(value);
            }
        }

        public ulong StackSize
        {
            get { return RStartupParameter.Common.ppsize.ToUInt64(); }
            set
            {
                RStartupParameter.Common.ppsize = new UIntPtr(value);
            }
        }

        public bool NoRenviron
        {
            get { return RStartupParameter.Common.NoRenviron; }
            set { RStartupParameter.Common.NoRenviron = value; }
        }

        public string RHome
        {
            get
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT) throw new PlatformNotSupportedException();
                return Marshal.PtrToStringAnsi(RStartupParameter.rhome);
            }
            set
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT) throw new PlatformNotSupportedException();
                RStartupParameter.rhome = Marshal.StringToHGlobalAnsi(value);
            }
        }

        public string Home
        {
            get
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT) throw new PlatformNotSupportedException();
                return Marshal.PtrToStringAnsi(RStartupParameter.home);
            }
            set
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT) throw new PlatformNotSupportedException();
                RStartupParameter.home = Marshal.StringToHGlobalAnsi(value);
            }
        }

        public UiMode CharacterMode
        {
            get
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT) throw new PlatformNotSupportedException();
                return RStartupParameter.CharacterMode;
            }
            set
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT) throw new PlatformNotSupportedException();
                RStartupParameter.CharacterMode = value;
            }
        }

        public List<string> BuildArguments()
        {
            var argv = new List<string> { "RDotNet_app" };
            if (Quiet && !Interactive) argv.Add("--quiet");
            if (Slave) argv.Add("--slave");
            if (Interactive) argv.Add("--interactive");
            if (Verbose) argv.Add("--verbose");
            if (!LoadSiteFile) argv.Add("--no-site-file");
            if (!LoadInitFile) argv.Add("--no-init-file");
            if (NoRenviron) argv.Add("--no-environ");

            switch (SaveAction)
            {
                case StartupSaveAction.NoSave:
                    argv.Add("--no-save");
                    break;
                case StartupSaveAction.Save:
                    argv.Add("--save");
                    break;
            }
            switch (RestoreAction)
            {
                case StartupRestoreAction.NoRestore:
                    argv.Add("--no-restore-data");
                    break;
                case StartupRestoreAction.Restore:
                    argv.Add("--restore");
                    break;
            }

            if (MaxMemorySize != (Environment.Is64BitProcess ? UInt64.MaxValue : UInt32.MaxValue))
            {
                argv.Add("--max-mem-size=" + MaxMemorySize);
            }
            argv.Add("--max-ppsize=" + StackSize);
            return argv;
        }
    }
}
