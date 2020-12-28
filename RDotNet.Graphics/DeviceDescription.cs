using RDotNet.Graphics.Internals;
using System;
using System.Runtime.InteropServices;

namespace RDotNet.Graphics
{
    public class DeviceDescription : SafeHandle
    {
        public DeviceDescription()
            : base(IntPtr.Zero, true)
        {
            var pointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DevDesc)));
            Marshal.StructureToPtr(new DevDesc(), pointer, true);
            SetHandle(pointer);
            SetDefaultParameter();
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        public Rectangle Bounds
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
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
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
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
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.xCharOffset;
            }
            set { WriteDouble("xCharOffset", value); }
        }

        public double CharOffsetY
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.yCharOffset;
            }
            set { WriteDouble("yCharOffset", value); }
        }

        public double LineBiasY
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.yLineBias;
            }
            set { WriteDouble("yLineBias", value); }
        }

        public double InchesPerRasterX
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.ipr[0];
            }
            set { WriteDoubleArray("ipr", 0, value); }
        }

        public double InchesPerRasterY
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.ipr[1];
            }
            set { WriteDoubleArray("ipr", 1, value); }
        }

        public double CharacterSizeInRasterX
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.cra[0];
            }
            set { WriteDoubleArray("cra", 0, value); }
        }

        public double CharacterSizeInRasterY
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.cra[1];
            }
            set { WriteDoubleArray("cra", 1, value); }
        }

        public double Gamma
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.gamma;
            }
            set { WriteDouble("gamma", value); }
        }

        public bool IsGammaModifiable
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.canChangeGamma;
            }
            set { WriteBoolean("canChangeGamma", value); }
        }

        public bool IsClippable
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.canClip;
            }
            set { WriteBoolean("canClip", value); }
        }

        public Adjustment Adjustment
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.canHAdj;
            }
            set { WriteInt32Enum("canHAdj", value); }
        }

        public double StartFontSize
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.startps;
            }
            set { WriteDouble("startps", value); }
        }

        public Color StartForeground
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.startcol;
            }
            set { WriteColor("startcol", value); }
        }

        public Color StartBackground
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.startfill;
            }
            set { WriteColor("startfill", value); }
        }

        public LineType StartLineType
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.startlty;
            }
            set { WriteInt32Enum("startlty", value); }
        }

        public int StartFont
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.startfont;
            }
            set { WriteInt32("startfont", value); }
        }

        public double StartGamma
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.startgamma;
            }
            set { WriteDouble("startgamma", value); }
        }

        public bool DisplayListOn
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.displayListOn;
            }
            set { WriteBoolean("displayListOn", value); }
        }

        public bool IsTextRotatedInContour
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.useRotatedTextInContour;
            }
            set { WriteBoolean("useRotatedTextInContour", value); }
        }

        protected IntPtr DeviceSpecific
        {
            get
            {
                var dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
                return dd.deviceSpecific;
            }
            set { WriteIntPtr("deviceSpecific", value); }
        }

        private void SetDefaultParameter()
        {
            StartForeground = Colors.Black;
            StartBackground = Colors.White;
            StartLineType = LineType.Solid;
            StartFont = 1;
            StartFontSize = 14.0;
            StartGamma = 1.0;
            CharOffsetX = 0.4900;
            CharOffsetY = 0.3333;
            CharacterSizeInRasterX = StartFontSize * 0.9;
            CharacterSizeInRasterY = StartFontSize * 1.2;
            InchesPerRasterX = 1.0 / 72.0;
            InchesPerRasterY = 1.0 / 72.0;
            LineBiasY = 0.20;
            IsClippable = true;
            Adjustment = Adjustment.None;
            IsGammaModifiable = false;
            DisplayListOn = false;
        }

        private void WriteDouble(string fieldName, double value)
        {
            var bytes = BitConverter.GetBytes(value);
            var offset = Marshal.OffsetOf(typeof(DevDesc), fieldName).ToInt32();
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length);
        }

        private void WriteDoubleArray(string fieldName, int index, double value)
        {
            var bytes = BitConverter.GetBytes(value);
            var offset = Marshal.OffsetOf(typeof(DevDesc), fieldName).ToInt32() + sizeof(double) * index;
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length);
        }

        private void WriteBoolean(string fieldName, bool value)
        {
            var bytes = BitConverter.GetBytes(Convert.ToInt32(value));
            var offset = Marshal.OffsetOf(typeof(DevDesc), fieldName).ToInt32();
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length);
        }

        private void WriteInt32Enum(string fieldName, Enum value)
        {
            var bytes = BitConverter.GetBytes(Convert.ToInt32(value));
            var offset = Marshal.OffsetOf(typeof(DevDesc), fieldName).ToInt32();
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length);
        }

        private void WriteColor(string fieldName, Color value)
        {
            var bytes = BitConverter.GetBytes(value.GetHashCode());
            var offset = Marshal.OffsetOf(typeof(DevDesc), fieldName).ToInt32();
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length);
        }

        private void WriteInt32(string fieldName, int value)
        {
            var offset = Marshal.OffsetOf(typeof(DevDesc), fieldName).ToInt32();
            Marshal.WriteInt32(handle, offset, value);
        }

        private void WriteIntPtr(string fieldName, IntPtr value)
        {
            var offset = Marshal.OffsetOf(typeof(DevDesc), fieldName).ToInt32();
            Marshal.WriteIntPtr(handle, offset, value);
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(handle);
            return true;
        }

        internal void SetMethod(string fieldName, Delegate d)
        {
            var pointer = Marshal.GetFunctionPointerForDelegate(d);
            WriteIntPtr(fieldName, pointer);
        }
    }
}