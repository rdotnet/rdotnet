using System;
using System.Numerics;
using System.Runtime.InteropServices;
using RDotNet.Internals;

namespace RDotNet
{
	public class DynamicVector : Vector<object>
	{
		public override object this[int index]
		{
			get
			{
				if (index < 0 || Length <= index)
				{
					throw new ArgumentOutOfRangeException();
				}
				using (new ProtectedPointer(this))
				{
					IntPtr pointer = DataPointer;
					int offset = GetOffset(index);
					switch (Type)
					{
						case SymbolicExpressionType.NumericVector:
							return ReadDouble(pointer, offset);
						case SymbolicExpressionType.IntegerVector:
							return ReadInt32(pointer, offset);
						case SymbolicExpressionType.CharacterVector:
							return ReadString(pointer, offset);
						case SymbolicExpressionType.LogicalVector:
							return ReadBoolean(pointer, offset);
						case SymbolicExpressionType.RawVector:
							return ReadByte(pointer, offset);
						case SymbolicExpressionType.ComplexVector:
							return ReadComplex(pointer, offset);
						default:
							throw new NotImplementedException();
					}
				}
			}
			set
			{
				if (index < 0 || Length <= index)
				{
					throw new ArgumentOutOfRangeException();
				}
				using (new ProtectedPointer(this))
				{
					IntPtr pointer = DataPointer;
					int offset = GetOffset(index);
					switch (Type)
					{
						case SymbolicExpressionType.NumericVector:
							WriteDouble((double)value, pointer, offset);
							return;
						case SymbolicExpressionType.IntegerVector:
							WriteInt32((int)value, pointer, offset);
							return;
						case SymbolicExpressionType.CharacterVector:
							WriteString((string)value, pointer, offset);
							return;
						case SymbolicExpressionType.LogicalVector:
							WriteBoolean((bool)value, pointer, offset);
							return;
						case SymbolicExpressionType.RawVector:
							WriteByte((byte)value, pointer, offset);
							return;
						case SymbolicExpressionType.ComplexVector:
							WriteComplex((Complex)value, pointer, offset);
							return;
						default:
							throw new NotImplementedException();
					}
				}
			}
		}

		protected override int DataSize
		{
			get
			{
				switch (Type)
				{
					case SymbolicExpressionType.NumericVector:
						return sizeof(double);
					case SymbolicExpressionType.IntegerVector:
						return sizeof(int);
					case SymbolicExpressionType.CharacterVector:
						return Marshal.SizeOf(typeof(IntPtr));
					case SymbolicExpressionType.LogicalVector:
						return sizeof(int);
					case SymbolicExpressionType.RawVector:
						return sizeof(byte);
					case SymbolicExpressionType.ComplexVector:
						return Marshal.SizeOf(typeof(Complex));
					default:
						throw new NotImplementedException();
				}
			}
		}

		internal protected DynamicVector(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}

		private static double ReadDouble(IntPtr pointer, int offset)
		{
			byte[] data = new byte[sizeof(double)];
			for (int byteIndex = 0; byteIndex < data.Length; byteIndex++)
			{
				data[byteIndex] = Marshal.ReadByte(pointer, offset + byteIndex);
			}
			return BitConverter.ToDouble(data, 0);
		}

		private static void WriteDouble(double value, IntPtr pointer, int offset)
		{
			byte[] data = BitConverter.GetBytes(value);
			for (int byteIndex = 0; byteIndex < data.Length; byteIndex++)
			{
				Marshal.WriteByte(pointer, offset + byteIndex, data[byteIndex]);
			}
		}

		private static int ReadInt32(IntPtr pointer, int offset)
		{
			return Marshal.ReadInt32(pointer, offset);
		}

		private static void WriteInt32(int value, IntPtr pointer, int offset)
		{
			Marshal.WriteInt32(pointer, offset, value);
		}

		private static string ReadString(IntPtr pointer, int offset)
		{
			pointer = Marshal.ReadIntPtr(pointer, offset);
			pointer = IntPtr.Add(pointer, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
			return Marshal.PtrToStringAnsi(pointer);
		}

		private static void WriteString(string value, IntPtr pointer, int offset)
		{
			IntPtr stringPointer = NativeMethods.Rf_mkChar(value);
			Marshal.WriteIntPtr(pointer, offset, stringPointer);
		}

		private static bool ReadBoolean(IntPtr pointer, int offset)
		{
			int data = Marshal.ReadInt32(pointer, offset);
			return Convert.ToBoolean(data);
		}

		private static void WriteBoolean(bool value, IntPtr pointer, int offset)
		{
			int data = Convert.ToInt32(value);
			Marshal.WriteInt32(pointer, offset, data);
		}

		private static byte ReadByte(IntPtr pointer, int offset)
		{
			return Marshal.ReadByte(pointer, offset);
		}

		private static void WriteByte(byte value, IntPtr pointer, int offset)
		{
			Marshal.WriteByte(pointer, offset, value);
		}

		private static Complex ReadComplex(IntPtr pointer, int offset)
		{
			byte[] data = new byte[Marshal.SizeOf(typeof(Complex))];
			Marshal.Copy(pointer, data, 0, data.Length);
			double real = BitConverter.ToDouble(data, 0);
			double imaginary = BitConverter.ToDouble(data, sizeof(double));
			return new Complex(real, imaginary);
		}

		private static void WriteComplex(Complex value, IntPtr pointer, int offset)
		{
			byte[] real = BitConverter.GetBytes(value.Real);
			byte[] imaginary = BitConverter.GetBytes(value.Imaginary);
			Marshal.Copy(real, 0, pointer, real.Length);
			pointer = IntPtr.Add(pointer, real.Length);
			Marshal.Copy(imaginary, 0, pointer, imaginary.Length);
		}
	}
}
