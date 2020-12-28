namespace RDotNet.Graphics
{
    public enum LineEnd
    {
        Round = 1,
        Butt = 2,
        Square = 3,
    }

    public enum LineJoin
    {
        Round = 1,
        Miter = 2,
        Beveled = 3,
    }

    public enum LineType
    {
        Blank = -1,
        Solid = 0,
        Dashed = 4 + (4 << 4),
        Dotted = 1 + (3 << 4),
        DotDash = 1 + (3 << 4) + (4 << 8) + (3 << 12),
        LongDash = 7 + (3 << 4),
        TwoDash = 2 + (2 << 4) + (6 << 8) + (2 << 12),
    }

    public enum Unit
    {
        Device = 0,
        NormalizedDeviceCoordinates = 1,
        Inches = 2,
        Centimeters = 3,
    }

    public enum Adjustment
    {
        None = 0,
        Half = 1,
        All = 2,
    }

    public enum FontFace
    {
        Plain = 1,
        Bold = 2,
        Italic = 3,
        BoldItalic = 4,
    }
}