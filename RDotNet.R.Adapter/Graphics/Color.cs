using System;
using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter.Graphics
{
   [StructLayout(LayoutKind.Sequential)]
   public struct Color : IEquatable<Color>
   {
      private byte red;
      private byte green;
      private byte blue;
      private byte alpha;

      public byte Alpha
      {
         get { return alpha; }
         set { alpha = value; }
      }

      public byte Opaque
      {
         get { return (byte)(byte.MaxValue - alpha); }
      }

      public byte Red
      {
         get { return red; }
         set { red = value; }
      }

      public byte Green
      {
         get { return green; }
         set { green = value; }
      }

      public byte Blue
      {
         get { return blue; }
         set { blue = value; }
      }

      public bool IsTransparent
      {
         get { return alpha == 0; }
      }

      public static Color FromUInt32(uint rgba)
      {
         var color = new Color
         {
             alpha = (byte) ((rgba & 0xFF000000u) >> 24),
             blue = (byte) ((rgba & 0x00FF0000u) >> 16),
             green = (byte) ((rgba & 0x0000FF00u) >> 8),
             red = (byte) (rgba & 0x000000FFu)
         };
          return color;
      }

      public static Color FromRgb(byte red, byte green, byte blue)
      {
         var color = new Color {alpha = byte.MaxValue, blue = blue, green = green, red = red};
          return color;
      }

      public static Color FromArgb(byte alpha, byte red, byte green, byte blue)
      {
         var color = new Color {alpha = alpha, blue = blue, green = green, red = red};
          return color;
      }

      public static bool operator ==(Color c1, Color c2)
      {
         return c1.Alpha == c2.Alpha && c1.Blue == c2.Blue && c1.Green == c2.Green && c1.Red == c2.Red;
      }

      public static bool operator !=(Color c1, Color c2)
      {
         return !(c1 == c2);
      }

      public override int GetHashCode()
      {
         return (Alpha << 24) | (Blue << 16) | (Green << 8) | Red;
      }

      public override bool Equals(object obj)
      {
         if (obj is Color)
         {
            var color = (Color)obj;
            return (this == color);
         }
         return false;
      }

      public bool Equals(Color other)
      {
         return (this == other);
      }
   }
}
