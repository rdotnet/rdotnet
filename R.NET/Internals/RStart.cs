using System;
using System.Runtime.InteropServices;
using System.Text;

using size_t = System.UIntPtr;

namespace RDotNet.Internals
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
		private size_t vsize;
		private size_t nsize;
		private size_t max_vsize;
		private size_t max_nsize;
		private size_t ppsize;
		[MarshalAs(UnmanagedType.Bool)]
		public bool NoRenviron;
#if WINDOWS
		internal IntPtr rhome;
		internal IntPtr home;
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal blah1 ReadConsole;
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal blah2 WriteConsole;
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal blah3 CallBack;
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal blah4 ShowMessage;
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal blah5 YesNoCancel;
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal blah6 Busy;
		internal UiMode CharacterMode;
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal blah7 WriteConsoleEx;
#endif
		
		private static readonly ulong EnvironmentDependentMaxSize = System.Environment.Is64BitProcess ? ulong.MaxValue : uint.MaxValue;
		
		/// <summary>
		/// Gets and sets the minimum heap memory size in bytes. 
		/// </summary>
		internal ulong MinMemorySize
		{
			get
			{
				return vsize.ToUInt64();
			}
			set
			{
				if (value > EnvironmentDependentMaxSize)
				{
					throw new ArgumentOutOfRangeException();
				}
				vsize = new UIntPtr(value);
			}
		}
		
		/// <summary>
		/// Gets and sets the minimum number of cons cells. 
		/// </summary>
		internal ulong MinCellSize
		{
			get
			{
				return nsize.ToUInt64();
			}
			set
			{
				if (value > EnvironmentDependentMaxSize)
				{
					throw new ArgumentOutOfRangeException();
				}
				nsize = new UIntPtr(value);
			}
		}
		
		/// <summary>
		/// Gets and sets the first heap memory size. 
		/// </summary>
		internal ulong MaxMemorySize
		{
			get
			{
				return max_vsize.ToUInt64();
			}
			set
			{
				if (value > EnvironmentDependentMaxSize)
				{
					throw new ArgumentOutOfRangeException();
				}
				max_vsize = new UIntPtr(value);
			}
		}
		
		/// <summary>
		/// Gets and sets the maximum number of cons cells. 
		/// </summary>
		internal ulong MaxCellSize
		{
			get
			{
				return max_nsize.ToUInt64();
			}
			set
			{
				if (value > EnvironmentDependentMaxSize)
				{
					throw new ArgumentOutOfRangeException();
				}
				max_nsize = new UIntPtr(value);
			}
		}
		
		/// <summary>
		/// Gets and sets the maximum number of protected pointers in the stack. 
		/// </summary>
		internal ulong ProtectedPointerStackSize
		{
			get
			{
				return ppsize.ToUInt64();
			}
			set
			{
				if (value > EnvironmentDependentMaxSize)
				{
					throw new ArgumentOutOfRangeException();
				}
				ppsize = new UIntPtr(value);
			}
		}
	}

#if WINDOWS
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal delegate bool blah1([In] [MarshalAs(UnmanagedType.LPStr)] string prompt, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer, int length, [MarshalAs(UnmanagedType.Bool)] bool history);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void blah2([In] [MarshalAs(UnmanagedType.LPStr)] string buffer, int length);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void blah3();
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void blah4([In] [MarshalAs(UnmanagedType.LPStr)] string message);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate YesNoCancel blah5([In] [MarshalAs(UnmanagedType.LPStr)] string question);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void blah6(BusyType which);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void blah7([In] [MarshalAs(UnmanagedType.LPStr)] string buffer, int length, ConsoleOutputType outputType);

	internal enum UiMode
	{
		RGui,
		RTerminal,
		LinkDll,
	}
#elif MAC || LINUX
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
#endif
}
