using System;

namespace RDotNet.R.Adapter.Graphics
{
    public struct Point : IEquatable<Point>
    {
        public Point(double x, double y)
            : this()
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public bool Equals(Point other)
        {
            return (this == other);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            return prime*X.GetHashCode() + Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                var point = (Point)obj;
                return (this == point);
            }
            return false;
        }
    }
}
