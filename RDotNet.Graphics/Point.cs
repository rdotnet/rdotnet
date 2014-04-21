using System;

namespace RDotNet.Graphics
{
   public struct Point : IEquatable<Point>
   {
      private double x;
      private double y;

      public Point(double x, double y)
      {
         this.x = x;
         this.y = y;
      }

      public double X
      {
         get { return this.x; }
         set { this.x = value; }
      }

      public double Y
      {
         get { return this.y; }
         set { this.y = value; }
      }

      #region IEquatable<Point> Members

      public bool Equals(Point other)
      {
         return (this == other);
      }

      #endregion IEquatable<Point> Members

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
         const int Prime = 31;
         return Prime * X.GetHashCode() + Y.GetHashCode();
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