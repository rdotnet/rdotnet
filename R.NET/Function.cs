using System;
using System.Collections.Generic;
using RDotNet.Internals;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;


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
      /// A convenience method to executes the function. Match the function arguments by order, after evaluating each to an R expression.
      /// </summary>
      /// <param name="args">string representation of the arguments; each is evaluated to symbolic expression before being passed as argument to this object (i.e. this Function)</param>
      /// <returns>The result of the function evaluation</returns>
      /// <example>
      /// <code>
      /// </code>
      /// </example>
      public SymbolicExpression InvokeStrArgs(params string[] args)
      {
         return Invoke(Array.ConvertAll(args, x => Engine.Evaluate(x)));
      }

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
         return InvokeViaPairlist(Array.ConvertAll(args, x => x.Item1), Array.ConvertAll(args, x => x.Item2));
      }

      /// <summary>
      /// Executes the function. Match the function arguments by name.
      /// </summary>
      /// <param name="argNames">The names of the arguments. These can be empty strings for unnamed function arguments</param>
      /// <param name="args">The arguments passed to the function</param>
      /// <returns></returns>
      protected SymbolicExpression InvokeViaPairlist(string[] argNames, SymbolicExpression[] args)
      {
         var names = new CharacterVector(Engine, argNames);
         var arguments = new GenericVector(Engine, args);
         arguments.SetAttribute(Engine.GetPredefinedSymbol("R_NamesSymbol"), names);
         var argPairList = arguments.ToPairlist();

         //IntPtr newEnvironment = Engine.GetFunction<Rf_allocSExp>()(SymbolicExpressionType.Environment);
         //IntPtr result = Engine.GetFunction<Rf_applyClosure>()(Body.DangerousGetHandle(), handle,
         //                                                      argPairList.DangerousGetHandle(),
         //                                                      Environment.DangerousGetHandle(), newEnvironment);
         IntPtr call = Engine.GetFunction<Rf_lcons>()(handle, argPairList.DangerousGetHandle());
         IntPtr result = evaluateCall(call);

         return new SymbolicExpression(Engine, result);
      }

      // http://msdn.microsoft.com/en-us/magazine/dd419661.aspx
      [HandleProcessCorruptedStateExceptions]
      [SecurityCritical]
      private IntPtr evaluateCall(IntPtr call)
      {
         IntPtr result;
         bool errorOccurred = false;
         try
         {
            result = Engine.GetFunction<R_tryEval>()(call, Engine.GlobalEnvironment.DangerousGetHandle(), out errorOccurred);
         }
         catch (Exception ex) // TODO: this is usually dubious to catch all that, but given the inner exception is preserved
         {
            throw new EvaluationException(Engine.LastErrorMessage, ex);
         }
         if (errorOccurred)
            throw new EvaluationException(Engine.LastErrorMessage);
         return result;
      }

      /// <summary>
      /// Invoke this function with unnamed arguments.
      /// </summary>
      /// <param name="args">The arguments passed to function call.</param>
      /// <returns>The result of the function evaluation.</returns>
      protected SymbolicExpression InvokeOrderedArguments(SymbolicExpression[] args)
      {
         IntPtr argument = Engine.NilValue.DangerousGetHandle();
         foreach (SymbolicExpression arg in args.Reverse())
         {
            argument = Engine.GetFunction<Rf_cons>()(arg.DangerousGetHandle(), argument);
         }
         IntPtr call = Engine.GetFunction<Rf_lcons>()(handle, argument);

         IntPtr result = evaluateCall(call);
         return new SymbolicExpression(Engine, result);
      }


   }
}