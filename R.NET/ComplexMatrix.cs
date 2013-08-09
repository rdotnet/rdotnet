using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A matrix of complex numbers.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class ComplexMatrix : Matrix<Complex>
	{
		/// <summary>
		/// Creates a new empty ComplexMatrix with the specified size.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="rowCount">The row size.</param>
		/// <param name="columnCount">The column size.</param>
		/// <seealso cref="REngineExtension.CreateComplexMatrix(REngine, int, int)"/>
		public ComplexMatrix(REngine engine, int rowCount, int columnCount)
			: base(engine, SymbolicExpressionType.ComplexVector, rowCount, columnCount)
		{}

		/// <summary>
		/// Creates a new ComplexMatrix with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="matrix">The values.</param>
		/// <seealso cref="REngineExtension.CreateComplexMatrix(REngine, Complex[,])"/>
		public ComplexMatrix(REngine engine, Complex[,] matrix)
			: base(engine, SymbolicExpressionType.CharacterVector, matrix)
		{}

		/// <summary>
		/// Creates a new instance for a complex number matrix.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="coerced">The pointer to a complex number matrix.</param>
		protected internal ComplexMatrix(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
		/// <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override Complex this[int rowIndex, int columnIndex]
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
					var data = new double[2];
					int offset = GetOffset(rowIndex, columnIndex);
					IntPtr pointer = IntPtr.Add(DataPointer, offset);
					Marshal.Copy(pointer, data, 0, data.Length);
					return new Complex(data[0], data[1]);
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
					var data = new[] { value.Real, value.Imaginary };
					int offset = GetOffset(rowIndex, columnIndex);
					IntPtr pointer = IntPtr.Add(DataPointer, offset);
					Marshal.Copy(data, 0, pointer, data.Length);
				}
			}
		}

		/// <summary>
		/// Gets the size of a complex number in byte.
		/// </summary>
		protected override int DataSize
		{
			get { return Marshal.SizeOf(typeof(Complex)); }
		}
	}
}
