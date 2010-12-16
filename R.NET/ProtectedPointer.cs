using System;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	internal class ProtectedPointer : IDisposable
	{
		private readonly IntPtr sexp;

		public ProtectedPointer(IntPtr sexp)
		{
			this.sexp = sexp;
			NativeMethods.Rf_protect(this.sexp);
		}

		public ProtectedPointer(SymbolicExpression sexp)
		{
			this.sexp = (IntPtr)sexp;
			NativeMethods.Rf_protect(this.sexp);
		}

		public static implicit operator IntPtr(ProtectedPointer p)
		{
			return p.sexp;
		}

		public void Dispose()
		{
			NativeMethods.Rf_unprotect_ptr(this.sexp);
		}
	}
}
