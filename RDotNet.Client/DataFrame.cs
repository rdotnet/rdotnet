using RDotNet.Client.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
    [DebuggerDisplay(@"ColumnCount = {ColumnCount}; RowCount = {RowCount}; RObjectType = {Type}")]
    [DebuggerTypeProxy(typeof (DataFrameDebugView))]
    public class DataFrame : Vector<DynamicVector>, IDataFrame
    {
        public DataFrame(IRSafeHandle handle)
            : base(handle, SymbolicExpressionType.List)
        { }

        public object this[int rowIndex, int columnIndex]
        {
            get
            {
                var column = this[columnIndex];
                return column[rowIndex];
            }
            set
            {
                var column = this[columnIndex];
                column[rowIndex] = value;
            }
        }

        public object this[int rowIndex, string columnName]
        {
            get
            {
                var column = this[columnName];
                return column[rowIndex];
            }
            set
            {
                var column = this[columnName];
                column[rowIndex] = value;
            }
        }

        public object this[string rowName, string columnName]
        {
            get
            {
                var column = this[columnName];
                return column[rowName];
            }
            set
            {
                var column = this[columnName];
                column[rowName] = value;
            }
        }

        public int GetRowCount()
        {
            return GetNumberOfRows();
        }

        public int GetColumnCount()
        {
            return GetNumberOfColumns();
        }

        public int GetItemCount()
        {
            return GetRowCount()*GetColumnCount();
        }

        public IList<string> GetRowNames()
        {
            var names = GetDimensions();
            if (names == null) return new List<string>();

            var rowNames = names[0];
            if (rowNames == null) return new List<string>();

            var result = rowNames.ToCharacterVector().ToList();
            return result;
        }

        public IList<string> GetColumnNames()
        {
            var names = GetDimensions();
            if (names == null) return new List<string>();

            var columnNames = names[1];
            if (columnNames == null) return new List<string>();

            var result = columnNames.ToCharacterVector().ToList();
            return result;
        }

        private GenericVector GetDimensions()
        {
            var dimnamesSymbol = GetDimNames();
            var dimnames = GetAttribute(dimnamesSymbol);
            if (dimnames == null) return null;

            return dimnames.ToGenericVector();
        }

        public DataFrameRow GetRow(int rowIndex)
        {
            return new DataFrameRow(this, rowIndex);
        }

        public TRow GetRow<TRow>(int rowIndex) where TRow : class, new()
        {
            var rowType = typeof (TRow);
            var attribute = (DataFrameRowAttribute)rowType.GetCustomAttributes(typeof (DataFrameRowAttribute), false).Single();
            if (attribute == null) throw new ArgumentException("DataFrameRowAttribute is required.");
            var row = GetRow(rowIndex);
            return attribute.Convert<TRow>(row);
        }

        public IEnumerable<DataFrameRow> GetRows()
        {
            int rowCount = GetRowCount();
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                yield return GetRow(rowIndex);
            }
        }

        public IEnumerable<TRow> GetRows<TRow>() where TRow : class, new()
        {
            var rowType = typeof (TRow);
            var attribute = (DataFrameRowAttribute)rowType.GetCustomAttributes(typeof (DataFrameRowAttribute), false).Single();
            if (attribute == null) throw new ArgumentException("DataFrameRowAttribute is required.");

            int rowCount = GetRowCount();
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var row = GetRow(rowIndex);
                yield return attribute.Convert<TRow>(row);
            }
        }
    }
}
