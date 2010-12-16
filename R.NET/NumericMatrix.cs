using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A matrix of real numbers in double precision.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class NumericMatrix : Matrix<double>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
		/// <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override double this[int rowIndex, int columnIndex]
		{
			get
			{
				if (rowIndex < 0 || RowCount <= rowIndex)
				{
					throw new ArgumentOutOfRangeException("rowIndex");
				}
				if (columnIndex < 0 || ColumnCount <= columnIndex)
				{
					throw new ArgumentOutOfRangeException("columnIndex");
				}
				using (new ProtectedPointer(this))
				{
					byte[] data = new byte[DataSize];
					IntPtr pointer = DataPointer;
					int offset = GetOffset(rowIndex, columnIndex);
					for (int byteIndex = 0; byteIndex < data.Length; byteIndex++)
					{
						data[byteIndex] = Marshal.ReadByte(pointer, offset + byteIndex);
					}
					return BitConverter.ToDouble(data, 0);
				}
			}
			set
			{
				if (rowIndex < 0 || RowCount <= rowIndex)
				{
					throw new ArgumentOutOfRangeException("rowIndex");
				}
				if (columnIndex < 0 || ColumnCount <= columnIndex)
				{
					throw new ArgumentOutOfRangeException("columnIndex");
				}
				using (new ProtectedPointer(this))
				{
					byte[] data = BitConverter.GetBytes(value);
					IntPtr pointer = DataPointer;
					int offset = GetOffset(rowIndex, columnIndex);
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
		/// Creates a new NumericMatrix with the specified size.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="rowCount">The row size.</param>
		/// <param name="columnCount">The column size.</param>
		public NumericMatrix(REngine engine, int rowCount, int columnCount)
			: base(engine, SymbolicExpressionType.NumericVector, rowCount, columnCount)
		{
		}

		/// <summary>
		/// Creates a new NumericMatrix with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="matrix">The values.</param>
		public NumericMatrix(REngine engine, double[, ] matrix)
			: base(engine, SymbolicExpressionType.NumericVector, matrix)
		{
		}

		internal protected NumericMatrix(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}
	}
}
