using System.Collections.Generic;

namespace RDotNet.Graphics
{
	public interface IGraphicsDevice
	{
		int Version { get; }
		string Name { get; }

		void OnActivated(DeviceDescription description);
		void OnDeactivated(DeviceDescription description);
		void OnNewPageRequested(GraphicsContext context, DeviceDescription description);
		Rectangle OnResized(DeviceDescription description);
		void OnClosed(DeviceDescription description);
		void OnDrawStarted(DeviceDescription description);
		void OnDrawStopped(DeviceDescription description);
		Rectangle GetSize(GraphicsContext context, DeviceDescription description);
		bool ConfirmNewFrame(DeviceDescription description);
		void DrawCircle(Point center, double radius, GraphicsContext context, DeviceDescription description);
		void Clip(Rectangle rectangle, DeviceDescription description);
		Point? GetLocation(DeviceDescription description);
		void DrawLine(Point source, Point destination, GraphicsContext context, DeviceDescription description);
		MetricsInfo GetMetricInfo(int character, GraphicsContext context, DeviceDescription description);
		void DrawPolygon(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description);
		void DrawPolyline(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description);
		void DrawRectangle(Rectangle rectangle, GraphicsContext context, DeviceDescription description);
		void DrawPath(IEnumerable<IEnumerable<Point>> points, bool winding, GraphicsContext context, DeviceDescription description);
		void DrawRaster(Raster raster, Rectangle destination, double rotation, bool interpolated, GraphicsContext context, DeviceDescription description);
		Raster Capture(DeviceDescription description);
		double MeasureWidth(string s, GraphicsContext context, DeviceDescription description);
		void DrawText(string s, Point location, double rotation, double adjustment, GraphicsContext context, DeviceDescription description);
	}
}
