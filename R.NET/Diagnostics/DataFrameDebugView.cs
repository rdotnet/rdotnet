using System;
using System.Diagnostics;
using System.Linq;

namespace RDotNet.Diagnostics
{
   internal class DataFrameDebugView
   {
      private readonly DataFrame dataFrame;

      public DataFrameDebugView(DataFrame dataFrame)
      {
         this.dataFrame = dataFrame;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public NotQuotedStringDisplay[] Column
      {
         get
         {
            var result = from c in this.dataFrame.ColumnNames
                         let value = String.Format("\"{0}\" ({1})", c, this.dataFrame[c].Type)
                         select new NotQuotedStringDisplay { Value = value };
            return result.ToArray();
         }
      }
   }
}
