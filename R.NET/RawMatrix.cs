using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A matrix of byte values.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class RawMatrix : Matrix<byte>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
		/// <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override byte this[int rowIndex, int columnIndex]
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
					int offset = GetOffset(rowIndex, columnIndex);
					return Marshal.ReadByte(DataPointer, offset);
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
					int offset = GetOffset(rowIndex, columnIndex);
					Marshal.WriteByte(DataPointer, offset, value);
				}
			}
		}

		/// <summary>
		/// Gets the size of an Raw in byte.
		/// </summary>
		protected override int DataSize
		{
			get
			{
				return sizeof(byte);
			}
		}

		/// <summary>
		/// Creates a new RawMatrix with the specified size.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="rowCount">The row size.</param>
		/// <param name="columnCount">The column size.</param>
		public RawMatrix(REngine engine, int rowCount, int columnCount)
			: base(engine, SymbolicExpressionType.RawVector, rowCount, columnCount)
		{
		}

		/// <summary>
		/// Creates a new RawMatrix with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="matrix">The values.</param>
		public RawMatrix(REngine engine, byte[, ] matrix)
			: base(engine, SymbolicExpressionType.RawVector, matrix)
		{
		}

		internal protected RawMatrix(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}
	}
}
