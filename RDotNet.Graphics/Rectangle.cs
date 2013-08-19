using System;

namespace RDotNet.Graphics
{
   public struct Rectangle : IEquatable<Rectangle>
   {
      private double height;
      private double width;
      private double x;
      private double y;

      public Rectangle(double x, double y, double width, double height)
      {
         this.x = x;
         this.y = y;
         this.width = width;
         this.height = height;
      }

      public Rectangle(Point location, Size size)
      {
         this.x = location.X;
         this.y = location.Y;
         this.width = size.Width;
         this.height = size.Height;
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

      public double Left
      {
         get { return X; }
      }

      public double Right
      {
         get { return X + Width; }
      }

      public double Bottom
      {
         get { return Y; }
      }

      public double Top
      {
         get { return Y + Height; }
      }

      public Point Location
      {
         get { return new Point(X, Y); }
         set
         {
            X = value.X;
            Y = value.Y;
         }
      }

      public Size Size
      {
         get { return new Size(Width, Height); }
         set
         {
            Width = value.Width;
            Height = value.Height;
         }
      }

      #region IEquatable<Rectangle> Members

      public bool Equals(Rectangle other)
      {
         return (this == other);
      }

      #endregion IEquatable<Rectangle> Members

      public static bool operator ==(Rectangle r1, Rectangle r2)
      {
         return r1.Location == r2.Location && r1.Size == r2.Size;
      }

      public static bool operator !=(Rectangle r1, Rectangle r2)
      {
         return !(r1 == r2);
      }

      public override int GetHashCode()
      {
         return Location.GetHashCode() ^ Size.GetHashCode();
      }

      public override bool Equals(object obj)
      {
         if (obj is Rectangle)
         {
            var rectangle = (Rectangle)obj;
            return (this == rectangle);
         }
         return false;
      }
   }
}