using System;
using System.Runtime.InteropServices;

namespace RDotNet.Graphics.Internals
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct DevDesc
	{
		internal double left;
		internal double right;
		internal double bottom;
		internal double top;
		internal double clipLeft;
		internal double clipRight;
		internal double clipBottom;
		internal double clipTop;
		internal double xCharOffset;
		internal double yCharOffset;
		internal double yLineBias;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.R8)]
		internal double[] ipr;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.R8)]
		internal double[] cra;
		internal double gamma;
		[MarshalAs(UnmanagedType.Bool)]
		internal bool canClip;
		[MarshalAs(UnmanagedType.Bool)]
		internal bool canChangeGamma;
		internal Adjustment canHAdj;
		internal double startps;
		internal Color startcol;
		internal Color startfill;
		internal LineType startlty;
		internal int startfont;
		internal double startgamma;
		internal IntPtr deviceSpecific;
		[MarshalAs(UnmanagedType.Bool)]
		internal bool displayListOn;
		[MarshalAs(UnmanagedType.Bool)]
		internal bool canGenMouseDown;
		[MarshalAs(UnmanagedType.Bool)]
		internal bool canGenMouseMove;
		[MarshalAs(UnmanagedType.Bool)]
		internal bool canGenMouseUp;
		[MarshalAs(UnmanagedType.Bool)]
		internal bool canGenKeybd;
		[MarshalAs(UnmanagedType.Bool)]
		internal bool gettingEvent;
		internal _DevDesc_activate activate;
		internal _DevDesc_circle circle;
		internal _DevDesc_clip clip;
		internal _DevDesc_close close;
		internal _DevDesc_deactivate deactivate;
		internal _DevDesc_locator locator;
		internal _DevDesc_line line;
		internal _DevDesc_metricInfo metricInfo;
		internal _DevDesc_mode mode;
		internal _DevDesc_newPage newPage;
		internal _DevDesc_polygon polygon;
		internal _DevDesc_Polyline polyline;
		internal _DevDesc_rect rect;
		internal _DevDesc_path path;
		internal _DevDesc_raster raster;
		internal _DevDesc_cap cap;
		internal _DevDesc_size size;
		internal _DevDesc_strWidth strWidth;
		internal _DevDesc_text text;
		internal _DevDesc_onExit onExit;
		internal _DevDesc_getEvent getEvent;
		internal _DevDesc_newFrameConfirm newFrameConfirm;
		[MarshalAs(UnmanagedType.Bool)]
		internal bool hasTextUTF8;
		//internal _DevDesc_textUTF8 textUTF8;
		//internal _DevDesc_strWidthUTF8 strWidthUTF8;
		internal _DevDesc_text textUTF8;
		internal _DevDesc_strWidth strWidthUTF8;
		[MarshalAs(UnmanagedType.Bool)]
		internal bool wantSymbolUTF8;
		[MarshalAs(UnmanagedType.Bool)]
		internal bool useRotatedTextInContour;
		internal IntPtr eventEnv;
		internal _DevDesc_eventHelper eventHelper;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		internal string reserved;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_activate(IntPtr pDevDesc);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_circle(double x, double y, double r, IntPtr gc, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_clip(double x0, double x1, double y0, double y1, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_close(IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_deactivate(IntPtr pDevDesc);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal delegate bool _DevDesc_locator(out double x, out double y, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_line(double x1, double y1, double x2, double y2, IntPtr gc, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_metricInfo(int c, IntPtr gc, out double ascent, out double descent, out double width, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_mode(int mode, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_newPage(IntPtr gc, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_polygon(int n, IntPtr x, IntPtr y, IntPtr gc, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_Polyline(int n, IntPtr x, IntPtr y, IntPtr gc, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_rect(double x0, double y0, double x1, double y1, IntPtr gc, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_path(IntPtr x, IntPtr y, int npoly, IntPtr nper, [MarshalAs(UnmanagedType.Bool)] bool winding, IntPtr gc, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_raster(IntPtr raster, int w, int h, double x, double y, double width, double height, double rot, [MarshalAs(UnmanagedType.Bool)] bool interpolate, IntPtr gc, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate IntPtr _DevDesc_cap(IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_size(out double left, out double right, out double bottom, out double top, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate double _DevDesc_strWidth([MarshalAs(UnmanagedType.LPStr)] string str, IntPtr gc, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_text(double x, double y, [MarshalAs(UnmanagedType.LPStr)] string str, double rot, double hadj, IntPtr gc, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_onExit(IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate IntPtr _DevDesc_getEvent(IntPtr sexp, [MarshalAs(UnmanagedType.LPStr)] string e);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal delegate bool _DevDesc_newFrameConfirm(IntPtr dd);
	//[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	//internal delegate void _DevDesc_textUTF8(double x, double y, [MarshalAs(UnmanagedType.LPStr)] string str, double rot, double hadj, IntPtr gc, IntPtr dd);
	//[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	//internal delegate double _DevDesc_strWidthUTF8([MarshalAs(UnmanagedType.LPStr)] string str, IntPtr gc, IntPtr dd);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void _DevDesc_eventHelper(IntPtr dd, int code);
}
