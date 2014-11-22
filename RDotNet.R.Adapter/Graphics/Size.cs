using System;

namespace RDotNet.R.Adapter.Graphics
{
    public class Size : IEquatable<Size>
    {
        public Size(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public double Width { get; set; }
        public double Height { get; set; }

        public bool Equals(Size other)
        {
            return (this == other);
        }


        public static bool operator ==(Size size1, Size size2)
        {
            return size1.Width == size2.Width && size1.Height == size2.Height;
        }

        public static bool operator !=(Size size1, Size size2)
        {
            return !(size1 == size2);
        }

        public override int GetHashCode()
        {
            const int Prime = 31;
            return Prime*Width.GetHashCode() + Height.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Size)
            {
                var size = (Size)obj;
                return (this == size);
            }
            return false;
        }
    }
}
