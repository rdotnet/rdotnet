using System.Diagnostics;
using System.Linq;

namespace RDotNet.Client.Diagnostics
{
   internal class FactorDebugView
   {
      private readonly Factor _factor;

      public FactorDebugView(Factor factor)
      {
         _factor = factor;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public string[] Value
      {
         get
         {
            return _factor.GetFactors().ToArray();
         }
      }
   }
}
