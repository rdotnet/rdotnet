using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter.Graphics
{
    public class DeviceDescription : SafeHandle
    {
        private readonly List<GCHandle> _handles = new List<GCHandle>(22);
        private GDActivate _activate;
        private GDCap _cap;
        private GDCircle _circle;
        private GDClip _clip;
        private GDClose _closeDevice;
        private GDDeactivate _deactivate;
        private GDEventHelper _eventHelper;
        private GDGetEvent _getEvent;
        private GDLine _line;
        private GDLocator _locator;
        private GDMetricInfo _metricInfo;
        private GDMode _mode;
        private GDNewPage _newPage;
        private GDPath _path;
        private GDPolygon _polygon;
        private GDPolyline _polyline;
        private GDRaster _raster;
        private GDRect _rect;
        private GDSize _size;
        private GDStrWidth _strWidth;
        private GDText _text;
        private GDNewFrameConfirm _newFrameConfirm;

        public DeviceDescription()
            : base(IntPtr.Zero, true)
        {   
            var pointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (DevDesc)));
            Marshal.StructureToPtr(new DevDesc(), pointer, true);
            SetHandle(pointer);

            StartForeground = Colors.Black;
            StartBackground = Colors.White;
            StartLineType = LineType.Solid;
            StartFont = 1;
            StartFontSize = 12.0;
            StartGamma = 1.0;
            CharOffsetX = 0.4900;
            CharOffsetY = 0.3333;
            CharacterSizeInRasterX = 9.0;
            CharacterSizeInRasterY = 12.0;
            InchesPerRasterX = 1.0/72.0;
            InchesPerRasterY = 1.0/72.0;
            LineBiasY = 0.20;
            IsClippable = true;
            Adjustment = Adjustment.None;
            IsGammaModifiable = false;
            DisplayListOn = false;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        public Rectangle Bounds
        {
            get
            {
                Debug.WriteLine("Bounds");
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return new Rectangle(dd.left, dd.bottom, dd.right - dd.left, dd.top - dd.bottom);
            }
            set
            {
                WriteDouble("left", value.Left);
                WriteDouble("right", value.Right);
                WriteDouble("bottom", value.Bottom);
                WriteDouble("top", value.Top);
            }
        }

        public Rectangle ClipBounds
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return new Rectangle(dd.clipLeft, dd.clipBottom, dd.clipRight - dd.clipLeft, dd.clipTop - dd.clipBottom);
            }
            set
            {
                WriteDouble("clipLeft", value.Left);
                WriteDouble("clipRight", value.Right);
                WriteDouble("clipBottom", value.Bottom);
                WriteDouble("clipTop", value.Top);
            }
        }

        public double CharOffsetX
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.xCharOffset;
            }
            set { WriteDouble("xCharOffset", value); }
        }

        public double CharOffsetY
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.yCharOffset;
            }
            set { WriteDouble("yCharOffset", value); }
        }

        public double LineBiasY
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.yLineBias;
            }
            set { WriteDouble("yLineBias", value); }
        }

        public double InchesPerRasterX
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.ipr[0];
            }
            set { WriteDoubleArray("ipr", 0, value); }
        }

        public double InchesPerRasterY
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.ipr[1];
            }
            set { WriteDoubleArray("ipr", 1, value); }
        }

        public double CharacterSizeInRasterX
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.cra[0];
            }
            set { WriteDoubleArray("cra", 0, value); }
        }

        public double CharacterSizeInRasterY
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.cra[1];
            }
            set { WriteDoubleArray("cra", 1, value); }
        }

        public double Gamma
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.gamma;
            }
            set { WriteDouble("gamma", value); }
        }

        public bool IsGammaModifiable
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.canChangeGamma;
            }
            set { WriteBoolean("canChangeGamma", value); }
        }

        public bool IsClippable
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.canClip;
            }
            set { WriteBoolean("canClip", value); }
        }

        public Adjustment Adjustment
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.canHAdj;
            }
            set { WriteInt32Enum("canHAdj", value); }
        }

        public double StartFontSize
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.startps;
            }
            set { WriteDouble("startps", value); }
        }

        public Color StartForeground
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.startcol;
            }
            set { WriteColor("startcol", value); }
        }

        public Color StartBackground
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.startfill;
            }
            set { WriteColor("startfill", value); }
        }

        public LineType StartLineType
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.startlty;
            }
            set { WriteInt32Enum("startlty", value); }
        }

        public int StartFont
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.startfont;
            }
            set { WriteInt32("startfont", value); }
        }

        public YesOrNo HaveTransparency
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.haveTransparency;
            }
            set { WriteInt32("haveTransparency", (int)value); }
        }

        public YesNoOrMostly HaveTransparentBackground
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.haveTransparentBg;
            }
            set { WriteInt32("haveTransparentBg", (int)value); }
        }

        public YesNoOrMostly HaveRaster
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.haveRaster;
            }
            set { WriteInt32("haveRaster", (int)value); }
        }

        public YesOrNo HaveCapture
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.haveCapture;
            }
            set { WriteInt32("haveCapture", (int)value); }
        }

        public YesOrNo HaveLocator
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.haveLocator;
            }
            set { WriteInt32("haveLocator", (int)value); }
        }
        
        public double StartGamma
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.startgamma;
            }
            set { WriteDouble("startgamma", value); }
        }

        public bool DisplayListOn
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.displayListOn;
            }
            set { WriteBoolean("displayListOn", value); }
        }

        public bool IsTextRotatedInContour
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.useRotatedTextInContour;
            }
            set { WriteBoolean("useRotatedTextInContour", value); }
        }

        protected IntPtr DeviceSpecific
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof (DevDesc));
                return dd.deviceSpecific;
            }
            set { WriteIntPtr("deviceSpecific", value); }
        }

        public GDActivate Activate
        {
            get { return _activate; }
            set
            {
                _activate = value;
                WriteIntPtr("activate", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDCap Cap
        {
            get { return _cap; }
            set
            {
                _cap = value;
                WriteIntPtr("cap", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDCircle Circle
        {
            get { return _circle; }
            set
            {
                _circle = value;
                WriteIntPtr("circle", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDClip Clip
        {
            get { return _clip; }
            set
            {
                _clip = value;
                WriteIntPtr("clip", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDClose CloseDevice
        {
            get { return _closeDevice; }
            set
            {
                _closeDevice = value;
                WriteIntPtr("close", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDDeactivate Deactivate
        {
            get { return _deactivate; }
            set
            {
                _deactivate = value;
                WriteIntPtr("deactivate", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDEventHelper EventHelper
        {
            get { return _eventHelper; }
            set
            {
                _eventHelper = value;
                WriteIntPtr("eventHelper", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDGetEvent GetEvent
        {
            get { return _getEvent; }
            set
            {
                _getEvent = value;
                WriteIntPtr("getEvent", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDLine Line
        {
            get { return _line; }
            set
            {
                _line = value;
                WriteIntPtr("line", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDLocator Locator
        {
            get { return _locator; }
            set
            {
                _locator = value;
                WriteIntPtr("locator", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDMetricInfo MetricInfo
        {
            get { return _metricInfo; }
            set
            {
                _metricInfo = value;
                WriteIntPtr("metricInfo", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                WriteIntPtr("mode", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDNewPage NewPage
        {
            get { return _newPage; }
            set
            {
                _newPage = value;
                WriteIntPtr("newPage", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDPath Path
        {
            get { return _path; }
            set
            {
                _path = value;
                WriteIntPtr("path", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDPolygon Polygon
        {
            get { return _polygon; }
            set
            {
                _polygon = value;
                WriteIntPtr("polygon", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDPolyline Polyline
        {
            get { return _polyline; }
            set
            {
                _polyline = value;
                WriteIntPtr("polyline", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDRaster Raster
        {
            get { return _raster; }
            set
            {
                _raster = value;
                WriteIntPtr("raster", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDRect Rect
        {
            get { return _rect; }
            set
            {
                _rect = value;
                WriteIntPtr("rect", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDSize Size
        {
            get { return _size; }
            set
            {
                _size = value;
                WriteIntPtr("size", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDStrWidth StrWidth
        {
            get { return _strWidth; }
            set
            {
                _strWidth = value;
                WriteIntPtr("strWidth", Marshal.GetFunctionPointerForDelegate(value));
                WriteIntPtr("strWidthUTF8", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDText Text
        {
            get { return _text; }
            set
            {
                _text = value;
                WriteIntPtr("text", Marshal.GetFunctionPointerForDelegate(value));
                WriteIntPtr("textUTF8", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        public GDNewFrameConfirm NewFrameConfirm
        {
            get { return _newFrameConfirm; }
            set
            {
                _newFrameConfirm = value;
                WriteIntPtr("newFrameConfirm", Marshal.GetFunctionPointerForDelegate(value));
            }
        }

        private void WriteDouble(string fieldName, double value)
        {
            var bytes = BitConverter.GetBytes(value);
            var offset = Marshal.OffsetOf(typeof (DevDesc), fieldName).ToInt32();
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length);
        }

        private void WriteDoubleArray(string fieldName, int index, double value)
        {
            var bytes = BitConverter.GetBytes(value);
            var offset = Marshal.OffsetOf(typeof (DevDesc), fieldName).ToInt32() + sizeof (double)*index;
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length);
        }

        private void WriteBoolean(string fieldName, bool value)
        {
            var bytes = BitConverter.GetBytes(Convert.ToInt32(value));
            var offset = Marshal.OffsetOf(typeof (DevDesc), fieldName).ToInt32();
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length);
        }

        private void WriteInt32Enum(string fieldName, Enum value)
        {
            var bytes = BitConverter.GetBytes(Convert.ToInt32(value));
            var offset = Marshal.OffsetOf(typeof (DevDesc), fieldName).ToInt32();
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length);
        }

        private void WriteColor(string fieldName, Color value)
        {
            var bytes = BitConverter.GetBytes(value.GetHashCode());
            var offset = Marshal.OffsetOf(typeof (DevDesc), fieldName).ToInt32();
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length);
        }

        private void WriteInt32(string fieldName, int value)
        {
            var offset = Marshal.OffsetOf(typeof (DevDesc), fieldName).ToInt32();
            Marshal.WriteInt32(handle, offset, value);
        }

        private void WriteIntPtr(string fieldName, IntPtr value)
        {
            var offset = Marshal.OffsetOf(typeof (DevDesc), fieldName).ToInt32();
            Marshal.WriteIntPtr(handle, offset, value);
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(handle);
            return true;
        }
    }
}
