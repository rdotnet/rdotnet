using System;
using System.Linq;
using RDotNet.Internals;

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
		{}

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

		public override SymbolicExpression Invoke(SymbolicExpression[] args)
		{
			int count = Arguments.Count;
			if (args.Length != count)
			{
				throw new ArgumentException();
			}

			var arguments = new GenericVector(Engine, args);
			var names = new CharacterVector(Engine, Arguments.Select(arg => arg.PrintName).ToArray());
			arguments.SetAttribute(Engine.GetPredefinedSymbol("R_NamesSymbol"), names);

			IntPtr newEnvironment = Engine.GetFunction<Rf_allocSExp>()(SymbolicExpressionType.Environment);
			IntPtr result = Engine.GetFunction<Rf_applyClosure>()(Body.DangerousGetHandle(), handle, arguments.ToPairlist().DangerousGetHandle(), Environment.DangerousGetHandle(), newEnvironment);
			return new SymbolicExpression(Engine, result);
		}
	}
}
