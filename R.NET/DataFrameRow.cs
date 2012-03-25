using System;
using System.Collections.Generic;
using System.Dynamic;

namespace RDotNet
{
	/// <summary>
	/// A data frame row.
	/// </summary>
	public class DataFrameRow : DynamicObject
	{
		private DataFrame frame;
		private int rowIndex;

		public DataFrameRow(DataFrame frame, int rowIndex)
		{
			this.frame = frame;
			this.rowIndex = rowIndex;
		}

		/// <summary>
		/// Gets and sets the value at the specified column.
		/// </summary>
		/// <param name="index">The column index.</param>
		/// <returns>The value.</returns>
		public object this[int index]
		{
			get
			{
				DynamicVector column = DataFrame[index];
				return column[RowIndex];
			}
			set
			{
				DynamicVector column = DataFrame[index];
				column[RowIndex] = value;
			}
		}

		/// <summary>
		/// Gets and sets the value at the specified column.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>The value.</returns>
		public object this[string name]
		{
			get
			{
				DynamicVector column = DataFrame[name];
				return column[RowIndex];
			}
			set
			{
				DynamicVector column = DataFrame[name];
				column[RowIndex] = value;
			}
		}

		/// <summary>
		/// Gets the data frame containing this row.
		/// </summary>
		public DataFrame DataFrame
		{
			get { return this.frame; }
		}

		/// <summary>
		/// Gets the index of this row.
		/// </summary>
		public int RowIndex
		{
			get { return this.rowIndex; }
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return DataFrame.ColumnNames;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			string[] columnNames = DataFrame.ColumnNames;
			if (columnNames == null || Array.IndexOf(columnNames, binder.Name) < 0)
			{
				result = null;
				return false;
			}
			result = this[binder.Name];
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			string[] columnNames = DataFrame.ColumnNames;
			if (columnNames == null || Array.IndexOf(columnNames, binder.Name) < 0)
			{
				return false;
			}
			this[binder.Name] = value;
			return true;
		}
	}
}
