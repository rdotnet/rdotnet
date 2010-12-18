using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A collection of real numbers in double precision.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class NumericVector : Vector<double>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override double this[int index]
		{
			get
			{
				if (index < 0 || Length <= index)
				{
					throw new ArgumentOutOfRangeException();
				}
				using (new ProtectedPointer(this))
				{
					byte[] data = new byte[DataSize];
					IntPtr pointer = DataPointer;
					int offset = GetOffset(index);
					for (int byteIndex = 0; byteIndex < data.Length; byteIndex++)
					{
						data[byteIndex] = Marshal.ReadByte(pointer, offset + byteIndex);
					}
					return BitConverter.ToDouble(data, 0);
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
					byte[] data = BitConverter.GetBytes(value);
					IntPtr pointer = DataPointer;
					int offset = GetOffset(index);
					for (int byteIndex = 0; byteIndex < data.Length; byteIndex++)
					{
						Marshal.WriteByte(pointer, offset + byteIndex, data[byteIndex]);
					}
				}
			}
		}

		/// <summary>
		/// Gets the size of a real number in byte.
		/// </summary>
		protected override int DataSize
		{
			get
			{
				return sizeof(double);
			}
		}

		/// <summary>
		/// Creates a new empty NumericVector with the specified length.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="length">The length.</param>
		public NumericVector(REngine engine, int length)
			: base(engine, SymbolicExpressionType.NumericVector, length)
		{
		}

		/// <summary>
		/// Creates a new NumericVector with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="vector">The values.</param>
		public NumericVector(REngine engine, double[] vector)
			: base(engine, SymbolicExpressionType.NumericVector, vector)
		{
		}

		internal protected NumericVector(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}
	}
}
