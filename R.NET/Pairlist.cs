using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A pairlist.
	/// </summary>
	public class Pairlist : SymbolicExpression, IEnumerable<Symbol>
	{
		/// <summary>
		/// Gets the number of nodes.
		/// </summary>
		public int Count
		{
			get
			{
				return Engine.Proxy.Rf_length(this.handle);
			}
		}

		/// <summary>
		/// Creates a pairlist.
		/// </summary>
		/// <param name="engine">The engine</param>
		/// <param name="pointer">The pointer.</param>
		internal protected Pairlist(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{
		}

		public IEnumerator<Symbol> GetEnumerator()
		{
			if (Count != 0)
			{
				for (SEXPREC sexp = GetInternalStructure(); sexp.sxpinfo.type != SymbolicExpressionType.Null; sexp = (SEXPREC)Marshal.PtrToStructure(sexp.listsxp.cdrval, typeof(SEXPREC)))
				{
					yield return new Symbol(Engine, sexp.listsxp.tagval);
				}
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
