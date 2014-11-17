using System.Collections.Generic;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class LogicalVector : Vector<bool>, ILogicalVector
   {
      public LogicalVector(int length, IRObjectAPI api)
         : base(length, SymbolicExpressionType.LogicalVector, api)
      { }

      public LogicalVector(IList<bool> vector, IRObjectAPI api)
         : base(vector, SymbolicExpressionType.LogicalVector, api)
      { }

       public LogicalVector(IRSafeHandle handle)
           : base(handle, SymbolicExpressionType.LogicalVector)
       {
           CoerceVector(SymbolicExpressionType.LogicalVector);
       }
   }
}
