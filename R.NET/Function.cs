using System;
using System.Collections.Generic;
using RDotNet.Internals;
using System.Linq;


namespace RDotNet
{
   /// <summary>
   /// A function is one of closure, built-in function, or special function.
   /// </summary>
   public abstract class Function : SymbolicExpression
   {
      /// <summary>
      /// Creates a function object.
      /// </summary>
      /// <param name="engine">The engine.</param>
      /// <param name="pointer">The pointer.</param>
      protected Function(REngine engine, IntPtr pointer)
         : base(engine, pointer)
      { }

      /// <summary>
      /// Executes the function. Match the function arguments by order.
      /// </summary>
      /// <param name="args">The arguments.</param>
      /// <returns>The result of the function evaluation</returns>
      public abstract SymbolicExpression Invoke(params SymbolicExpression[] args);

      /// <summary>
      /// Executes the function. Match the function arguments by name.
      /// </summary>
      /// <param name="args">The arguments, indexed by argument name</param>
      /// <returns>The result of the function evaluation</returns>
      public abstract SymbolicExpression Invoke(IDictionary<string, SymbolicExpression> args);

      /// <summary>
      /// Executes the function. Match the function arguments by name.
      /// </summary>
      /// <param name="args">one or more tuples, conceptually a pairlist of arguments. The argument names must be unique</param>
      /// <returns>The result of the function evaluation</returns>
      public SymbolicExpression InvokeNamed(params Tuple<string, SymbolicExpression>[] args)
      {
         return Invoke(args.ToDictionary(a => a.Item1, a => a.Item2));
      }

      protected SymbolicExpression InvokeSpecialFunction(SymbolicExpression[] args)
      {
         IntPtr argument = Engine.NilValue.DangerousGetHandle();
         foreach (SymbolicExpression arg in args.Reverse())
         {
            argument = Engine.GetFunction<Rf_cons>()(arg.DangerousGetHandle(), argument);
         }
         IntPtr call = Engine.GetFunction<Rf_lcons>()(handle, argument);

         IntPtr result = Engine.GetFunction<Rf_eval>()(call, Engine.GlobalEnvironment.DangerousGetHandle());
         return new SymbolicExpression(Engine, result);
      }


   }
}