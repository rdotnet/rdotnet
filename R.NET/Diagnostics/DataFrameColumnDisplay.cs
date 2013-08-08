using System;
using System.Diagnostics;
using System.Linq;

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
            var name = this.data.ColumnNames[this.columnIndex];
            return String.Format("\"{0}\" ({1})", name, column.Type);
         }
      }
   }
}
