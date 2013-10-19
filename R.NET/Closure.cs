using System.Collections.Generic;
using RDotNet.Internals;
using System;
using System.Linq;

namespace RDotNet
{
   /// <summary>
   /// A closure.
   /// </summary>
   public class Closure : Function
   {
      /// <summary>
      /// Creates a closure object.
      /// </summary>
      /// <param name="engine">The engine.</param>
      /// <param name="pointer">The pointer.</param>
      protected internal Closure(REngine engine, IntPtr pointer)
         : base(engine, pointer)
      { }

      /// <summary>
      /// Gets the arguments list.
      /// </summary>
      public Pairlist Arguments
      {
         get
         {
            SEXPREC sexp = GetInternalStructure();
            return new Pairlist(Engine, sexp.closxp.formals);
         }
      }

      /// <summary>
      /// Gets the body.
      /// </summary>
      public Language Body
      {
         get
         {
            SEXPREC sexp = GetInternalStructure();
            return new Language(Engine, sexp.closxp.body);
         }
      }

      /// <summary>
      /// Gets the environment.
      /// </summary>
      public REnvironment Environment
      {
         get
         {
            SEXPREC sexp = GetInternalStructure();
            return new REnvironment(Engine, sexp.closxp.env);
         }
      }

      public override SymbolicExpression Invoke(params SymbolicExpression[] args)
      {
         int count = Arguments.Count;
         if (args.Length > count)
            throw new ArgumentException("Too many arguments provided for this function", "args");

         string[] fargNames = GetArgumentNames();
         fargNames = Utility.Subset(fargNames, 0, args.Length - 1);
         return Invoke(fargNames, args);
      }

      public override SymbolicExpression Invoke(IDictionary<string, SymbolicExpression> args)
      {
         if (args.Count > Arguments.Count)
            throw new ArgumentException("Too many arguments provided for this function", "args");

         var argNames = args.Keys.ToArray();
         var argsArr = new SymbolicExpression[args.Count];
         for (int i = 0; i < argNames.Length; i++)
            argsArr[i] = args[argNames[i]];

         return Invoke(argNames, argsArr);
      }

      private SymbolicExpression Invoke(string[] argNames, SymbolicExpression[] args)
      {
         var names = new CharacterVector(Engine, argNames);
         var arguments = new GenericVector(Engine, args);
         arguments.SetAttribute(Engine.GetPredefinedSymbol("R_NamesSymbol"), names);
         var argPairList = arguments.ToPairlist();

         IntPtr newEnvironment = Engine.GetFunction<Rf_allocSExp>()(SymbolicExpressionType.Environment);
         IntPtr result = Engine.GetFunction<Rf_applyClosure>()(Body.DangerousGetHandle(), handle,
                                                               argPairList.DangerousGetHandle(),
                                                               Environment.DangerousGetHandle(), newEnvironment);
         return new SymbolicExpression(Engine, result);
      }

      private string[] GetArgumentNames()
      {
         return Arguments.Select(arg => arg.PrintName).ToArray();
      }
   }
}