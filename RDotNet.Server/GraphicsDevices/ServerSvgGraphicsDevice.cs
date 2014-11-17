using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using RDotNet.R.Adapter.Graphics;
using Svg;
using Svg.Pathing;
using Svg.Transforms;
using Point = RDotNet.R.Adapter.Graphics.Point;
using Rectangle = RDotNet.R.Adapter.Graphics.Rectangle;

namespace RDotNet.Server.GraphicsDevices
{
    public class ServerSvgGraphicsDevice : IGraphicsDevice
    {
        private readonly ISvgContextMapper _mapper;
        private SvgDocument _currentPlot;
        private Uri _currentClipUri;
        private int _clipId;
        private Dictionary<string, SvgGroup> _clipGroups;
        private readonly List<SvgDocument> _plots = new List<SvgDocument>();
        private readonly Rectangle _initialClip;
        private readonly Rectangle _initalBounds;

        public ServerSvgGraphicsDevice()
            : this(new SvgContextMapper(DefaultUnitType, DefaultFont), DefaultBounds, DefaultClip)
        { }

        public ServerSvgGraphicsDevice(ISvgContextMapper mapper, Rectangle bounds, Rectangle clip)
        {
            if (mapper == null) throw new ArgumentNullException("mapper");
            _mapper = mapper;
            Name = "ServerSvgGraphicsDevice";

            _initalBounds = bounds;
            _initialClip = clip;
            CurrentBounds = bounds;
            CurrentClip = clip;
        }

        public static readonly Rectangle DefaultBounds = new Rectangle(0, 0, 700, 700);
        public static readonly Rectangle DefaultClip = new Rectangle(0, 0, 700, 700);
        public const SvgUnitType DefaultUnitType = SvgUnitType.Pixel;
        public static readonly Font DefaultFont = new Font("Arial", 12, GraphicsUnit.Point);

        public string Name { get; private set; }
        public Rectangle CurrentBounds { get; private set; }
        public Rectangle CurrentClip { get; private set; }

        public void SetDeviceParameters(DeviceDescription description)
        {
            description.CharacterSizeInRasterY = _mapper.Font.GetHeight();
            description.IsClippable = true;
            description.Adjustment = Adjustment.LeftMiddleRight;
            description.HaveTransparency = YesOrNo.Yes;
            description.HaveTransparentBackground = YesNoOrMostly.Yes;
            description.IsTextRotatedInContour = true;
            description.HaveLocator = YesOrNo.No;
            description.HaveCapture = YesOrNo.No;
            description.HaveRaster = YesNoOrMostly.Yes;
            description.Bounds = CurrentBounds;
            description.ClipBounds = CurrentClip;
        }

        public void NewPageRequested(GraphicsContext context, DeviceDescription description)
        {
            var mapped = _mapper.MapGraphicsContextToSvg(CurrentBounds, context);
            if (_currentPlot != null && !_currentPlot.Children.Any())
            {
                _plots.Remove(_currentPlot);
            }

            _currentPlot = new SvgDocument
            {
                Width = new SvgUnit(mapped.UnitType, (float)CurrentBounds.Width),
                Height = new SvgUnit(mapped.UnitType, (float)CurrentBounds.Height)
            };
            _clipGroups = new Dictionary<string, SvgGroup>();

            SetClip(CurrentClip, description);
            _plots.Add(_currentPlot);
        }

        public bool ConfirmNewFrame(DeviceDescription description)
        {
            return true;
        }

        public Rectangle Resized(DeviceDescription description)
        {
            return CurrentBounds;
        }

        public void Activated(DeviceDescription description)
        { }

        public void Deactivated(DeviceDescription description)
        { }

        public void DrawStarted(DeviceDescription description)
        { }

        public void DrawStopped(DeviceDescription description)
        { }

        public void Closed(DeviceDescription description)
        { }

        public void DrawLine(Point source, Point destination, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(CurrentBounds, source, context);
            var end = _mapper.MapPoint(CurrentBounds, destination, 0d);

            AddChild(new SvgLine
            {
                EndX = end.X,
                EndY = end.Y,
                Fill = svgContext.Fill,
                FillOpacity = 0,
                StartX = svgContext.Coordinate.X,
                StartY = svgContext.Coordinate.Y,
                Stroke = svgContext.Pen.Stroke,
                StrokeDashArray = svgContext.Pen.StrokeDashArray,
                StrokeLineCap = svgContext.Pen.StrokeLineCap,
                StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                StrokeOpacity = svgContext.Pen.StrokeOpacity,
                StrokeWidth = svgContext.Pen.StrokeWidth
            });
        }

        public void DrawCircle(Point center, double radius, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(CurrentBounds, center, context);

            AddChild(new SvgCircle
            {
                CenterX = svgContext.Coordinate.X,
                CenterY = svgContext.Coordinate.Y,
                Fill = svgContext.Fill,
                FillOpacity = svgContext.FillOpacity,
                Radius = new SvgUnit(svgContext.UnitType, (float) (radius)),
                Stroke = svgContext.Pen.Stroke,
                StrokeDashArray = svgContext.Pen.StrokeDashArray,
                StrokeLineCap = svgContext.Pen.StrokeLineCap,
                StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                StrokeOpacity = svgContext.Pen.StrokeOpacity,
                StrokeWidth = svgContext.Pen.StrokeWidth
            });
        }

        public void DrawPolygon(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(CurrentBounds, context);

            var collection = new SvgUnitCollection();
            collection.AddRange(points.Select(p => _mapper.MapPoint(CurrentBounds, p, SvgUnitType.User))
                .Select(p => new[] {p.X, p.Y})
                .SelectMany(p => p));

            AddChild(new SvgPolygon
            {
                Fill = svgContext.Fill,
                FillOpacity = svgContext.FillOpacity,
                Points = collection,
                Stroke = svgContext.Pen.Stroke,
                StrokeDashArray = svgContext.Pen.StrokeDashArray,
                StrokeLineCap = svgContext.Pen.StrokeLineCap,
                StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                StrokeOpacity = svgContext.Pen.StrokeOpacity,
                StrokeWidth = svgContext.Pen.StrokeWidth
            });
        }

        public void DrawPolyline(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(CurrentBounds, context);

            var collection = new SvgUnitCollection();
            collection.AddRange(points.Select(p => _mapper.MapPoint(CurrentBounds, p, SvgUnitType.User))
                .Select(p => new[] {p.X, p.Y})
                .SelectMany(p => p));

            AddChild(new SvgPolyline
            {
                Fill = svgContext.Fill,
                FillOpacity = 0,
                Points = collection,
                Stroke = svgContext.Pen.Stroke,
                StrokeDashArray = svgContext.Pen.StrokeDashArray,
                StrokeLineCap = svgContext.Pen.StrokeLineCap,
                StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                StrokeOpacity = svgContext.Pen.StrokeOpacity,
                StrokeWidth = svgContext.Pen.StrokeWidth
            });
        }

        public void DrawRectangle(Rectangle rectangle, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(CurrentBounds, new Point(rectangle.Left, rectangle.Top), context);

            AddChild(new SvgRectangle
            {
                Fill = svgContext.Fill,
                FillOpacity = svgContext.FillOpacity,
                Height = (float)rectangle.Height,
                Stroke = svgContext.Pen.Stroke,
                StrokeDashArray = svgContext.Pen.StrokeDashArray,
                StrokeLineCap = svgContext.Pen.StrokeLineCap,
                StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                StrokeOpacity = svgContext.Pen.StrokeOpacity,
                StrokeWidth = svgContext.Pen.StrokeWidth,
                Width = (float) rectangle.Width,
                X = svgContext.Coordinate.X,
                Y = svgContext.Coordinate.Y,
            });
        }

        public void DrawPath(IEnumerable<IEnumerable<Point>> points,
                            bool winding,
                            GraphicsContext context,
                            DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(CurrentBounds, context);

            foreach (var point in points)
            {
                var vertices = point.ToList();
                var list = new SvgPathSegmentList();
                var first = vertices.First();
                list.Add(new SvgMoveToSegment(new PointF((float) first.X, (float) first.Y)));

                foreach (var vertex in vertices.Skip(1))
                {
                    list.Add(new SvgLineSegment(new PointF((float) first.X, (float) first.Y),
                        new PointF((float) vertex.X, (float) vertex.Y)));
                    first = vertex;
                }

                list.Add(new SvgClosePathSegment());

                AddChild(new SvgPath
                {
                    Fill = svgContext.Fill,
                    FillOpacity = svgContext.FillOpacity,
                    PathData = list,
                    Stroke = svgContext.Pen.Stroke,
                    StrokeDashArray = svgContext.Pen.StrokeDashArray,
                    StrokeLineCap = svgContext.Pen.StrokeLineCap,
                    StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                    StrokeOpacity = svgContext.Pen.StrokeOpacity,
                    StrokeWidth = svgContext.Pen.StrokeWidth,
                });
            }
        }

        public void DrawRaster(Bitmap bitmap,
                                Rectangle rectangle,
                                double rotation,
                                bool interpolated,
                                GraphicsContext context,
                                DeviceDescription description)
        {
            using (var converted = new MemoryStream())
            {
                bitmap.Save(converted, ImageFormat.Png);
                var base64 = Convert.ToBase64String(converted.GetBuffer());

                var svgContext = _mapper.MapGraphicsContextToSvg(CurrentBounds, new Point(rectangle.Left, rectangle.Top), rotation, context);
                AddChild(new SvgImage
                {
                    Href = new Uri(String.Format("data:image/png;base64,{0}", base64), UriKind.Absolute),
                    Fill = svgContext.Fill,
                    FillOpacity = svgContext.FillOpacity,
                    Height = (float) rectangle.Height,
                    Stroke = svgContext.Pen.Stroke,
                    StrokeDashArray = svgContext.Pen.StrokeDashArray,
                    StrokeLineCap = svgContext.Pen.StrokeLineCap,
                    StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                    StrokeOpacity = svgContext.Pen.StrokeOpacity,
                    StrokeWidth = svgContext.Pen.StrokeWidth,
                    Width = (float) rectangle.Width,
                    X = svgContext.Coordinate.X,
                    Y = svgContext.Coordinate.Y,
                });
            }
        }

        public MetricsInfo GetMetricInfo(int character, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(CurrentBounds, new Point(0, 0), context);
            var s = new string(new[] { (char)character });

            // http://cran.r-project.org/doc/manuals/r-release/R-ints.html#Handling-text
            // "For ascent and descent this is intended to be the bounding box of the 
            // ‘ink’ put down by the glyph and not the box which might be used...
            // However, the width is used in plotmath to advance to the next character,
            // and so needs to include left and right bearings."
            var tempTextNode = CreateTextNode(s, svgContext);
            var result = new MetricsInfo
            {
                Ascent = Math.Max(0, svgContext.Coordinate.Y - tempTextNode.Bounds.Top),
                Descent = Math.Max(0, tempTextNode.Bounds.Bottom - svgContext.Coordinate.Y),
                Width = MeasureWidth(s, svgContext)
            };

            return result;
        }

        public double MeasureWidth(string s, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(CurrentBounds, context);
            return MeasureWidth(s, svgContext);
        }

        public void DrawText(string s,
                             Point location,
                             double rotation,
                             double adjustment,
                             GraphicsContext context,
                             DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(CurrentBounds, location, rotation, context);

            var textNode = CreateTextNode(s, svgContext);
            textNode.TextAnchor = GetAnchor(adjustment);
            textNode.Transforms = new SvgTransformCollection {svgContext.Coordinate.Rotation};

            AddChild(textNode);
        }

        public void SetClip(Rectangle rectangle, DeviceDescription description)
        {
            if ((rectangle.X - CurrentBounds.Left) < 1 &&
                (rectangle.Y - CurrentBounds.Top) < 1 &&
                (rectangle.Width - CurrentBounds.Width) < 1 &&
                (rectangle.Height - CurrentBounds.Height) < 1)
            {
                _currentClipUri = null;
                return;
            }

            var points = new List<Point>
            {
                new Point(rectangle.Left, rectangle.Top),
                new Point(rectangle.Right, rectangle.Top),
                new Point(rectangle.Right, rectangle.Bottom),
                new Point(rectangle.Left, rectangle.Bottom),
            };

            var collection = new SvgUnitCollection();
            collection.AddRange(points.Select(p => _mapper.MapPoint(CurrentBounds, p, SvgUnitType.User))
                .Select(p => new[] { p.X, p.Y })
                .SelectMany(p => p));

            var matchingClip = FindClip(collection);
            if (matchingClip != null)
            {
                _currentClipUri = new Uri(String.Format("url(#{0})", matchingClip), UriKind.Relative);
                return;
            }

            var clipName = "clip" + _clipId++;
            var path = new SvgClipPath { ID = clipName, ClipPathUnits = SvgCoordinateUnits.UserSpaceOnUse };

            var polygon = new SvgPolygon { Points = collection };
            path.Children.Add(polygon);
            AddClip(path);
            CurrentClip = rectangle;
            _currentClipUri = new Uri(String.Format("url(#{0})", clipName), UriKind.Relative);
        }

        public Raster Capture(DeviceDescription description)
        {
            throw new NotImplementedException();
        }

        public Point? GetLocation(DeviceDescription description)
        {
            throw new NotImplementedException();
        }

        public List<string> GetPlots()
        {
            var result = _plots.Where(p => p.Children.Any()).Select(WriteSvgDocToString).ToList();
            return result;
        }

        public void Clear()
        {
            _plots.Clear();
            if (_currentPlot != null)
            {
                _currentPlot.Children.Clear();
                _plots.Add(_currentPlot);
            }

            _currentClipUri = null;
            CurrentBounds = _initalBounds;
            CurrentClip = _initialClip;
        }

        private double MeasureWidth(string s, SvgGraphicsContext svgContext)
        {
            // this is a temporary fix to the issue of single-character text width 
            // calculations not including anything for spacing between the characters
            // "H" is selected because it won't have any kerning pairs in most fonts
            var tempCharText = CreateTextNode("HH", svgContext);
            var measureText = CreateTextNode("H" + s + "H", svgContext);

            return measureText.Bounds.Width - tempCharText.Bounds.Width;
        }

        private static SvgText CreateTextNode(string s, SvgGraphicsContext svgContext)
        {
            //BUG: [RMEL] Version 1.7.0 of the svg library would throw an exception on text.Bounds if the string was just spaces  " ".
            if (s != null && string.IsNullOrWhiteSpace(s))
            {
                //BUG: [RMEL] Workaround. Replace spaces with dashes.
                s = s.Replace(' ', '-');
            }

            return new SvgText
            {
                Fill = svgContext.Pen.Stroke,
                FillOpacity = svgContext.FillOpacity,
                FontFamily = svgContext.Font.Family,
                FontSize = svgContext.Font.Size,
                FontWeight = svgContext.Font.Weight,
                Text = s,
                TextAnchor = SvgTextAnchor.Middle,
                X = svgContext.Coordinate.X,
                Y = svgContext.Coordinate.Y,
            };
        }

        private static string WriteSvgDocToString(SvgDocument svg)
        {
            using (var stream = new MemoryStream())
            {
                svg.Write(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    // work around a casing issue for this enum. Creates console errors in the browser
                    return contents
                        .Replace("clipPathUnits=\"UserSpaceOnUse\"", "clipPathUnits=\"userSpaceOnUse\"")
                        .Replace("clipPathUnits=\"ObjectBoundingBox\"", "clipPathUnits=\"objectBoundingBox\"");
                }
            }
        }

        private string FindClip(SvgUnitCollection points)
        {
            var defs = _currentPlot.Children.GetSvgElementOf<SvgDefinitionList>();
            if (defs == null) return null;

            var match = defs.Children.FirstOrDefault(c => IsMatchingClip(c, points));

            return match != null ? match.ID : null;
        }

        private static bool IsMatchingClip(SvgElement element, SvgUnitCollection points)
        {
            if (!(element is SvgClipPath)) return false;

            var polygon = element.Children.GetSvgElementOf<SvgPolygon>();
            if (polygon.Points.Count != points.Count) return false;

            var differences = points.Select((t, i) => Math.Abs(polygon.Points[i].Value - t.Value));
            return differences.All(d => d < 0.99);
        }

        private void AddClip(SvgClipPath path)
        {
            var defs = _currentPlot.Children.GetSvgElementOf<SvgDefinitionList>();
            if (defs == null)
            {
                defs = new SvgDefinitionList();
                _currentPlot.Children.Add(defs);
            }
            defs.Children.Add(path);
        }

        private void AddChild(SvgElement element)
        {
            SvgGroup group;
            string clipKey = "~";
            if (_currentClipUri != null && !string.IsNullOrWhiteSpace(_currentClipUri.ToString()))
            {
                clipKey = _currentClipUri.ToString();
            }

            if (_clipGroups.ContainsKey(clipKey))
            {
                group = _clipGroups[clipKey];
            }
            else
            {
                group = new SvgGroup {ClipPath = _currentClipUri};
                _clipGroups.Add(clipKey, group);
                _currentPlot.Children.Add(group);
            }
            group.Children.Add(element);
        }

        private static SvgTextAnchor GetAnchor(double hadj)
        {
            //NOTE: [RMEL] These values come from src/extra/graphapp/gdraw.c for devWindows.c
            if (Math.Abs(hadj) < 0.25) return SvgTextAnchor.Start;
            if (Math.Abs(hadj) < 0.75) return SvgTextAnchor.Middle;

            return SvgTextAnchor.End;
        }
    }
}
