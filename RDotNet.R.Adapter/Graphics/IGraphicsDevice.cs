using System.Collections.Generic;
using System.Drawing;

namespace RDotNet.R.Adapter.Graphics
{
    public interface IGraphicsDevice
    {
        string Name { get; }

        void Activated(DeviceDescription description);
        void Deactivated(DeviceDescription description);
        Raster Capture(DeviceDescription description);
        void Clear();
        void SetClip(Rectangle rectangle, DeviceDescription description);
        void Closed(DeviceDescription description);
        bool ConfirmNewFrame(DeviceDescription description);
        void DrawCircle(Point center, double radius, GraphicsContext context, DeviceDescription description);
        void DrawLine(Point source, Point destination, GraphicsContext context, DeviceDescription description);
        void DrawPolygon(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description);
        void DrawPolyline(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description);
        void DrawRectangle(Rectangle rectangle, GraphicsContext context, DeviceDescription description);
        void DrawPath(IEnumerable<IEnumerable<Point>> points,
                      bool winding,
                      GraphicsContext context,
                      DeviceDescription description);
        void DrawRaster(Bitmap raster,
                        Rectangle destination,
                        double rotation,
                        bool interpolated,
                        GraphicsContext context,
                        DeviceDescription description);
        void DrawStopped(DeviceDescription description);
        void DrawStarted(DeviceDescription description);
        void DrawText(string s,
                      Point location,
                      double rotation,
                      double adjustment,
                      GraphicsContext context,
                      DeviceDescription description);

        Point? GetLocation(DeviceDescription description);
        MetricsInfo GetMetricInfo(int character, GraphicsContext context, DeviceDescription description);
        List<string> GetPlots(); 
        double MeasureWidth(string s, GraphicsContext context, DeviceDescription description);
        void NewPageRequested(GraphicsContext context, DeviceDescription description);
        Rectangle Resized(DeviceDescription description);
        void SetDeviceParameters(DeviceDescription description);
    }
}
