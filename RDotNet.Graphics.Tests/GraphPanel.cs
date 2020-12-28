using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace RDotNet.Graphics
{
    public partial class GraphPanel : Panel, IGraphicsDevice
    {
        public GraphPanel()
        {
            InitializeComponent();
        }

        #region IGraphicsDevice Members

        //public int Version
        //{
        //   get
        //   {
        //      var engine = REngine.GetInstanceFromID("RDotNetTest");
        //      var version = new Version(engine.DllVersion);
        //      if (version < new Version(2, 14))  // maybe version 8 (R 2.12) works
        //      {
        //         throw new NotSupportedException();
        //      }
        //      else if (version < new Version(3, 0))
        //      {
        //         return 9;
        //      }
        //      return 10;  // cannot support future versions
        //   }
        //}

        string IGraphicsDevice.Name
        {
            get { return "WinGraph"; }
        }

        Rectangle IGraphicsDevice.GetSize(GraphicsContext context, DeviceDescription description)
        {
            return description.ClipBounds;
        }

        bool IGraphicsDevice.ConfirmNewFrame(DeviceDescription description)
        {
            return true;
        }

        void IGraphicsDevice.DrawCircle(Point center, double radius, GraphicsContext context, DeviceDescription description)
        {
            var diameter = (float)(2 * radius);
            var point = ConvertPointF(center);
            var color = ConvertColor(context.Foreground);
            using (var g = CreateGraphics())
            using (var pen = new Pen(color))
            {
                g.DrawEllipse(pen, (float)(point.X - radius), (float)(point.Y - radius), diameter, diameter);
            }
        }

        void IGraphicsDevice.Clip(Rectangle rectangle, DeviceDescription description)
        { }

        Point? IGraphicsDevice.GetLocation(DeviceDescription description)
        {
            throw new NotImplementedException();
        }

        void IGraphicsDevice.DrawLine(Point source, Point destination, GraphicsContext context, DeviceDescription description)
        {
            var color = ConvertColor(context.Foreground);
            var p0 = ConvertPointF(source);
            var p1 = ConvertPointF(destination);
            using (var g = CreateGraphics())
            using (var pen = new Pen(color))
            {
                g.DrawLine(pen, p0, p1);
            }
        }

        public MetricsInfo GetMetricInfo(int character, GraphicsContext context, DeviceDescription description)
        {
            return GetTextMetrics(character.ToString(CultureInfo.InvariantCulture), context, description);
        }

        private MetricsInfo GetTextMetrics(string s, GraphicsContext context, DeviceDescription description)
        {
            var style = GetStyle(context.FontFace);
            var family = Font.FontFamily;
            var conversion = Font.SizeInPoints / family.GetEmHeight(style);
            using (var g = CreateGraphics())
            {
                return new MetricsInfo {
                    Ascent = family.GetCellAscent(style) * conversion,
                    Descent = family.GetCellDescent(style) * conversion,
                    Width = g.MeasureString(s, Font).Width
                };
            }
        }

        void IGraphicsDevice.DrawPolygon(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description)
        {
            var color = ConvertColor(context.Foreground);
            using (var g = CreateGraphics())
            using (var pen = new Pen(color))
            {
                g.DrawPolygon(pen, points.Select(ConvertPointF).ToArray());
            }
        }

        void IGraphicsDevice.DrawPolyline(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description)
        {
            var color = ConvertColor(context.Foreground);
            using (var g = CreateGraphics())
            using (var pen = new Pen(color))
            {
                g.DrawLines(pen, points.Select(ConvertPointF).ToArray());
            }
        }

        void IGraphicsDevice.DrawRectangle(Rectangle rectangle, GraphicsContext context, DeviceDescription description)
        {
            var color = ConvertColor(context.Foreground);
            var rect = ConvertRectangle(rectangle);
            using (var g = CreateGraphics())
            using (var pen = new Pen(color))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        void IGraphicsDevice.DrawPath(IEnumerable<IEnumerable<Point>> points, bool winding, GraphicsContext context, DeviceDescription description)
        {
            var path = new GraphicsPath();
            foreach (var vertices in points)
            {
                path.StartFigure();
                path.AddLines(vertices.Select(ConvertPointF).ToArray());
            }
            var color = ConvertColor(context.Foreground);
            using (var g = CreateGraphics())
            using (var pen = new Pen(color))
            {
                g.DrawPath(pen, path);
            }
        }

        void IGraphicsDevice.DrawRaster(Raster raster, Rectangle destination, double rotation, bool interpolated, GraphicsContext context, DeviceDescription description)
        {
            throw new NotImplementedException();
        }

        Raster IGraphicsDevice.Capture(DeviceDescription description)
        {
            throw new NotImplementedException();
        }

        double IGraphicsDevice.MeasureWidth(string s, GraphicsContext context, DeviceDescription description)
        {
            var metrics = GetTextMetrics(s, context, description);
            return metrics.Width;
        }

        void IGraphicsDevice.DrawText(string s, Point location, double rotation, double adjustment, GraphicsContext context, DeviceDescription description)
        {
            var color = ConvertColor(context.Foreground);
            using (var g = CreateGraphics())
            using (var brush = new SolidBrush(color))
            {
                g.DrawString(s, Font, brush, ConvertPointF(location));
            }
        }

        public void OnActivated(DeviceDescription description)
        {
            var bounds = Bounds;
            var rectangle = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            description.Bounds = rectangle;
            description.ClipBounds = rectangle;
        }

        public void OnDeactivated(DeviceDescription description)
        { }

        public void OnNewPageRequested(GraphicsContext context, DeviceDescription description)
        {
            var color = ConvertColor(description.StartBackground);
            using (var g = CreateGraphics())
            {
                g.Clear(color);
            }
        }

        public Rectangle OnResized(DeviceDescription description)
        {
            throw new NotImplementedException();
        }

        public void OnClosed(DeviceDescription description)
        { }

        public void OnDrawStarted(DeviceDescription description)
        { }

        public void OnDrawStopped(DeviceDescription description)
        { }

        #endregion IGraphicsDevice Members

        private PointF ConvertPointF(Point point)
        {
            return new PointF((float)point.X, (float)(Height - point.Y));
        }

        private static FontStyle GetStyle(FontFace face)
        {
            switch (face)
            {
                case FontFace.Bold:
                    return FontStyle.Bold;

                case FontFace.Italic:
                    return FontStyle.Italic;

                case FontFace.BoldItalic:
                    return FontStyle.Bold | FontStyle.Italic;

                default:
                    return FontStyle.Regular;
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        private System.Drawing.Color ConvertColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

        private System.Drawing.Rectangle ConvertRectangle(Rectangle rectangle)
        {
            return System.Drawing.Rectangle.FromLTRB((int)rectangle.Left, (int)(Height - rectangle.Top), (int)rectangle.Right, (int)(Height - rectangle.Bottom));
        }
    }
}