using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter.Graphics
{
    public class PlotException : Exception
    {
        public PlotException(Exception inner)
            : base("Critical error while plotting", inner)
        { }
    }

    public class GraphicsDeviceManager : IDisposable
    {
        private readonly RInstance _rInstance;
        private readonly List<GraphicsDeviceContext>  _devices = new List<GraphicsDeviceContext>();

        public GraphicsDeviceManager(RInstance rInstance)
        {
            if (rInstance == null) throw new ArgumentNullException("rInstance");

            _rInstance = rInstance;
        }

        public List<IGraphicsDevice> Devices { get { return _devices.Select(s => s.GraphicsDevice).ToList(); } } 

        public void AddDevice(IGraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null) throw new ArgumentNullException("graphicsDevice");

            var dc = _devices.SingleOrDefault(s => string.Equals(s.GraphicsDevice.Name, graphicsDevice.Name));
            if (dc != null) return;

            ConfigureGraphicsDevice(graphicsDevice);
        }

        private void ConfigureGraphicsDevice(IGraphicsDevice graphicsDevice)
        {
            CheckDeviceAvailable();

            var dc = new GraphicsDeviceContext
            {
                GraphicsDevice = graphicsDevice,
                DeviceDescription = InitializeDeviceDescription(),
            };
            graphicsDevice.SetDeviceParameters(dc.DeviceDescription);

            //Our device must be cached before it is passed to R because R will try to activate it.
            _devices.Add(dc);

            using (InterruptManager.SuspendInterrupts(_rInstance))
            {
                dc.GEDeviceDescriptionPtr = CreateGraphicsEngineDeviceDescription(dc.DeviceDescription);
                AddDevice(dc);
            }
        }

        private DeviceDescription InitializeDeviceDescription()
        {
            var deviceDescription = new DeviceDescription
            {
                Activate = Activate,
                Cap = Capture,
                Circle = DrawCircle,
                Clip = Clip,
                CloseDevice = Close,
                Deactivate = Deactivate,
                Line = DrawLine,
                Locator = GetLocation,
                MetricInfo = GetMetricInfo,
                Mode = ChangeMode,
                NewPage = NewPage,
                Path = DrawPath,
                Polygon = DrawPolygon,
                Polyline = DrawPolyline,
                Raster = DrawRaster,
                Rect = DrawRectangle,
                Size = Resize,
                StrWidth = MeasureWidth,
                Text = DrawText,
                NewFrameConfirm = ConfirmNewFrame,
                GetEvent = GetEvent,
                EventHelper = EventHelper
            };

            return deviceDescription;
        }

        private IntPtr CreateGraphicsEngineDeviceDescription(DeviceDescription dd)
        {
            var result = _rInstance.GetFunction<GEcreateDevDesc>()(dd.DangerousGetHandle());
            return result;
        }

        private void CheckDeviceAvailable()
        {
            _rInstance.GetFunction<R_CheckDeviceAvailable>()(); //NOTE: [RMEL] Does this fault or throw somehow?
        }

        private void AddDevice(GraphicsDeviceContext dc)
        {
            _rInstance.GetFunction<GEaddDevice2>()(dc.GEDeviceDescriptionPtr, dc.GraphicsDevice.Name);
        }

        private void Activate(IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                context.GraphicsDevice.Activated(context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void Close(IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                context.GraphicsDevice.Closed(context.DeviceDescription);
                CleanupDeviceContext(context);
                _devices.Remove(context);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void Deactivate(IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                context.GraphicsDevice.Deactivated(context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void NewPage(IntPtr pGraphicsContext, IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                context.GraphicsDevice.NewPageRequested(new GraphicsContext(pGraphicsContext), context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void Resize(out double left, out double right, out double bottom, out double top, IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                var rectangle = context.GraphicsDevice.Resized(context.DeviceDescription);
                left = rectangle.Left;
                right = rectangle.Right;
                bottom = rectangle.Bottom;
                top = rectangle.Top;
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private bool ConfirmNewFrame(IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                return context.GraphicsDevice.ConfirmNewFrame(context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void ChangeMode(int mode, IntPtr pDevDesc)
        {
            try
            {

                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                if (mode == 0)
                {
                    context.GraphicsDevice.DrawStopped(context.DeviceDescription);
                }
                else if (mode == 1)
                {
                    context.GraphicsDevice.DrawStarted(context.DeviceDescription);
                }
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void DrawCircle(double x, double y, double r, IntPtr pGraphicsContext, IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                var center = new Point(x, y);
                context.GraphicsDevice.DrawCircle(center,
                                                    r,
                                                    new GraphicsContext(pGraphicsContext),
                                                    context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void Clip(double x0, double x1, double y0, double y1, IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                var rectangle = new Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x1 - x0), Math.Abs(y1 - y0));
                context.GraphicsDevice.SetClip(rectangle, context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private bool GetLocation(out double x, out double y, IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                var location = context.GraphicsDevice.GetLocation(context.DeviceDescription);
                if (!location.HasValue)
                {
                    x = default(double);
                    y = default(double);
                    return false;
                }

                var p = location.Value;
                x = p.X;
                y = p.Y;
                return true;
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void DrawLine(double x1, double y1, double x2, double y2, IntPtr pGraphicsContext, IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                var source = new Point(x1, y1);
                var destination = new Point(x2, y2);
                context.GraphicsDevice.DrawLine(source,
                                                destination,
                                                new GraphicsContext(pGraphicsContext),
                                                context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void GetMetricInfo(int character,
                                   IntPtr pGraphicsContext,
                                   out double ascent,
                                   out double descent,
                                   out double width,
                                   IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                var metric = context.GraphicsDevice.GetMetricInfo(character,
                                                                   new GraphicsContext(pGraphicsContext),
                                                                   context.DeviceDescription);
                ascent = metric.Ascent;
                descent = metric.Descent;
                width = metric.Width;
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void DrawPolygon(int numPoints, IntPtr x, IntPtr y, IntPtr pGraphicsContext, IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                var points = GetPoints(numPoints, x, y);
                context.GraphicsDevice.DrawPolygon(points, new GraphicsContext(pGraphicsContext), context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void DrawPolyline(int numPoints, IntPtr x, IntPtr y, IntPtr pGraphicsContext, IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                var points = GetPoints(numPoints, x, y);
                context.GraphicsDevice.DrawPolyline(points,
                                                    new GraphicsContext(pGraphicsContext),
                                                    context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void DrawRectangle(double x0, double y0, double x1, double y1, IntPtr pGraphicsContext, IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                var rectangle = new Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x1 - x0), Math.Abs(y1 - y0));
                context.GraphicsDevice.DrawRectangle(rectangle,
                                                     new GraphicsContext(pGraphicsContext),
                                                     context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void DrawPath(IntPtr x,
                              IntPtr y,
                              int npoly,
                              IntPtr nper,
                              bool winding,
                              IntPtr pGraphicsContext,
                              IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                var points = GetPoints(x, y, npoly, nper);
                context.GraphicsDevice.DrawPath(points,
                                                winding,
                                                new GraphicsContext(pGraphicsContext),
                                                context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void DrawRaster(IntPtr raster,
                                int rasterWidth,
                                int rasterHeight,
                                double x,
                                double y,
                                double width,
                                double height,
                                double rot,
                                bool interpolate,
                                IntPtr pGraphicsContext,
                                IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                var output = RasterToBitmap(raster, rasterWidth, rasterHeight);
                context.GraphicsDevice.DrawRaster(output,
                                                new Rectangle(x, y, width, height),
                                                rot,
                                                interpolate,
                                                new GraphicsContext(pGraphicsContext),
                                                context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private static Bitmap RasterToBitmap(IntPtr raster, int w, int h)
        {
            var result = new Bitmap(w, h);
            for (var i = 0; i < w; i++)
            {
                for (var j = 0; j < h; j++)
                {
                    var rColor = Color.FromUInt32((uint) Marshal.ReadInt32(raster));
                    result.SetPixel(i, j, ConvertColor(rColor));
                    raster = IntPtr.Add(raster, sizeof (int));
                }
            }
            return result;
        }

        private IntPtr Capture(IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                context.GraphicsDevice.Capture(context.DeviceDescription);
                return IntPtr.Zero;
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private double MeasureWidth(string str, IntPtr pGraphicsContext, IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                return context.GraphicsDevice.MeasureWidth(str,
                                                           new GraphicsContext(pGraphicsContext),
                                                           context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private void DrawText(double x,
                              double y,
                              string str,
                              double rot,
                              double hadj,
                              IntPtr pGraphicsContext,
                              IntPtr pDevDesc)
        {
            try
            {
                var context = _devices.Single(d => d.DeviceDescription.DangerousGetHandle() == pDevDesc);
                context.GraphicsDevice.DrawText(str,
                                                new Point(x, y),
                                                rot,
                                                hadj,
                                                new GraphicsContext(pGraphicsContext),
                                                context.DeviceDescription);
            }
            catch (Exception ex)
            {
                throw new PlotException(ex);
            }
        }

        private static IntPtr GetEvent(IntPtr sexp, string s)
        {
            return IntPtr.Zero;
        }

        private static void EventHelper(IntPtr dd, int code)
        { }

        private static IEnumerable<Point> GetPoints(int n, IntPtr x, IntPtr y)
        {
            return Enumerable.Range(0, n).Select(
                index =>
                {
                    var offset = sizeof (double)*index;
                    var px = ReadDouble(x, offset);
                    var py = ReadDouble(y, offset);
                    return new Point(px, py);
                });
        }

        private static IEnumerable<IEnumerable<Point>> GetPoints(IntPtr x, IntPtr y, int npoly, IntPtr nper)
        {
            for (var index = 0; index < npoly; index++)
            {
                var offset = sizeof (int)*index;
                var n = Marshal.ReadInt32(nper, offset);
                yield return GetPoints(n, x, y);
                var pointOffset = sizeof (double)*n;
                x = IntPtr.Add(x, pointOffset);
                y = IntPtr.Add(y, pointOffset);
            }
        }

        private static double ReadDouble(IntPtr pointer, int offset)
        {
            var value = new double[1];
            Marshal.Copy(IntPtr.Add(pointer, offset), value, 0, value.Length);
            return value[0];
        }

        private static System.Drawing.Color ConvertColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

        public void Dispose()
        {
            _devices.ForEach(Kill);
            _devices.ForEach(CleanupDeviceContext);
            _devices.Clear();
        }

        private void Kill(GraphicsDeviceContext context)
        {
            _rInstance.GetFunction<GEkillDevice>()(context.DeviceDescription.DangerousGetHandle());
        }

        private static void CleanupDeviceContext(GraphicsDeviceContext context)
        {
            var geDevDesc = (GraphicsEngineDeviceDescription)Marshal.PtrToStructure(context.GEDeviceDescriptionPtr, typeof(GraphicsEngineDeviceDescription));
            geDevDesc.dev = IntPtr.Zero;
            Marshal.StructureToPtr(geDevDesc, context.GEDeviceDescriptionPtr, false);

            if (context.DeviceDescription != null)
            {
                context.DeviceDescription.Dispose();
                context.DeviceDescription = null;
            }
        }

        private class GraphicsDeviceContext
        {
            public DeviceDescription DeviceDescription { get; set; }
            public IntPtr GEDeviceDescriptionPtr { get; set; }
            public IGraphicsDevice GraphicsDevice { get; set; }
        }
    }
}
