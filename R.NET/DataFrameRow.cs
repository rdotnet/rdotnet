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

        /// <summary>
        /// Creates a new object representing a data frame row
        /// </summary>
        /// <param name="frame">R Data frame</param>
        /// <param name="rowIndex">zero-based row index</param>
        public DataFrameRow(DataFrame frame, int rowIndex)
        {
            this.frame = frame;
            this.rowIndex = rowIndex;
        }

        /// <summary>
        /// Gets and sets the value at the specified column.
        /// </summary>
        /// <param name="index">The zero-based column index.</param>
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
        /// Gets the inner representation of the value; an integer if the column is a factor
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal object GetInnerValue(int index)
        {
            DynamicVector column = DataFrame[index];
            if (column.IsFactor())
                return column.AsInteger()[RowIndex];
            else
                return column[RowIndex];
        }

        /// <summary>
        /// Sets the inner representation of the value; an integer if the column is a factor
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        internal void SetInnerValue(int index, object value)
        {
            DynamicVector column = DataFrame[index];
            if (column.IsFactor())
                column.AsInteger()[RowIndex] = (int)value;
            else
                column[RowIndex] = value;
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

        /// <summary>
        /// Gets the column names of the data frame.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return DataFrame.ColumnNames;
        }

        /// <summary>
        /// Try to get a member to a specified value
        /// </summary>
        /// <param name="binder">Dynamic get member operation at the call site; Binder whose name should be one of the data frame column name</param>
        /// <param name="result">The value of the member</param>
        /// <returns>false if setting failed</returns>
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

        /// <summary>
        /// Try to set a member to a specified value
        /// </summary>
        /// <param name="binder">Dynamic set member operation at the call site; Binder whose name should be one of the data frame column name</param>
        /// <param name="value">The value to set</param>
        /// <returns>false if setting failed</returns>
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