using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
	/// <summary>
	/// A data frame.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class DataFrame : Vector<DynamicVector>
	{
		private const string RRowNamesSymbolName = "R_RowNamesSymbol";

		/// <summary>
		/// Gets or sets the column at the specified index as a vector.
		/// </summary>
		/// <param name="columnIndex">The zero-based index of the column to get or set.</param>
		/// <returns>The column at the specified index.</returns>
		public override DynamicVector this[int columnIndex]
		{
			get
			{
				if (columnIndex < 0 || Length <= columnIndex)
				{
					throw new ArgumentOutOfRangeException();
				}
				using (new ProtectedPointer(this))
				{
					int offset = GetOffset(columnIndex);
					IntPtr pointer = Marshal.ReadIntPtr(DataPointer, offset);
					return new DynamicVector(Engine, pointer);
				}
			}
			set
			{
				if (columnIndex < 0 || Length <= columnIndex)
				{
					throw new ArgumentOutOfRangeException();
				}
				using (new ProtectedPointer(this))
				{
					int offset = GetOffset(columnIndex);
					Marshal.WriteIntPtr(DataPointer, offset, (IntPtr)(value ?? Engine.NilValue));
				}
			}
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="rowIndex">The row index.</param>
		/// <param name="columnIndex">The column index.</param>
		/// <returns>The element.</returns>
		public object this[int rowIndex, int columnIndex]
		{
			get
			{
				DynamicVector column = this[columnIndex];
				return column[rowIndex];
			}
			set
			{
				DynamicVector column = this[columnIndex];
				column[rowIndex] = value;
			}
		}

		/// <summary>
		/// Gets the number of data sets.
		/// </summary>
		public int RowCount
		{
			get
			{
				return ColumnCount == 0 ? 0 : this[0].Length;
			}
		}

		/// <summary>
		/// Gets the number of kinds of data.
		/// </summary>
		public int ColumnCount
		{
			get
			{
				return Length;
			}
		}

		/// <summary>
		/// Gets the names of rows.
		/// </summary>
		public string[] RowNames
		{
			get
			{
				SymbolicExpression rowNamesSymbol = Engine.CallPredefinedExpression(RRowNamesSymbolName);
				SymbolicExpression rowNames = GetAttribute(rowNamesSymbol);
				if (rowNames == null)
				{
					return null;
				}
				CharacterVector rowNamesVector = rowNames.AsCharacter();
				if (rowNamesVector == null)
				{
					return null;
				}

				int length = rowNamesVector.Length;
				string[] result = new string[length];
				rowNamesVector.CopyTo(result, length);
				return result;
			}
		}

		/// <summary>
		/// Gets the names of columns.
		/// </summary>
		public string[] ColumnNames
		{
			get
			{
				return Names;
			}
		}

		protected override int DataSize
		{
			get
			{
				return Marshal.SizeOf(typeof(IntPtr));
			}
		}

		internal protected DataFrame(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}

		public DataFrameRow GetRow(int rowIndex)
		{
			return new DataFrameRow(this, rowIndex);
		}

		public IEnumerable<DataFrameRow> GetRows()
		{
			int rowCount = RowCount;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
			{
				yield return GetRow(rowIndex);
			}
		}
	}
}
