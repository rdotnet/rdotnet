using System;

namespace RDotNet.Graphics
{
    public struct Size : IEquatable<Size>
    {
        private double height;
        private double width;

        public Size(double width, double height)
        {
            this.width = width;
            this.height = height;
        }

        public double Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        public double Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        #region IEquatable<Size> Members

        public bool Equals(Size other)
        {
            return (this == other);
        }

        #endregion IEquatable<Size> Members

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
            return Prime * Width.GetHashCode() + Height.GetHashCode();
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