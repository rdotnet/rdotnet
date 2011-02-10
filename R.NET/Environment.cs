using System;
using System.Runtime.InteropServices;
using RDotNet.Internals;

namespace RDotNet
{
	public class Environment : SymbolicExpression
	{
		public RDotNet.Environment Parent
		{
			get
			{
				SEXPREC sexp = (SEXPREC)Marshal.PtrToStructure(this.handle, typeof(SEXPREC));
				IntPtr parent = sexp.envsxp.enclos;
				return parent == (IntPtr)Engine.NilValue ? null : new RDotNet.Environment(Engine, parent);
			}
		}

		internal Environment(REngine engine, IntPtr pointer)
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
			IntPtr value = Engine.Proxy.Rf_findVar(installedName, (IntPtr)this);
			if (value == (IntPtr)Engine.UnboundValue)
			{
				return null;
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
			Engine.Proxy.Rf_setVar(installedName, (IntPtr)expression, (IntPtr)this);
		}
	}
}
