using System;
using System.Runtime.InteropServices;

namespace RDotNet.Graphics
{
	/// <summary>
	/// 32-bit color of ABGR model.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Color : IEquatable<Color>
	{
		private byte red;
		private byte green;
		private byte blue;
		private byte alpha;

		/// <summary>
		/// Gets and sets the alpha value.
		/// </summary>
		public byte Alpha
		{
			get { return this.alpha; }
			set { this.alpha = value; }
		}

		/// <summary>
		/// Gets the opaque value.
		/// </summary>
		public byte Opaque
		{
			get { return (byte)(byte.MaxValue - this.alpha); }
		}

		/// <summary>
		/// Gets and sets the red value.
		/// </summary>
		public byte Red
		{
			get { return this.red; }
			set { this.red = value; }
		}

		/// <summary>
		/// Gets and sets the green value.
		/// </summary>
		public byte Green
		{
			get { return this.green; }
			set { this.green = value; }
		}

		/// <summary>
		/// Gets and sets the blue value.
		/// </summary>
		public byte Blue
		{
			get { return this.blue; }
			set { this.blue = value; }
		}

		/// <summary>
		/// Gets whether the point is transparent.
		/// </summary>
		public bool IsTransparent
		{
			get { return this.alpha == 0; }
		}

		/// <summary>
		/// Gets a color from 32-bit value.
		/// </summary>
		/// <param name="rgba">UInt32.</param>
		/// <returns>The color.</returns>
		public static Color FromUInt32(uint rgba)
		{
			var color = new Color();
			color.alpha = (byte)((rgba & 0xFF000000u) >> 24);
			color.blue = (byte)((rgba & 0x00FF0000u) >> 16);
			color.green = (byte)((rgba & 0x0000FF00u) >> 8);
			color.red = (byte)(rgba & 0x000000FFu);
			return color;
		}

		/// <summary>
		/// Gets a color from bytes.
		/// </summary>
		/// <param name="red">Red.</param>
		/// <param name="green">Green.</param>
		/// <param name="blue">Blue.</param>
		/// <returns>The color.</returns>
		public static Color FromRgb(byte red, byte green, byte blue)
		{
			var color = new Color();
			color.alpha = byte.MaxValue;
			color.blue = blue;
			color.green = green;
			color.red = red;
			return color;
		}

		/// <summary>
		/// Gets a color from bytes.
		/// </summary>
		/// <param name="alpha">Alpha.</param>
		/// <param name="red">Red.</param>
		/// <param name="green">Green.</param>
		/// <param name="blue">Blue.</param>
		/// <returns>The color.</returns>
		public static Color FromArgb(byte alpha, byte red, byte green, byte blue)
		{
			var color = new Color();
			color.alpha = alpha;
			color.blue = blue;
			color.green = green;
			color.red = red;
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
