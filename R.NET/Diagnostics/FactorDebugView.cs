using System.Diagnostics;
using System.Linq;

namespace RDotNet.Diagnostics
{
   internal class FactorDebugView
   {
      private readonly Factor factor;

      public FactorDebugView(Factor factor)
      {
         this.factor = factor;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public string[] Value
      {
         get
         {
            return this.factor.GetFactors().ToArray();
         }
      }
   }
}
