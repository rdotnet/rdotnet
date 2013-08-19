using System;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	internal class ProtectedPointer : IDisposable
	{
		private readonly REngine engine;
		private readonly IntPtr sexp;

		public ProtectedPointer(REngine engine, IntPtr sexp)
		{
			this.sexp = sexp;
			this.engine = engine;

			engine.GetFunction<Rf_protect>()(this.sexp);
		}

		public ProtectedPointer(SymbolicExpression sexp)
		{
			this.sexp = sexp.DangerousGetHandle();
			this.engine = sexp.Engine;

			this.engine.GetFunction<Rf_protect>()(this.sexp);
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.engine.GetFunction<Rf_unprotect_ptr>()(this.sexp);
		}

		#endregion

		public static implicit operator IntPtr(ProtectedPointer p)
		{
			return p.sexp;
		}
	}
}
