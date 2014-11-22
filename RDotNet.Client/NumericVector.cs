using System.Collections.Generic;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class NumericVector : Vector<double>, INumericVector
   {
      public NumericVector(int length, IRObjectAPI api)
         : base(length, SymbolicExpressionType.NumericVector, api)
      { }

      public NumericVector(IList<double> vector, IRObjectAPI api)
         : base(vector, SymbolicExpressionType.NumericVector, api)
      { }

       public NumericVector(IRSafeHandle handle)
           : base(handle, SymbolicExpressionType.NumericVector)
       {
           CoerceVector(SymbolicExpressionType.NumericVector);
       }
   }
}
