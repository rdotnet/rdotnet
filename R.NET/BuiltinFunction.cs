using System.Collections.Generic;
using System;

namespace RDotNet
{
   /// <summary>
   /// A built-in function.
   /// </summary>
   public class BuiltinFunction : Function
   {
      /// <summary>
      /// Creates a built-in function proxy.
      /// </summary>
      /// <param name="engine">The engine.</param>
      /// <param name="pointer">The pointer.</param>
      protected internal BuiltinFunction(REngine engine, IntPtr pointer)
         : base(engine, pointer)
      { }

      public override SymbolicExpression Invoke(params SymbolicExpression[] args)
      {
         return InvokeSpecialFunction(args);
      }

      public override SymbolicExpression Invoke(IDictionary<string, SymbolicExpression> args)
      {
         throw new NotImplementedException();
      }
   }
}