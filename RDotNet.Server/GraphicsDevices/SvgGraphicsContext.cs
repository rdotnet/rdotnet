using System.Drawing;
using RDotNet.R.Adapter.Graphics;
using Svg;
using Svg.DataTypes;
using Svg.Transforms;
using Color = System.Drawing.Color;
using Point = RDotNet.R.Adapter.Graphics.Point;
using Rectangle = RDotNet.R.Adapter.Graphics.Rectangle;

namespace RDotNet.Server.GraphicsDevices
{
    public class SvgCoordinateContext
    {
        public SvgRotate Rotation { get; set; }
        public SvgUnit X { get; set; }
        public SvgUnit Y { get; set; }
    }

    public class SvgPenContext
    {
        public SvgColourServer Stroke { get; set; }
        public SvgUnitCollection StrokeDashArray { get; set; }
        public SvgStrokeLineCap StrokeLineCap { get; set; }
        public SvgStrokeLineJoin StrokeLineJoin { get; set; }
        public float StrokeOpacity { get; set; }
        public SvgUnit StrokeWidth { get; set; }
    }

    public class SvgFontContext
    {
        public float CharacterExpansion { get; set; }
        public string Family { get; set; }
        public float LineHeight { get; set; }
        public float Size { get; set; }
        public SvgFontWeight Weight { get; set; }
    }

    public class SvgGraphicsContext
    {
        public SvgCoordinateContext Coordinate { get; set; }
        public SvgColourServer Fill { get; set; }
        public SvgFontContext Font { get; set; }
        public SvgPenContext Pen { get; set; }
        public SvgUnitType UnitType { get; set; }
        public float FillOpacity { get; set; }
    }

    public interface ISvgContextMapper
    {
        SvgGraphicsContext MapGraphicsContextToSvg(Rectangle bounds, Point point, double rotation, IGraphicsContext context);
        SvgGraphicsContext MapGraphicsContextToSvg(Rectangle bounds, Point point, IGraphicsContext context);
        SvgGraphicsContext MapGraphicsContextToSvg(Rectangle bounds, IGraphicsContext context);
        SvgCoordinateContext MapPoint(Rectangle bounds, Point point, double rotation, SvgUnitType type);
        SvgCoordinateContext MapPoint(Rectangle bounds, Point point, double rotation);
        SvgCoordinateContext MapPoint(Rectangle bounds, Point point, SvgUnitType unitType);
        SvgCoordinateContext MapPoint(Rectangle bounds, Point point);
        SvgPenContext MapPen(IGraphicsContext context);
        SvgFontContext MapFont(IGraphicsContext context);
        Font Font { get; }
    }

    public class SvgContextMapper : ISvgContextMapper
    {
        public SvgContextMapper(SvgUnitType unitType, Font font)
        {
            UnitType = unitType;
            Font = font;
        }

        public Font Font { get; private set; }
        public SvgUnitType UnitType { get; private set; }


        public SvgGraphicsContext MapGraphicsContextToSvg(Rectangle bounds, IGraphicsContext context)
        {
            return MapGraphicsContextToSvg(bounds, new Point(bounds.Left, bounds.Top), 0, context);
        }

        public SvgGraphicsContext MapGraphicsContextToSvg(Rectangle bounds, Point point, IGraphicsContext context)
        {
            return MapGraphicsContextToSvg(bounds, point, 0, context);
        }

        public SvgGraphicsContext MapGraphicsContextToSvg(Rectangle bounds, Point point, double rotation, IGraphicsContext context)
        {
            var svgContext = new SvgGraphicsContext
            {
                Coordinate = MapPoint(bounds, point, rotation),
                Fill = new SvgColourServer { Colour = ConvertColor(context.Background) },
                FillOpacity = (float)context.Background.Alpha / byte.MaxValue,
                Font = MapFont(context),
                Pen = MapPen(context),
                UnitType = UnitType,
            };

            return svgContext;
        }

        public SvgPenContext MapPen(IGraphicsContext context)
        {
            var pen = new SvgPenContext
            {
                Stroke = new SvgColourServer { Colour = ConvertColor(context.Foreground) },
                StrokeDashArray = MapLineType(context.LineType),
                StrokeLineCap = MapLineEnd(context.LineEnd),
                StrokeLineJoin = MapLineJoin(context.LineJoin),
                StrokeOpacity = (float)context.Foreground.Alpha / byte.MaxValue,
                StrokeWidth = new SvgUnit(UnitType, (float)context.LineWidth),
            };

            return pen;
        }

        public SvgCoordinateContext MapPoint(Rectangle bounds, Point point)
        {
            return MapPoint(bounds, point, 0, UnitType);
        }

        public SvgCoordinateContext MapPoint(Rectangle bounds, Point point, double rotation)
        {
            return MapPoint(bounds, point, rotation, UnitType);
        }

        public SvgCoordinateContext MapPoint(Rectangle bounds, Point point, SvgUnitType unitType)
        {
            return MapPoint(bounds, point, 0, unitType);
        }

        public SvgCoordinateContext MapPoint(Rectangle bounds, Point point, double rotation, SvgUnitType unitType)
        {
            point.Y = bounds.Height - point.Y;

            var svgStartX = new SvgUnit(unitType, (float)point.X);
            var svgStartY = new SvgUnit(unitType, (float)point.Y);
            var context = new SvgCoordinateContext
            {
                Rotation = new SvgRotate((float)-rotation, svgStartX, svgStartY),
                X = svgStartX,
                Y = svgStartY
            };

            return context;
        }

        public SvgFontContext MapFont(IGraphicsContext context)
        {
            var family = !string.IsNullOrEmpty(context.FontFamily) ? context.FontFamily : Font.FontFamily.Name;
            var font = new SvgFontContext
            {
                CharacterExpansion = (float)context.CharacterExpansion,
                Family = family,
                LineHeight = (float)context.LineHeight,
                Size = (float)context.FontSizeInPoints,
                Weight = MapFontWeight(context.FontFace)
            };

            return font;
        }

        private static SvgStrokeLineCap MapLineEnd(LineEnd lineEnd)
        {
            switch (lineEnd)
            {
                case LineEnd.Butt:
                    return SvgStrokeLineCap.Butt;
                case LineEnd.Round:
                    return SvgStrokeLineCap.Round;
                default:
                    return SvgStrokeLineCap.Square;
            }
        }

        private static SvgStrokeLineJoin MapLineJoin(LineJoin lineJoin)
        {
            switch (lineJoin)
            {
                case LineJoin.Beveled:
                    return SvgStrokeLineJoin.Bevel;
                case LineJoin.Miter:
                    return SvgStrokeLineJoin.Miter;
                default:
                    return SvgStrokeLineJoin.Round;
            }
        }

        private static SvgUnitCollection MapLineType(LineType lineType)
        {
            var results = new SvgUnitCollection();

            switch (lineType)
            {
                case LineType.Dashed:
                    results.AddRange(new[] { new SvgUnit(SvgUnitType.User, 5), new SvgUnit(SvgUnitType.User, 5) });
                    break;
                case LineType.DotDash:
                    results.AddRange(new[]
                    {
                        new SvgUnit( SvgUnitType.User, 5 ),
                        new SvgUnit( SvgUnitType.User, 5 ),
                        new SvgUnit( SvgUnitType.User, 1 ),
                        new SvgUnit( SvgUnitType.User, 5 )
                    });
                    break;
                case LineType.Dotted:
                    results.AddRange(new[] { new SvgUnit(SvgUnitType.User, 1), new SvgUnit(SvgUnitType.User, 5) });
                    break;
                case LineType.LongDash:
                    results.AddRange(new[] { new SvgUnit(SvgUnitType.User, 10), new SvgUnit(SvgUnitType.User, 5) });
                    break;
                case LineType.TwoDash:
                    results.AddRange(new[] { new SvgUnit(SvgUnitType.User, 1), new SvgUnit(SvgUnitType.User, 5) });
                    break;
            }

            return results;
        }

        private static SvgFontWeight MapFontWeight(FontFace face)
        {
            switch (face)
            {
                case FontFace.Bold:
                    return SvgFontWeight.bold;
                default:
                    return SvgFontWeight.normal;
            }
        }

        private static Color ConvertColor(R.Adapter.Graphics.Color color)
        {
            return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }
    }
}
