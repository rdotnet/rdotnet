using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RDotNet.Internals
{
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
		public uint vsize;
		public uint nsize;
		public uint max_vsize;
		public uint max_nsize;
		public uint ppsize;
		[MarshalAs(UnmanagedType.Bool)]
		public bool NoRenviron;
#if !MAC && !LINUX
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
}
