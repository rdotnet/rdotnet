using System;
using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter.Graphics
{
    public interface IGraphicsContext
    {
        Color Foreground { get; }
        Color Background { get; }
        double Gamma { get; }
        double LineWidth { get; }
        LineType LineType { get; }
        LineEnd LineEnd { get; }
        LineJoin LineJoin { get; }
        double LineMitre { get; }
        double CharacterExpansion { get; }
        double FontSizeInPoints { get; }
        double LineHeight { get; }
        FontFace FontFace { get; }
        string FontFamily { get; }
    }

    public class GraphicsContext : SafeHandle, IGraphicsContext
    {
        public GraphicsContext(IntPtr pointer)
            : base(IntPtr.Zero, true)
        {
            SetHandle(pointer);
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        public Color Foreground
        {
            get { return GetContext().col; }
        }

        public Color Background
        {
            get { return GetContext().fill; }
        }

        public double Gamma
        {
            get { return GetContext().gamma; }
        }

        public double LineWidth
        {
            get { return GetContext().lwd; }
        }

        public LineType LineType
        {
            get { return GetContext().lty; }
        }

        public LineEnd LineEnd
        {
            get { return GetContext().lend; }
        }

        public LineJoin LineJoin
        {
            get { return GetContext().ljoin; }
        }

        public double LineMitre
        {
            get { return GetContext().lmitre; }
        }

        public double CharacterExpansion
        {
            get { return GetContext().cex; }
        }

        public double FontSizeInPoints
        {
            get { return GetContext().ps; }
        }

        public double LineHeight
        {
            get { return GetContext().lineheight; }
        }

        public FontFace FontFace
        {
            get { return GetContext().fontface; }
        }

        public string FontFamily
        {
            get { return GetContext().fontfamily; }
        }

        protected override bool ReleaseHandle()
        {
            return true;
        }

        private GEcontext GetContext()
        {
            return (GEcontext)Marshal.PtrToStructure(handle, typeof (GEcontext));
        }
    }
}
