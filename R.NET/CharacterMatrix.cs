using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A matrix of strings.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class CharacterMatrix : Matrix<string>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
		/// <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override string this[int rowIndex, int columnIndex]
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
					IntPtr pointer = Marshal.ReadIntPtr(DataPointer, offset);
					return new InternalString(Engine, pointer);
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
					SymbolicExpression s = value == null ? Engine.NilValue : new InternalString(Engine, value);
					using (new ProtectedPointer(s))
					{
						Marshal.WriteIntPtr(DataPointer, offset, (IntPtr)s);
					}
				}
			}
		}

		/// <summary>
		/// Gets the size of a pointer in byte.
		/// </summary>
		protected override int DataSize
		{
			get
			{
				return Marshal.SizeOf(typeof(IntPtr));
			}
		}

		/// <summary>
		/// Creates a new empty CharacterMatrix with the specified size.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="rowCount">The row size.</param>
		/// <param name="columnCount">The column size.</param>
		/// <seealso cref="REngineExtension.CreateCharacterMatrix(REngine, int, int)"/>
		public CharacterMatrix(REngine engine, int rowCount, int columnCount)
			: base(engine, SymbolicExpressionType.CharacterVector, rowCount, columnCount)
		{
		}

		/// <summary>
		/// Creates a new CharacterMatrix with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="matrix">The values.</param>
		/// <seealso cref="REngineExtension.CreateCharacterMatrix(REngine, string[,])"/>
		public CharacterMatrix(REngine engine, string[, ] matrix)
			: base(engine, SymbolicExpressionType.CharacterVector, matrix)
		{
		}

		/// <summary>
		/// Creates a new instance for a string matrix.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="coerced">The pointer to a string matrix.</param>
		internal protected CharacterMatrix(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}
	}
}
