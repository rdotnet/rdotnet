using System.Collections.Generic;
using System.Numerics;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class ComplexVector : Vector<Complex>, IComplexVector
   {
      public ComplexVector(int length, IRObjectAPI api)
         : base(length, SymbolicExpressionType.ComplexVector, api)
      { }

      public ComplexVector(IList<Complex> list, IRObjectAPI api)
         : base(list, SymbolicExpressionType.ComplexVector, api)
      { }

       public ComplexVector(IRSafeHandle handle)
           : base(handle, SymbolicExpressionType.ComplexVector)
       {
           CoerceVector(SymbolicExpressionType.ComplexVector);
       }
   }
}
