using System.Collections.Generic;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class GenericVector : Vector<SymbolicExpression>, IGenericVector
   {
      public GenericVector(int length, IRObjectAPI api)
         : base(length, SymbolicExpressionType.List, api)
      { }

      public GenericVector(IList<SymbolicExpression> list, IRObjectAPI api)
         : base(list, SymbolicExpressionType.List, api)
      { }
   }
}
