using System.Diagnostics;
using System.Linq;

namespace RDotNet.Client.Diagnostics
{
   internal class DataFrameDebugView
   {
      private readonly DataFrame _dataFrame;

      public DataFrameDebugView(DataFrame dataFrame)
      {
         _dataFrame = dataFrame;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public DataFrameColumnDisplay[] Column
      {
         get
         {
            return Enumerable.Range(0, _dataFrame.GetColumnCount())
               .Select(column => new DataFrameColumnDisplay(_dataFrame, column))
               .ToArray();
         }
      }
   }
}
