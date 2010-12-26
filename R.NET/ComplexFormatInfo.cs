using System;
using System.Globalization;

namespace RDotNet
{
	public class ComplexFormatInfo : IFormatProvider, ICustomFormatter
	{
		public object GetFormat(Type formatType)
		{
			if (formatType == typeof(Complex))
			{
				return this;
			}
			return null;
		}

		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			Complex z = (Complex)arg;
			NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(formatProvider);

			if (Complex.IsNaN(z))
			{
				return numberFormat.NaNSymbol;
			}
			if (Complex.IsInfinity(z))
			{
				return numberFormat.PositiveInfinitySymbol;
			}

			char imaginaryUnitSign = 'i';
			if (format != null)
			{
				if (format.Length == 1)
				{
					char c = format[0];
					if (Char.IsLetter(c))
					{
						char upper = Char.ToUpper(c);
						if (upper != 'C' && upper != 'D' && upper != 'E' && upper != 'F' && upper != 'G' && upper != 'N' && upper != 'P' && upper != 'R' && upper != 'X')
						{
							imaginaryUnitSign = c;
							format = null;
						}
					}
				}
				else
				{
					char c = format[format.Length - 1];
					if (Char.IsLetter(c))
					{
						imaginaryUnitSign = c;
						format = format.Substring(0, format.Length - 1);
					}
				}
			}

			string realPart = FormatRealPart(z, format, formatProvider);
			if (Complex.IsReal(z))
			{
				return realPart;
			}

			string imaginaryPart = FormatImaginaryPart(z, format, formatProvider, imaginaryUnitSign);
			if (Complex.IsPurelyImaginary(z))
			{
				return imaginaryPart;
			}
			return realPart + (z.Imaginary > 0 ? numberFormat.PositiveSign : "") + imaginaryPart;
		}

		private static string FormatRealPart(Complex z, string format, IFormatProvider formatProvider)
		{
			return format == null ?
				string.Format(formatProvider, "{0}", z.Real) :
				string.Format(formatProvider, "{0:" + format + "}", z.Real);
		}

		private static string FormatImaginaryPart(Complex z, string format, IFormatProvider formatProvider, char imaginaryUnitSign)
		{
			NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(formatProvider);
			if (format == null)
			{
				if (z.Imaginary == 1)
				{
					return Char.ToString(imaginaryUnitSign);
				}
				else if (z.Imaginary == -1)
				{
					return numberFormat.NegativeSign + imaginaryUnitSign;
				}
				else
				{
					return string.Format(formatProvider, "{0}" + imaginaryUnitSign, z.Imaginary);
				}
			}
			return string.Format(formatProvider, "{0:" + format + "}" + imaginaryUnitSign, z.Imaginary);
		}
	}
}
