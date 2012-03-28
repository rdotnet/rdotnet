using System;
using System.Runtime.InteropServices;
using RDotNet.Internals;
using RDotNet.Internals.Windows;

namespace RDotNet
{
	/// <summary>
	/// Represents parameters on R's startup.
	/// </summary>
	/// <remarks>
	/// Wraps RStart struct.
	/// </remarks>
	public class StartupParameter
	{
		private static readonly ulong EnvironmentDependentMaxSize = Environment.Is64BitProcess ? ulong.MaxValue : uint.MaxValue;

		// Windows style RStart includes Unix-style RStart.
		internal RStart start;

		public StartupParameter()
		{
			this.start = new RStart();
			SetDefaultParameter();
		}

		/// <summary>
		/// Gets or sets the value indicating that R runs as quiet mode.
		/// </summary>
		public bool Quiet
		{
			get { return this.start.Common.R_Quiet; }
			set { this.start.Common.R_Quiet = value; }
		}

		/// <summary>
		/// Gets or sets the value indicating that R runs as slave mode.
		/// </summary>
		public bool Slave
		{
			get { return this.start.Common.R_Slave; }
			set { this.start.Common.R_Slave = value; }
		}

		/// <summary>
		/// Gets or sets the value indicating that R runs as interactive mode.
		/// </summary>
		public bool Interactive
		{
			get { return this.start.Common.R_Interactive; }
			set { this.start.Common.R_Interactive = value; }
		}

		/// <summary>
		/// Gets or sets the value indicating that R runs as verbose mode.
		/// </summary>
		public bool Verbose
		{
			get { return this.start.Common.R_Verbose; }
			set { this.start.Common.R_Verbose = value; }
		}

		/// <summary>
		/// Gets or sets the value indicating that R loads the site file.
		/// </summary>
		public bool LoadSiteFile
		{
			get { return this.start.Common.LoadSiteFile; }
			set { this.start.Common.LoadSiteFile = value; }
		}

		/// <summary>
		/// Gets or sets the value indicating that R loads the init file.
		/// </summary>
		public bool LoadInitFile
		{
			get { return this.start.Common.LoadInitFile; }
			set { this.start.Common.LoadInitFile = value; }
		}

		/// <summary>
		/// Gets or sets the value indicating that R debugs the init file.
		/// </summary>
		public bool DebugInitFile
		{
			get { return this.start.Common.DebugInitFile; }
			set { this.start.Common.DebugInitFile = value; }
		}

		/// <summary>
		/// Gets or sets the value indicating that R restores the history.
		/// </summary>
		public StartupRestoreAction RestoreAction
		{
			get { return this.start.Common.RestoreAction; }
			set { this.start.Common.RestoreAction = value; }
		}

		/// <summary>
		/// Gets or sets the value indicating that R saves the history.
		/// </summary>
		public StartupSaveAction SaveAction
		{
			get { return this.start.Common.SaveAction; }
			set { this.start.Common.SaveAction = value; }
		}

		/// <summary>
		/// Gets or sets the minimum memory size.
		/// </summary>
		public ulong MinMemorySize
		{
			get { return this.start.Common.vsize.ToUInt64(); }
			set
			{
				if (value > EnvironmentDependentMaxSize)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.start.Common.vsize = new UIntPtr(value);
			}
		}

		/// <summary>
		/// Gets or sets the minimum number of cons cells.
		/// </summary>
		public ulong MinCellSize
		{
			get { return this.start.Common.nsize.ToUInt64(); }
			set
			{
				if (value > EnvironmentDependentMaxSize)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.start.Common.nsize = new UIntPtr(value);
			}
		}

		/// <summary>
		/// Gets or sets the maximum memory size.
		/// </summary>
		public ulong MaxMemorySize
		{
			get { return this.start.Common.max_vsize.ToUInt64(); }
			set
			{
				if (value > EnvironmentDependentMaxSize)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.start.Common.max_vsize = new UIntPtr(value);
			}
		}

		/// <summary>
		/// Gets or sets the maximum number of cons cells.
		/// </summary>
		public ulong MaxCellSize
		{
			get { return this.start.Common.max_nsize.ToUInt64(); }
			set
			{
				if (value > EnvironmentDependentMaxSize)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.start.Common.max_nsize = new UIntPtr(value);
			}
		}

		/// <summary>
		/// Gets or sets the maximum number of protected pointers in stack.
		/// </summary>
		public ulong StackSize
		{
			get { return this.start.Common.ppsize.ToUInt64(); }
			set
			{
				if (value > EnvironmentDependentMaxSize)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.start.Common.ppsize = new UIntPtr(value);
			}
		}

		/// <summary>
		/// Gets or sets the value indicating that R does NOT load the environment file.
		/// </summary>
		public bool NoRenviron
		{
			get { return this.start.Common.NoRenviron; }
			set { this.start.Common.NoRenviron = value; }
		}

		/// <summary>
		/// Gets or sets the base directory in which R is installed.
		/// </summary>
		/// <remarks>
		/// Only Windows.
		/// </remarks>
		public string RHome
		{
			get
			{
				if (Environment.OSVersion.Platform != PlatformID.Win32NT)
				{
					throw new NotSupportedException();
				}
				return Marshal.PtrToStringAnsi(this.start.rhome);
			}
			set
			{
				if (Environment.OSVersion.Platform != PlatformID.Win32NT)
				{
					throw new NotSupportedException();
				}
				this.start.rhome = Marshal.StringToHGlobalAnsi(value);
			}
		}

		/// <summary>
		/// Gets or sets the default user workspace.
		/// </summary>
		/// <remarks>
		/// Only Windows.
		/// </remarks>
		public string Home
		{
			get
			{
				if (Environment.OSVersion.Platform != PlatformID.Win32NT)
				{
					throw new NotSupportedException();
				}
				return Marshal.PtrToStringAnsi(this.start.home);
			}
			set
			{
				if (Environment.OSVersion.Platform != PlatformID.Win32NT)
				{
					throw new NotSupportedException();
				}
				this.start.home = Marshal.StringToHGlobalAnsi(value);
			}
		}

		/// <summary>
		/// Gets or sets the UI mode.
		/// </summary>
		/// <remarks>
		/// Only Windows.
		/// </remarks>
		public UiMode CharacterMode
		{
			get
			{
				if (Environment.OSVersion.Platform != PlatformID.Win32NT)
				{
					throw new NotSupportedException();
				}
				return this.start.CharacterMode;
			}
			set
			{
				if (Environment.OSVersion.Platform != PlatformID.Win32NT)
				{
					throw new NotSupportedException();
				}
				this.start.CharacterMode = value;
			}
		}

		private void SetDefaultParameter()
		{
			Quiet = true;
			//Slave = false;
			Interactive = true;
			//Verbose = false;
			RestoreAction = StartupRestoreAction.NoRestore;
			SaveAction = StartupSaveAction.NoSave;
			LoadSiteFile = true;
			LoadInitFile = true;
			//DebugInitFile = false;
			MinMemorySize = 6291456;
			MinCellSize = 350000;
			MaxMemorySize = EnvironmentDependentMaxSize;
			MaxCellSize = EnvironmentDependentMaxSize;
			StackSize = 50000;
			//NoRenviron = false;
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				CharacterMode = UiMode.LinkDll;
			}
		}
	}
}
