using System;
using System.Linq;
using System.Runtime.InteropServices;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A closure.
	/// </summary>
	public class Closure : Function
	{
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
		public RDotNet.Environment Environment
		{
			get
			{
				SEXPREC sexp = GetInternalStructure();
				return new RDotNet.Environment(Engine, sexp.closxp.env);
			}
		}

		/// <summary>
		/// Creates a closure object.
		/// </summary>
		/// <param name="engine">The engine.</param>
		/// <param name="pointer">The pointer.</param>
		internal protected Closure(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{
		}

		public override SymbolicExpression Invoke(SymbolicExpression[] args)
		{
			int count = Arguments.Count;
			if (args.Length != count)
			{
				throw new ArgumentException();
			}

			GenericVector arguments = new GenericVector(Engine, args);
			CharacterVector names = new CharacterVector(Engine, Arguments.Select(arg => arg.PrintName).ToArray());
			arguments.SetAttribute(Engine.GetPredefinedSymbol("R_NamesSymbol"), names);

			IntPtr newEnvironment = Engine.Proxy.Rf_allocSExp(SymbolicExpressionType.Environment);
			IntPtr result = Engine.Proxy.Rf_applyClosure(Body.DangerousGetHandle(), this.handle, arguments.ToPairlist().DangerousGetHandle(), Environment.DangerousGetHandle(), newEnvironment);
			return new SymbolicExpression(Engine, result);
		}
	}
}
