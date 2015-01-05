using System;
using System.Diagnostics;

namespace RDotNet.Diagnostics
{
    [DebuggerDisplay("{Display,nq}")]
    internal class DataFrameColumnDisplay
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DataFrame data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int columnIndex;

        public DataFrameColumnDisplay(DataFrame data, int columnIndex)
        {
            this.data = data;
            this.columnIndex = columnIndex;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Value
        {
            get
            {
                var column = this.data[this.columnIndex];
                return column.IsFactor() ? column.AsFactor().GetFactors() : column.ToArray();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Display
        {
            get
            {
                var column = this.data[this.columnIndex];
                var names = this.data.ColumnNames;
                if (names == null || names[this.columnIndex] == null)
                {
                    return String.Format("NA ({0})", column.Type);
                }
                else
                {
                    return String.Format("\"{0}\" ({1})", names[this.columnIndex], column.Type);
                }
            }
        }
    }
}