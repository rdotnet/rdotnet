using System.Collections.Generic;
using System;

namespace RDotNet
{
   /// <summary>
   /// A special function.
   /// </summary>
   public class SpecialFunction : Function
   {
      /// <summary>
      /// Creates a special function proxy.
      /// </summary>
      /// <param name="engine">The engine.</param>
      /// <param name="pointer">The pointer.</param>
      protected internal SpecialFunction(REngine engine, IntPtr pointer)
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