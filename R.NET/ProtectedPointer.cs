using System;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	internal class ProtectedPointer : IDisposable
	{
		private readonly IntPtr sexp;
		private readonly REngine engine;

		public ProtectedPointer(REngine engine, IntPtr sexp)
		{
			this.sexp = sexp;
			this.engine = engine;
			
			engine.Proxy.Rf_protect(this.sexp);
		}

		public ProtectedPointer(SymbolicExpression sexp)
		{
			this.sexp = (IntPtr)sexp;
			this.engine = sexp.Engine;
			
			engine.Proxy.Rf_protect(this.sexp);
		}

		public static implicit operator IntPtr(ProtectedPointer p)
		{
			return p.sexp;
		}

		public void Dispose()
		{
			this.engine.Proxy.Rf_unprotect_ptr(this.sexp);
		}
	}
}
