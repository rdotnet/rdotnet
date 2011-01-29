using System;
using System.Runtime.InteropServices;

namespace RDotNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Complex
	{
		private double real;
		private double imaginary;

		public static readonly Complex NaN = new Complex(double.NaN, double.NaN);
		public static readonly Complex Infinity = new Complex(double.PositiveInfinity, double.PositiveInfinity);
		public static readonly Complex Zero = new Complex(0, 0);
		public static readonly Complex Identity = new Complex(1, 0);
		public static readonly Complex ImaginaryUnit = new Complex(0, 1);

		public double Real
		{
			get
			{
				return real;
			}
		}

		public double Imaginary
		{
			get
			{
				return imaginary;
			}
		}

		public Complex(double real, double imaginary)
		{
			this.real = real;
			this.imaginary = imaginary;
		}

		public static bool IsReal(Complex z)
		{
			return z.Imaginary == 0;
		}

		public static bool IsPurelyImaginary(Complex z)
		{
			return z.Real == 0 && z.Imaginary != 0;
		}

		public static bool IsNaN(Complex z)
		{
			return double.IsNaN(z.Real) || double.IsNaN(z.Imaginary);
		}

		public static bool IsInfinity(Complex z)
		{
			return double.IsInfinity(z.Real) || double.IsInfinity(z.Imaginary);
		}

		public static implicit operator Complex(double real)
		{
			return new Complex(real, 0);
		}

		public static Complex operator +(Complex z)
		{
			return z;
		}

		public static Complex operator -(Complex z)
		{
			return new Complex(-z.Real, -z.Imaginary);
		}

		public static Complex operator ~(Complex z)
		{
			return new Complex(z.Real, -z.Imaginary);
		}

		public static Complex operator +(Complex z1, Complex z2)
		{
			if (IsNaN(z1) || IsNaN(z2))
			{
				return NaN;
			}

			double real = z1.Real + z2.Real;
			double imaginary = z1.Imaginary + z2.Imaginary;
			return new Complex(real, imaginary);
		}

		public static Complex operator -(Complex z1, Complex z2)
		{
			if (IsNaN(z1) || IsNaN(z2))
			{
				return NaN;
			}

			double real = z1.Real - z2.Real;
			double imaginary = z1.Imaginary - z2.Imaginary;
			return new Complex(real, imaginary);
		}

		public static Complex operator *(Complex z1, Complex z2)
		{
			if (IsNaN(z1) || IsNaN(z2))
			{
				return NaN;
			}

			double real = z1.Real * z2.Real - z1.Imaginary * z2.Imaginary;
			double imaginary = z1.Real * z2.Imaginary + z1.Imaginary * z2.Real;
			return new Complex(real, imaginary);
		}

		public static Complex operator /(Complex z1, Complex z2)
		{
			if (z2 == 0)
			{
				return NaN;
			}
			if (!IsInfinity(z1) && IsInfinity(z2))
			{
				return 0;
			}

			double denominator = z2.Real * z2.Real + z2.Imaginary * z2.Imaginary;
			double realNumerator = z1.Real * z2.Real + z1.Imaginary * z2.Imaginary;
			double imaginaryNumerator = z1.Imaginary * z2.Real - z1.Real * z2.Imaginary;
			return new Complex(realNumerator / denominator, imaginaryNumerator / denominator);
		}

		public static bool operator ==(Complex z1, Complex z2)
		{
			return z1.Real == z2.Real && z1.Imaginary == z2.Imaginary;
		}

		public static bool operator !=(Complex z1, Complex z2)
		{
			return z1.Real != z2.Real || z1.Imaginary != z2.Imaginary;
		}

		public bool Equals(Complex other)
		{
			return this == other || (IsNaN(this) && IsNaN(other));
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, this))
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}

			try
			{
				Complex z = (Complex)obj;
				return this == z || (IsNaN(this) && IsNaN(z));
			}
			catch
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			if (IsNaN(this))
			{
				return 5;
			}
			const int Prime1 = 37;
			const int Prime2 = 13;
			return Prime1 * (Prime2 * Real.GetHashCode() + Imaginary.GetHashCode());
		}

		public override string ToString()
		{
			return ToString(null, null);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			ICustomFormatter formatter = null;
			if (formatProvider != null)
			{
				formatter = formatProvider.GetFormat(typeof(Complex)) as ICustomFormatter;
			}
			if (formatter == null)
			{
				formatter = new ComplexFormatInfo();
			}

			return formatter.Format(format, this, formatProvider);
		}

	}
}
