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
#endif
}
