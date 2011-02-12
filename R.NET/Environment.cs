using System;
using System.Runtime.InteropServices;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// An environment object.
	/// </summary>
	public class Environment : SymbolicExpression
	{
		/// <summary>
		/// Gets the parental environment.
		/// </summary>
		public RDotNet.Environment Parent
		{
			get
			{
				SEXPREC sexp = (SEXPREC)Marshal.PtrToStructure(this.handle, typeof(SEXPREC));
				IntPtr parent = sexp.envsxp.enclos;
				return Engine.CheckNil(parent) ? null : new RDotNet.Environment(Engine, parent);
			}
		}

		/// <summary>
		/// Creates an environment object.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="pointer">The pointer to an environment.</param>
		internal protected Environment(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{
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

			IntPtr installedName = Engine.Proxy.Rf_install(name);
			IntPtr value = Engine.Proxy.Rf_findVar(installedName, this.handle);
			if (Engine.CheckUnbound(value))
			{
				return null;
			}

			SEXPREC sexp = (SEXPREC)Marshal.PtrToStructure(value, typeof(SEXPREC));
			if (sexp.sxpinfo.type == SymbolicExpressionType.Promise)
			{
				value = Engine.Proxy.Rf_eval(value, this.handle);
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
			IntPtr installedName = Engine.Proxy.Rf_install(name);
			Engine.Proxy.Rf_setVar(installedName, expression.DangerousGetHandle(), this.handle);
		}
		
		/// <summary>
		/// Gets the symbol names defined in this environment.
		/// </summary>
		/// <param name="all">Including special functions or not.</param>
		/// <returns>Symbol names.</returns>
		public string[] GetSymbolNames(bool all = false)
		{
			CharacterVector symbolNames = new CharacterVector(Engine, Engine.Proxy.R_lsInternal(this.handle, all));
			int length = symbolNames.Length;
			string[] copy = new string[length];
			symbolNames.CopyTo(copy, length);
			return copy;
		}
	}
}
