using System.Collections.Generic;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class ExpressionVector : Vector<Expression>
   {
      public ExpressionVector(int length, IRObjectAPI api)
         : base(length, SymbolicExpressionType.ExpressionVector, api)
      { }

      public ExpressionVector(IList<Expression> list, IRObjectAPI api)
         : base(list, SymbolicExpressionType.ExpressionVector, api)
      { }

       public ExpressionVector(IRSafeHandle handle)
           : base(handle, SymbolicExpressionType.ExpressionVector)
       {
           CoerceVector(SymbolicExpressionType.ExpressionVector);
       }

       public SymbolicExpression EvaulateExpressionAt(int index)
       {
           SymbolicExpression evaluated;
           var result = TryEvaulateExpressionAt(index, out evaluated);
           if (!result) this[index].ThrowWithLastError();

           return evaluated;
       }

       public bool TryEvaulateExpressionAt(int index, out SymbolicExpression evaluated)
       {
           var expression = this[index];
           var result = expression.TryEvaluate(GetGlobalEnvironment(), out evaluated);
           return result;
       }
   }
}
