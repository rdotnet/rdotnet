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
      public DataFrameColumnDisplay[] Column
      {
         get
         {
            return Enumerable.Range(0, this.dataFrame.ColumnCount)
               .Select(column => new DataFrameColumnDisplay(this.dataFrame, column))
               .ToArray();
         }
      }
   }
}