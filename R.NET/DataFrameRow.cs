using System;
using System.Collections.Generic;

namespace RDotNet
{
	/// <summary>
	/// A data frame row.
	/// </summary>
	public class DataFrameRow
	{
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

		private DataFrame frame;
		/// <summary>
		/// Gets the data frame containing this row.
		/// </summary>
		public DataFrame DataFrame
		{
			get
			{
				return this.frame;
			}
		}

		private int rowIndex;
		/// <summary>
		/// Gets the index of this row.
		/// </summary>
		public int RowIndex
		{
			get
			{
				return this.rowIndex;
			}
		}

		public DataFrameRow(DataFrame frame, int rowIndex)
		{
			this.frame = frame;
			this.rowIndex = rowIndex;
		}
	}
}
