using System;
using System.Runtime.InteropServices;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// An environment object.
	/// </summary>
	public class REnvironment : SymbolicExpression
	{
		/// <summary>
		/// Creates an environment object.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="pointer">The pointer to an environment.</param>
		protected internal REnvironment(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{}

		/// <summary>
		/// Gets the parental environment.
		/// </summary>
		public REnvironment Parent
		{
			get
			{
				SEXPREC sexp = GetInternalStructure();
				IntPtr parent = sexp.envsxp.enclos;
				return Engine.CheckNil(parent) ? null : new REnvironment(Engine, parent);
			}
		}

		/// <summary>
		/// Gets a symbol defined in this environment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The symbol.</returns>
		public SymbolicExpression GetSymbol(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			if (name == string.Empty)
			{
				throw new ArgumentException();
			}

			IntPtr installedName = Engine.GetFunction<Rf_install>("Rf_install")(name);
			IntPtr value = Engine.GetFunction<Rf_findVar>("Rf_findVar")(installedName, handle);
			if (Engine.CheckUnbound(value))
			{
				return null;
			}

			var sexp = (SEXPREC)Marshal.PtrToStructure(value, typeof(SEXPREC));
			if (sexp.sxpinfo.type == SymbolicExpressionType.Promise)
			{
				value = Engine.GetFunction<Rf_eval>("Rf_eval")(value, handle);
			}
			return new SymbolicExpression(Engine, value);
		}

		/// <summary>
		/// Defines a symbol in this environment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="expression">The symbol.</param>
		public void SetSymbol(string name, SymbolicExpression expression)
		{
			IntPtr installedName = Engine.GetFunction<Rf_install>("Rf_install")(name);
			Engine.GetFunction<Rf_setVar>("Rf_setVar")(installedName, expression.DangerousGetHandle(), handle);
		}

		/// <summary>
		/// Gets the symbol names defined in this environment.
		/// </summary>
		/// <param name="all">Including special functions or not.</param>
		/// <returns>Symbol names.</returns>
		public string[] GetSymbolNames(bool all = false)
		{
			var symbolNames = new CharacterVector(Engine, Engine.GetFunction<R_lsInternal>("R_lsInternal")(handle, all));
			int length = symbolNames.Length;
			var copy = new string[length];
			symbolNames.CopyTo(copy, length);
			return copy;
		}
	}
}
