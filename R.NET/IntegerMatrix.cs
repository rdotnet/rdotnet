using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A matrix of integers from <c>-2^31 + 1</c> to <c>2^31 - 1</c>.
	/// </summary>
	/// <remarks>
	/// The minimum value of IntegerVector is different from that of System.Int32 in .NET Framework.
	/// </remarks>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class IntegerMatrix : Matrix<int>
	{
		/// <summary>
		/// Creates a new empty IntegerMatrix with the specified size.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="rowCount">The row size.</param>
		/// <param name="columnCount">The column size.</param>
		/// <seealso cref="REngineExtension.CreateIntegerMatrix(REngine, int, int)"/>
		public IntegerMatrix(REngine engine, int rowCount, int columnCount)
			: base(engine, SymbolicExpressionType.IntegerVector, rowCount, columnCount)
		{}

		/// <summary>
		/// Creates a new IntegerMatrix with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="matrix">The values.</param>
		/// <seealso cref="REngineExtension.CreateIntegerMatrix(REngine, int[,])"/>
		public IntegerMatrix(REngine engine, int[,] matrix)
			: base(engine, SymbolicExpressionType.IntegerVector, matrix)
		{}

		/// <summary>
		/// Creates a new instance for an integer matrix.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="coerced">The pointer to an integer matrix.</param>
		protected internal IntegerMatrix(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
		/// <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override int this[int rowIndex, int columnIndex]
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
					return Marshal.ReadInt32(DataPointer, offset);
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
					Marshal.WriteInt32(DataPointer, offset, value);
				}
			}
		}

		/// <summary>
		/// Gets the size of an integer in byte.
		/// </summary>
		protected override int DataSize
		{
			get { return sizeof(int); }
		}
	}
}
