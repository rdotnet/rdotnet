﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RDotNet.Graphics.Internals;

namespace RDotNet.Graphics
{
	internal class GraphicsDeviceAdapter
	{
		private readonly IGraphicsDevice device;
		private REngine engine;

		public GraphicsDeviceAdapter(IGraphicsDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			this.device = device;
		}

		public REngine Engine
		{
			get { return this.engine; }
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

			engine.GetFunction<R_GE_checkVersionOrDie>("R_GE_checkVersionOrDie")(this.device.Version);
			engine.GetFunction<R_CheckDeviceAvailable>("R_CheckDeviceAvailable")();
			var oldSuspended = GetInterruptsSuspended(engine);
			SetInterruptsSuspended(engine, true);

			var description = new DeviceDescription();
			SetMethod(description);
			var gdd = engine.GetFunction<GEcreateDevDesc>("GEcreateDevDesc")(description.DangerousGetHandle());
			engine.GetFunction<GEaddDevice2>("GEaddDevice2")(gdd, this.device.Name);

			SetInterruptsSuspended(engine, oldSuspended);
			if (GetInterruptsPending(engine) && !GetInterruptsSuspended(engine))
			{
				engine.GetFunction<Rf_onintr>("Rf_onintr")();
			}
		}

		private static void SetInterruptsSuspended(REngine engine, bool value)
		{
			var pointer = engine.DangerousGetHandle("R_interrupts_suspended");
			Marshal.WriteInt32(pointer, Convert.ToInt32(value));
		}

		private static bool GetInterruptsSuspended(REngine engine)
		{
			var pointer = engine.DangerousGetHandle("R_interrupts_suspended");
			return Convert.ToBoolean(Marshal.ReadInt32(pointer));
		}

		private static bool GetInterruptsPending(REngine engine)
		{
			var pointer = engine.DangerousGetHandle("R_interrupts_pending");
			return Convert.ToBoolean(Marshal.ReadInt32(pointer));
		}

		private void SetMethod(DeviceDescription description)
		{
			description.SetMethod("activate", (_DevDesc_activate)Activate);
			description.SetMethod("cap", (_DevDesc_cap)Capture);
			description.SetMethod("circle", (_DevDesc_circle)DrawCircle);
			description.SetMethod("clip", (_DevDesc_clip)Clip);
			description.SetMethod("close", (_DevDesc_close)Close);
			description.SetMethod("deactivate", (_DevDesc_deactivate)Deactivate);
			description.SetMethod("line", (_DevDesc_line)DrawLine);
			description.SetMethod("locator", (_DevDesc_locator)GetLocation);
			description.SetMethod("metricInfo", (_DevDesc_metricInfo)GetMetricInfo);
			description.SetMethod("mode", (_DevDesc_mode)ChangeMode);
			description.SetMethod("newPage", (_DevDesc_newPage)NewPage);
			description.SetMethod("path", (_DevDesc_path)DrawPath);
			description.SetMethod("polygon", (_DevDesc_polygon)DrawPolygon);
			description.SetMethod("polyline", (_DevDesc_Polyline)DrawPolyline);
			description.SetMethod("raster", (_DevDesc_raster)DrawRaster);
			description.SetMethod("rect", (_DevDesc_rect)DrawRectangle);
			description.SetMethod("size", (_DevDesc_size)Resize);
			description.SetMethod("strWidth", (_DevDesc_strWidth)MeasureWidth);
			description.SetMethod("text", (_DevDesc_text)DrawText);
			description.SetMethod("strWidthUTF8", (_DevDesc_strWidth)MeasureWidth);
			description.SetMethod("textUTF8", (_DevDesc_text)DrawText);
			description.SetMethod("newFrameConfirm", (_DevDesc_newFrameConfirm)ConfirmNewFrame);
			description.SetMethod("getEvent", (_DevDesc_getEvent)GetEvent);
			description.SetMethod("eventHelper", (_DevDesc_eventHelper)EventHelper);
		}

		private void Activate(IntPtr pDevDesc)
		{
			var description = new DeviceDescription(pDevDesc);
			this.device.OnActivated(description);
		}

		private void Deactivate(IntPtr pDevDesc)
		{
			var description = new DeviceDescription(pDevDesc);
			this.device.OnDeactivated(description);
		}

		private void NewPage(IntPtr gc, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var context = new GraphicsContext(gc);
			this.device.OnNewPageRequested(context, description);
		}

		private void Resize(out double left, out double right, out double bottom, out double top, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var rectangle = this.device.OnResized(description);
			left = rectangle.Left;
			right = rectangle.Right;
			bottom = rectangle.Bottom;
			top = rectangle.Top;
		}

		private void Close(IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			this.device.OnClosed(description);
		}

		private bool ConfirmNewFrame(IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			return this.device.ConfirmNewFrame(description);
		}

		private void ChangeMode(int mode, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			if (mode == 0)
			{
				this.device.OnDrawStarted(description);
			}
			else if (mode == 1)
			{
				this.device.OnDrawStopped(description);
			}
		}

		private void DrawCircle(double x, double y, double r, IntPtr gc, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var context = new GraphicsContext(gc);
			var center = new Point(x, y);
			this.device.DrawCircle(center, r, context, description);
		}

		private void Clip(double x0, double x1, double y0, double y1, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var rectangle = new Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x0 - x1), Math.Abs(y0 - y1));
			this.device.Clip(rectangle, description);
		}

		private bool GetLocation(out double x, out double y, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var location = this.device.GetLocation(description);
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

		private void DrawLine(double x1, double y1, double x2, double y2, IntPtr gc, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var context = new GraphicsContext(gc);
			var source = new Point(x1, y1);
			var destination = new Point(x2, y2);
			this.device.DrawLine(source, destination, context, description);
		}

		private void GetMetricInfo(int c, IntPtr gc, out double ascent, out double descent, out double width, IntPtr dd)
		{
			var context = new GraphicsContext(gc);
			var description = new DeviceDescription(dd);
			var metric = this.device.GetMetricInfo(c, context, description);
			ascent = metric.Ascent;
			descent = metric.Descent;
			width = metric.Width;
		}

		private void DrawPolygon(int n, IntPtr x, IntPtr y, IntPtr gc, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var context = new GraphicsContext(gc);
			var points = GetPoints(n, x, y);
			this.device.DrawPolygon(points, context, description);
		}

		private void DrawPolyline(int n, IntPtr x, IntPtr y, IntPtr gc, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var context = new GraphicsContext(gc);
			var points = GetPoints(n, x, y);
			this.device.DrawPolyline(points, context, description);
		}

		private void DrawRectangle(double x0, double y0, double x1, double y1, IntPtr gc, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var context = new GraphicsContext(gc);
			var rectangle = new Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x0 - x1), Math.Abs(y0 - y1));
			this.device.DrawRectangle(rectangle, context, description);
		}

		private void DrawPath(IntPtr x, IntPtr y, int npoly, IntPtr nper, bool winding, IntPtr gc, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var context = new GraphicsContext(gc);
			var points = GetPoints(x, y, npoly, nper);
			this.device.DrawPath(points, winding, context, description);
		}

		private void DrawRaster(IntPtr raster, int w, int h, double x, double y, double width, double height, double rot, bool interpolate, IntPtr gc, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var context = new GraphicsContext(gc);
			var output = new Raster(w, h);
			unchecked
			{
				for (var i = 0; i < w; i++)
				{
					for (var j = 0; j < h; j++)
					{
						output[i, j] = Color.FromUInt32((uint)Marshal.ReadInt32(raster));
						raster = IntPtr.Add(raster, sizeof(int));
					}
				}
			}
			this.device.DrawRaster(output, new Rectangle(x, y, width, height), rot, interpolate, context, description);
		}

		private IntPtr Capture(IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var raster = this.device.Capture(description);
			return Engine.CreateIntegerMatrix(raster).DangerousGetHandle();
		}

		private double MeasureWidth(string str, IntPtr gc, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var context = new GraphicsContext(gc);
			return this.device.MeasureWidth(str, context, description);
		}

		private void DrawText(double x, double y, string str, double rot, double hadj, IntPtr gc, IntPtr dd)
		{
			var description = new DeviceDescription(dd);
			var context = new GraphicsContext(gc);
			this.device.DrawText(str, new Point(x, y), rot, hadj, context, description);
		}

		private IntPtr GetEvent(IntPtr sexp, string s)
		{
			return IntPtr.Zero;
		}

		private void EventHelper(IntPtr dd, int code)
		{}

		private IEnumerable<Point> GetPoints(int n, IntPtr x, IntPtr y)
		{
			return Enumerable.Range(0, n).Select(
				index =>
				{
					var offset = sizeof(double) * index;
					var px = Utility.ReadDouble(x, offset);
					var py = Utility.ReadDouble(y, offset);
					return new Point(px, py);
				}
				);
		}

		private IEnumerable<IEnumerable<Point>> GetPoints(IntPtr x, IntPtr y, int npoly, IntPtr nper)
		{
			if (!Engine.IsRunning)
			{
				throw new InvalidOperationException();
			}
			for (var index = 0; index < npoly; index++)
			{
				var offset = sizeof(int) * index;
				var n = Marshal.ReadInt32(nper, offset);
				yield return GetPoints(n, x, y);
				var pointOffset = sizeof(double) * n;
				x = IntPtr.Add(x, pointOffset);
				y = IntPtr.Add(y, pointOffset);
			}
		}
	}
}
