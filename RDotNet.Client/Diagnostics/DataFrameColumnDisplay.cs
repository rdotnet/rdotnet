using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RDotNet.Client.Diagnostics
{
   [DebuggerDisplay("{Display,nq}")]
   internal class DataFrameColumnDisplay
   {
      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      private readonly DataFrame _data;

      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      private readonly int _columnIndex;

      public DataFrameColumnDisplay(DataFrame data, int columnIndex)
      {
         _data = data;
         _columnIndex = columnIndex;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public IList<object> Value
      {
         get
         {
            var column = _data[_columnIndex];
            return column.IsFactor() ? (IList<object>)column.ToFactor().GetFactors() : column.ToList();
         }
      }

      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      public string Display
      {
         get
         {
            var column = _data[_columnIndex];
            var names = _data.GetColumnNames();
            if (names == null || names[_columnIndex] == null)
            {
               return String.Format("NA ({0})", column.Type);
            }
            
             return String.Format("\"{0}\" ({1})", names[_columnIndex], column.Type);
         }
      }
   }
}
