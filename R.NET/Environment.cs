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
	}
}
