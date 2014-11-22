using System;
using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter.Graphics
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DevDesc
    {
        public double left;
        public double right;
        public double bottom;
        public double top;
        public double clipLeft;
        public double clipRight;
        public double clipBottom;
        public double clipTop;
        public double xCharOffset;
        public double yCharOffset;
        public double yLineBias;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.R8)]
        public double[] ipr;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.R8)]
        public double[] cra;
        
        public double gamma;
        
        [MarshalAs(UnmanagedType.Bool)]
        public bool canClip;
        
        [MarshalAs(UnmanagedType.Bool)]
        public bool canChangeGamma;
        
        public Adjustment canHAdj;
        public double startps;
        public Color startcol;
        public Color startfill;
        public LineType startlty;
        public int startfont;
        public double startgamma;
        public IntPtr deviceSpecific;
        
        [MarshalAs(UnmanagedType.Bool)]
        public bool displayListOn;
        
        [MarshalAs(UnmanagedType.Bool)]
        public bool canGenMouseDown;
        
        [MarshalAs(UnmanagedType.Bool)]
        public bool canGenMouseMove;
        
        [MarshalAs(UnmanagedType.Bool)]
        public bool canGenMouseUp;
        
        [MarshalAs(UnmanagedType.Bool)]
        public bool canGenKeybd;
        
        [MarshalAs(UnmanagedType.Bool)]
        public bool gettingEvent;
        
        public GDActivate activate;
        public GDCircle circle;
        public GDClip clip;
        public GDClose close;
        public GDDeactivate deactivate;
        public GDLocator locator;
        public GDLine line;
        public GDMetricInfo metricInfo;
        public GDMode mode;
        public GDNewPage newPage;
        public GDPolygon polygon;
        public GDPolyline polyline;
        public GDRect rect;
        public GDPath path;
        public Raster raster;
        public GDCap cap;
        public GDSize size;
        public GDStrWidth strWidth;
        public GDText text;
        public GDOnExit onExit;
        public GDGetEvent getEvent;
        public GDNewFrameConfirm newFrameConfirm;
        
        [MarshalAs(UnmanagedType.Bool)]
        public bool hasTextUTF8;
        
        public GDText textUTF8;
        public GDStrWidth strWidthUTF8;
        
        [MarshalAs(UnmanagedType.Bool)]
        public bool wantSymbolUTF8;
        
        [MarshalAs(UnmanagedType.Bool)]
        public bool useRotatedTextInContour;
        
        public IntPtr eventEnv;
        public GDEventHelper eventHelper;
        public GDHoldFlush holdFlush;
        public YesOrNo haveTransparency; /* 1 = no, 2 = yes */
        public YesNoOrMostly haveTransparentBg; /* 1 = no, 2 = fully, 3 = semi */
        public YesNoOrMostly haveRaster; /* 1 = no, 2 = yes, 3 = except for missing values */
        public YesOrNo haveCapture;
        public YesOrNo haveLocator; /* 1 = no, 2 = yes */
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string reserved;
    }

    public enum YesOrNo
    {
        No = 1,
        Yes = 2,
    }

    public enum YesNoOrMostly
    {
        No = 1,
        Yes = 2,
        Mostly = 3
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDActivate(IntPtr pDevDesc);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDCircle(double x, double y, double r, IntPtr gc, IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDClip(double x0, double x1, double y0, double y1, IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDClose(IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDDeactivate(IntPtr pDevDesc);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool GDLocator(out double x, out double y, IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDLine(double x1, double y1, double x2, double y2, IntPtr gc, IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDMetricInfo(int c,
                                      IntPtr gc,
                                      out double ascent, 
                                      out double descent,
                                      out double width,
                                      IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDMode(int mode, IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDNewPage(IntPtr gc, IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDPolygon(int n, IntPtr x, IntPtr y, IntPtr gc, IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDPolyline(int n, IntPtr x, IntPtr y, IntPtr gc, IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDRect(double x0, double y0, double x1, double y1, IntPtr gc, IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDPath(IntPtr x,
                                IntPtr y,
                                int npoly,
                                IntPtr nper,
                                [MarshalAs(UnmanagedType.Bool)] bool winding,
                                IntPtr gc,
                                IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDRaster(IntPtr raster,
                                  int w,
                                  int h,
                                  double x,
                                  double y,
                                  double width,
                                  double height,
                                  double rot,
                                  [MarshalAs(UnmanagedType.Bool)] bool interpolate,
                                  IntPtr gc,
                                  IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr GDCap(IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDSize(out double left, out double right, out double bottom, out double top, IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double GDStrWidth([MarshalAs(UnmanagedType.LPStr)] string str, IntPtr gc, IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDText(double x,
                                double y,
                                [MarshalAs(UnmanagedType.LPStr)] string str,
                                double rot,
                                double hadj,
                                IntPtr gc,
                                IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDOnExit(IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr GDGetEvent(IntPtr sexp, [MarshalAs(UnmanagedType.LPStr)] string e);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool GDNewFrameConfirm(IntPtr dd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDEventHelper(IntPtr dd, int code);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GDHoldFlush(IntPtr dd, int level);
}
