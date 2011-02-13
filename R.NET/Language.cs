using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A language object.
	/// </summary>
	public class Language : SymbolicExpression
	{
		/// <summary>
		/// Gets function calls.
		/// </summary>
		public Pairlist FunctionCall
		{
			get
			{
				int count = Engine.Proxy.Rf_length(this.handle);
				// count == 1 for empty call.
				if (count < 2)
				{
					return null;
				}
				SEXPREC sexp = GetInternalStructure();
				return new Pairlist(Engine, sexp.listsxp.cdrval);
			}
		}

		/// <summary>
		/// Creates a language object.
		/// </summary>
		/// <param name="engine">The engine</param>
		/// <param name="pointer">The pointer.</param>
		internal protected Language(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{
		}
	}
}
