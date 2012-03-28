using System;
using System.Runtime.InteropServices;
using RDotNet.Graphics.Internals;

namespace RDotNet.Graphics
{
	public class DeviceDescription : SafeHandle
	{
		public DeviceDescription()
			: base(IntPtr.Zero, true)
		{
			IntPtr pointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DevDesc)));
			Marshal.StructureToPtr(new DevDesc(), pointer, true);
			SetHandle(pointer);
		}

		internal DeviceDescription(IntPtr pointer)
			: base(IntPtr.Zero, true)
		{
			SetHandle(pointer);
		}

		public override bool IsInvalid
		{
			get { return handle == IntPtr.Zero; }
		}

		public Rectangle Bounds
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return new Rectangle(dd.left, dd.bottom, dd.right - dd.left, dd.top - dd.bottom);
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.left = value.Left;
				dd.right = value.Right;
				dd.bottom = value.Bottom;
				dd.top = value.Top;
				Copy(ref dd);
			}
		}

		public Rectangle ClipBounds
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return new Rectangle(dd.clipLeft, dd.clipBottom, dd.clipRight - dd.clipLeft, dd.clipTop - dd.clipBottom);
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.clipLeft = value.Left;
				dd.clipRight = value.Right;
				dd.clipBottom = value.Bottom;
				dd.clipTop = value.Top;
				Copy(ref dd);
			}
		}

		public double CharOffsetX
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.xCharOffset;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.xCharOffset = value;
				Copy(ref dd);
			}
		}

		public double CharOffsetY
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.yCharOffset;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.yCharOffset = value;
				Copy(ref dd);
			}
		}

		public double LineBiasY
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.yLineBias;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.yLineBias = value;
				Copy(ref dd);
			}
		}

		public double InchesPerRasterX
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.ipr[0];
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.ipr[0] = value;
				Copy(ref dd);
			}
		}

		public double InchesPerRasterY
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.ipr[1];
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.ipr[1] = value;
				Copy(ref dd);
			}
		}

		public double CharacterSizeInRasterX
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.cra[0];
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.cra[0] = value;
				Copy(ref dd);
			}
		}

		public double CharacterSizeInRasterY
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.cra[1];
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.cra[1] = value;
				Copy(ref dd);
			}
		}

		public double Gamma
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.gamma;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.gamma = value;
				Copy(ref dd);
			}
		}

		public bool IsGammaModifiable
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.canChangeGamma;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.canChangeGamma = value;
				Copy(ref dd);
			}
		}

		public bool IsClippable
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.canClip;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.canClip = value;
				Copy(ref dd);
			}
		}

		public Adjustment Adjustment
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.canHAdj;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.canHAdj = value;
				Copy(ref dd);
			}
		}

		public double StartFontSize
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.startps;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.startps = value;
				Copy(ref dd);
			}
		}

		public Color StartForeground
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.startcol;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.startcol = value;
				Copy(ref dd);
			}
		}

		public Color StartBackground
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.startfill;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.startfill = value;
				Copy(ref dd);
			}
		}

		public int StartFont
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.startfont;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.startfont = value;
				Copy(ref dd);
			}
		}

		public double StartGamma
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.startgamma;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.startgamma = value;
				Copy(ref dd);
			}
		}

		public bool DisplayListOn
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.displayListOn;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.displayListOn = value;
				Copy(ref dd);
			}
		}

		public bool IsTextRotatedInContour
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.useRotatedTextInContour;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.useRotatedTextInContour = value;
				Copy(ref dd);
			}
		}

		protected IntPtr DeviceSpecific
		{
			get
			{
				DevDesc dd;
				GetDescription(out dd);
				return dd.deviceSpecific;
			}
			set
			{
				DevDesc dd;
				GetDescription(out dd);
				dd.deviceSpecific = value;
				Copy(ref dd);
			}
		}

		internal void Copy(ref DevDesc dd)
		{
			Marshal.StructureToPtr(dd, handle, false);
		}

		protected override bool ReleaseHandle()
		{
			Marshal.FreeHGlobal(handle);
			return true;
		}

		internal void GetDescription(out DevDesc dd)
		{
			dd = (DevDesc)Marshal.PtrToStructure(handle, typeof(DevDesc));
		}

		protected override void Dispose(bool disposing)
		{
			SetHandleAsInvalid();
			base.Dispose(disposing);
		}
	}
}
