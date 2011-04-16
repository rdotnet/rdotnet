using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RDotNet.Graphics.Internals;
using System.Linq;

namespace RDotNet.Graphics
{
	public abstract class GraphicsDevice
	{
		public event EventHandler<GraphicsDeviceEventArgs> Activated;
		public event EventHandler<GraphicsDeviceEventArgs> Deactivated;
		public event EventHandler<GraphicsDeviceEventArgs> NewPageRequested;
		public event EventHandler<GraphicsDeviceEventArgs> Resized;
		public event EventHandler<GraphicsDeviceEventArgs> Closed;
		public event EventHandler<GraphicsDeviceEventArgs> DrawStarted;
		public event EventHandler<GraphicsDeviceEventArgs> DrawStopped;

		private REngine engine;
		public REngine Engine
		{
			get { return this.engine; }
		}

		public virtual string Name
		{
			get
			{
				return GetType().Name;
			}
		}

		public virtual bool IsInvalid
		{
			get { return Engine == null || Engine.IsInvalid; }
		}

		protected GraphicsDevice()
		{
		}

		public void SetEngine(REngine engine)
		{
			if (engine == null)
			{
				throw new ArgumentNullException("engine");
			}
			if (this.engine != null)
			{
				throw new InvalidOperationException();
			}
			if (!engine.IsRunning)
			{
				throw new ArgumentException(null, "engine");
			}
			this.engine = engine;

			INativeMethodsProxy proxy = GetProxy(engine);

			proxy.R_GE_checkVersionOrDie(Constants.R_GE_version);
			proxy.R_CheckDeviceAvailable();
			bool oldSuspended = GetInterruptsSuspended(engine);
			SetInterruptsSuspended(engine, true);

			DeviceDescription description = CreateDefaultDescription();
			SetMethod(description);
			IntPtr gdd = proxy.GEcreateDevDesc(description.DangerousGetHandle());
			proxy.GEaddDevice2(gdd, Name);

			SetInterruptsSuspended(engine, oldSuspended);
			if (GetInterruptsPending(engine) && !GetInterruptsSuspended(engine))
			{
				proxy.Rf_onintr();
			}
		}

		private static INativeMethodsProxy GetProxy(REngine engine)
		{
			OperatingSystem os = System.Environment.OSVersion;
			switch (os.Platform)
			{
				case PlatformID.Win32NT:
					return DirectNativeMethods.Instance;
				case PlatformID.MacOSX:
				case PlatformID.Unix:
					return new DelegateNativeMethods(engine);
				default:
					throw new NotSupportedException(os.ToString());
			}
		}

		private static void SetInterruptsSuspended(REngine engine, bool value)
		{
			IntPtr pointer = engine.DangerousGetHandle("R_interrupts_suspended");
			Marshal.WriteInt32(pointer, Convert.ToInt32(value));
		}

		private static bool GetInterruptsSuspended(REngine engine)
		{
			IntPtr pointer = engine.DangerousGetHandle("R_interrupts_suspended");
			return Convert.ToBoolean(Marshal.ReadInt32(pointer));
		}

		private static bool GetInterruptsPending(REngine engine)
		{
			IntPtr pointer = engine.DangerousGetHandle("R_interrupts_pending");
			return Convert.ToBoolean(Marshal.ReadInt32(pointer));
		}

		protected virtual DeviceDescription CreateDefaultDescription()
		{
			DeviceDescription description = new DeviceDescription();
			DevDesc dd;
			description.GetDescription(out dd);
			dd.startps = 12.0;
			dd.startcol = Colors.Black;
			dd.startfill = Colors.White;
			dd.startlty = LineType.Solid;
			dd.startfont = 1;
			dd.startgamma = 1.0;
			dd.canClip = true;
			dd.canChangeGamma = false;
			dd.displayListOn = false;
			description.Copy(ref dd);
			return description;
		}

		private void SetMethod(DeviceDescription description)
		{
			DevDesc dd;
			description.GetDescription(out dd);
			dd.activate = this.Activate;
			dd.cap = this.Capture;
			dd.circle = this.DrawCircle;
			dd.clip = this.Clip;
			dd.close = this.Close;
			dd.deactivate = this.Deactivate;
			dd.line = this.DrawLine;
			dd.locator = this.GetLocation;
			dd.metricInfo = this.GetMetricInfo;
			dd.mode = this.ChangeMode;
			dd.newPage = this.NewPage;
			dd.path = this.DrawPath;
			dd.polygon = this.DrawPolygon;
			dd.polyline = this.DrawPolyline;
			dd.raster = this.DrawRaster;
			dd.rect = this.DrawRectangle;
			dd.size = this.Resize;
			dd.strWidth = this.MeasureWidth;
			dd.text = this.DrawText;
			dd.strWidthUTF8 = this.MeasureWidth;
			dd.textUTF8 = this.DrawText;
			dd.newFrameConfirm = this.ConfirmNewFrame;
			dd.getEvent = this.GetEvent;
			dd.eventHelper = this.EventHelper;
			description.Copy(ref dd);
		}

		protected virtual void OnActivated(GraphicsDeviceEventArgs e)
		{
			if (Activated != null)
			{
				Activated(this, e);
			}
		}

		protected virtual void OnDeactivated(GraphicsDeviceEventArgs e)
		{
			if (Deactivated != null)
			{
				Deactivated(this, e);
			}
		}

		protected virtual void OnNewPageRequested(GraphicsDeviceEventArgs e)
		{
			if (NewPageRequested != null)
			{
				NewPageRequested(this, e);
			}
		}

		protected virtual Rectangle OnResized(GraphicsDeviceEventArgs e)
		{
			if (Resized != null)
			{
				Resized(this, e);
			}
			return GetSize(e.Context, e.Description);
		}

		protected virtual void OnClosed(GraphicsDeviceEventArgs e)
		{
			if (Closed != null)
			{
				Closed(this, e);
			}
		}

		protected virtual void OnDrawStarted(GraphicsDeviceEventArgs e)
		{
			if (DrawStarted != null)
			{
				DrawStarted(this, e);
			}
		}

		protected virtual void OnDrawStopped(GraphicsDeviceEventArgs e)
		{
			if (DrawStopped != null)
			{
				DrawStopped(this, e);
			}
		}

		protected abstract Rectangle GetSize(GraphicsContext context, DeviceDescription description);

		protected abstract bool ConfirmNewFrame(DeviceDescription description);
		protected abstract void DrawCircle(Point center, double radius, GraphicsContext context, DeviceDescription description);
		protected abstract void Clip(Rectangle rectangle, DeviceDescription description);
		protected abstract Point? GetLocation(DeviceDescription description);
		protected abstract void DrawLine(Point source, Point destination, GraphicsContext context, DeviceDescription description);
		protected abstract MetricsInfo GetMetricInfo(int character, GraphicsContext context, DeviceDescription description);
		protected abstract void DrawPolygon(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description);
		protected abstract void DrawPolyline(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description);
		protected abstract void DrawRectangle(Rectangle rectangle, GraphicsContext context, DeviceDescription description);
		protected abstract void DrawPath(IEnumerable<IEnumerable<Point>> points, bool winding, GraphicsContext context, DeviceDescription description);
		protected abstract void DrawRaster(Raster raster, Rectangle destination, double rotation, bool interpolated, GraphicsContext context, DeviceDescription description);
		protected abstract Raster Capture(DeviceDescription description);
		protected abstract double MeasureWidth(string s, GraphicsContext context, DeviceDescription description);
		protected abstract void DrawText(string s, Point location, double rotation, double adjustment, GraphicsContext context, DeviceDescription description);

		private void Activate(IntPtr pDevDesc)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(pDevDesc);
			OnActivated(new GraphicsDeviceEventArgs(description));
		}

		private void Deactivate(IntPtr pDevDesc)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(pDevDesc);
			OnDeactivated(new GraphicsDeviceEventArgs(description));
		}

		private void NewPage(IntPtr gc, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			GraphicsContext context = new GraphicsContext(gc);
			OnNewPageRequested(new GraphicsDeviceEventArgs(description, context));
		}

		private void Resize(out double left, out double right, out double bottom, out double top, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			Rectangle rectangle = OnResized(new GraphicsDeviceEventArgs(description));
			left = rectangle.Left;
			right = rectangle.Right;
			bottom = rectangle.Bottom;
			top = rectangle.Top;
		}

		private void Close(IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			OnClosed(new GraphicsDeviceEventArgs(description));
		}

		private bool ConfirmNewFrame(IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			return ConfirmNewFrame(description);
		}

		private void ChangeMode(int mode, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			GraphicsDeviceEventArgs e = new GraphicsDeviceEventArgs(description);
			if (mode == 0)
			{
				OnDrawStarted(e);
			}
			else if (mode == 1)
			{
				OnDrawStopped(e);
			}
		}

		private void DrawCircle(double x, double y, double r, IntPtr gc, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			GraphicsContext context = new GraphicsContext(gc);
			Point center = new Point(x, y);
			DrawCircle(center, r, context, description);
		}

		private void Clip(double x0, double x1, double y0, double y1, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			Rectangle rectangle = new Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x0 - x1), Math.Abs(y0 - y1));
			Clip(rectangle, description);
		}

		private bool GetLocation(out double x, out double y, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			Point? location = GetLocation(description);
			if (!location.HasValue)
			{
				x = default(double);
				y = default(double);
				return false;
			}

			Point p = location.Value;
			x = p.X;
			y = p.Y;
			return true;
		}

		private void DrawLine(double x1, double y1, double x2, double y2, IntPtr gc, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			GraphicsContext context = new GraphicsContext(gc);
			Point source = new Point(x1, y1);
			Point destination = new Point(x2, y2);
			DrawLine(source, destination, context, description);
		}

		private void GetMetricInfo(int c, IntPtr gc, out double ascent, out double descent, out double width, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			GraphicsContext context = new GraphicsContext(gc);
			DeviceDescription description = new DeviceDescription(dd);
			MetricsInfo metric = GetMetricInfo(c, context, description);
			ascent = metric.Ascent;
			descent = metric.Descent;
			width = metric.Width;
		}

		private void DrawPolygon(int n, IntPtr x, IntPtr y, IntPtr gc, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			GraphicsContext context = new GraphicsContext(gc);
			var points = GetPoints(n, x, y);
			DrawPolygon(points, context, description);
		}

		private void DrawPolyline(int n, IntPtr x, IntPtr y, IntPtr gc, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			GraphicsContext context = new GraphicsContext(gc);
			var points = GetPoints(n, x, y);
			DrawPolyline(points, context, description);
		}

		private void DrawRectangle(double x0, double y0, double x1, double y1, IntPtr gc, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			GraphicsContext context = new GraphicsContext(gc);
			Rectangle rectangle = new Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x0 - x1), Math.Abs(y0 - y1));
			DrawRectangle(rectangle, context, description);
		}

		private void DrawPath(IntPtr x, IntPtr y, int npoly, IntPtr nper, bool winding, IntPtr gc, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			GraphicsContext context = new GraphicsContext(gc);
			var points = GetPoints(x, y, npoly, nper);
			DrawPath(points, winding, context, description);
		}

		private void DrawRaster(IntPtr raster, int w, int h, double x, double y, double width, double height, double rot, bool interpolate, IntPtr gc, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			GraphicsContext context = new GraphicsContext(gc);
			Raster output = new Raster(w, h);
			unchecked
			{
				for (int i = 0; i < w; i++)
				{
					for (int j = 0; j < h; j++)
					{
						output[i, j] = Color.FromUInt32((uint)Marshal.ReadInt32(raster));
						raster = IntPtr.Add(raster, sizeof(int));
					}
				}
			}
			DrawRaster(output, new Rectangle(x, y, width, height), rot, interpolate, context, description);
		}

		private IntPtr Capture(IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			Raster raster = Capture(description);
			return Engine.CreateIntegerMatrix(raster).DangerousGetHandle();
		}

		private double MeasureWidth(string str, IntPtr gc, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			GraphicsContext context = new GraphicsContext(gc);
			return MeasureWidth(str, context, description);
		}

		private void DrawText(double x, double y, string str, double rot, double hadj, IntPtr gc, IntPtr dd)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			DeviceDescription description = new DeviceDescription(dd);
			GraphicsContext context = new GraphicsContext(gc);
			DrawText(str, new Point(x, y), rot, hadj, context, description);
		}

		private IntPtr GetEvent(IntPtr sexp, string s)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			return IntPtr.Zero;
		}

		private void EventHelper(IntPtr dd, int code)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
		}

		private IEnumerable<Point> GetPoints(int n, IntPtr x, IntPtr y)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			return Enumerable.Range(0, n).Select(
				index =>
				{
					int offset = sizeof(double) * index;
					double px = Utility.ReadDouble(x, offset);
					double py = Utility.ReadDouble(y, offset);
					return new Point(px, py);
				}
			);
		}

		private IEnumerable<IEnumerable<Point>> GetPoints(IntPtr x, IntPtr y, int npoly, IntPtr nper)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			for (int index = 0; index < npoly; index++)
			{
				int offset = sizeof(int) * index;
				int n = Marshal.ReadInt32(nper, offset);
				yield return GetPoints(n, x, y);
				int pointOffset = sizeof(double) * n;
				x = IntPtr.Add(x, pointOffset);
				y = IntPtr.Add(y, pointOffset);
			}
		}
	}
}
