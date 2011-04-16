using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet.Graphics.Internals;
using System.Runtime.InteropServices;

namespace RDotNet.Graphics
{
	public class GraphicsContext : SafeHandle
	{
		public override bool IsInvalid
		{
			get { return this.handle == IntPtr.Zero; }
		}

		public Color Foreground { get { return GetContext().col; } }
		public Color Background { get { return GetContext().fill; } }
		public double Gamma { get { return GetContext().gamma; } }
		public double LineWidth { get { return GetContext().lwd; } }
		public LineType LineType { get { return GetContext().lty; } }
		public LineEnd LineEnd { get { return GetContext().lend; } }
		public LineJoin LineJoin { get { return GetContext().ljoin; } }
		public double LineMitre { get { return GetContext().lmitre; } }
		public double CharacterExpansion { get { return GetContext().cex; } }
		public double FontSizeInPoints { get { return GetContext().ps; } }
		public double LineHeight { get { return GetContext().lineheight; } }
		public FontFace FontFace { get { return GetContext().fontface; } }
		public string FontFamily { get { return GetContext().fontfamily; } }

		internal GraphicsContext(IntPtr pointer)
			: base(IntPtr.Zero, true)
		{
			SetHandle(pointer);
		}

		protected override bool ReleaseHandle()
		{
			return true;
		}

		private GEcontext GetContext()
		{
			return (GEcontext)Marshal.PtrToStructure(this.handle, typeof(GEcontext));
		}
	}
}
