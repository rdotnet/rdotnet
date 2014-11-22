using System.Collections.Generic;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class IntegerVector : Vector<int>, IIntegerVector
   {
      public IntegerVector(int length, IRObjectAPI api)
         : base(length, SymbolicExpressionType.IntegerVector, api)
      { }

      public IntegerVector(IList<int> vector, IRObjectAPI api)
         : base(vector, SymbolicExpressionType.IntegerVector, api)
      { }

       public IntegerVector(IRSafeHandle handle)
           : base(handle, SymbolicExpressionType.IntegerVector)
       {
           CoerceVector(SymbolicExpressionType.IntegerVector);
       }
   }
}
