using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class InternalString : SymbolicExpression
	{
		public InternalString(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{
		}

		public InternalString(REngine engine, string s)
			: base(engine, NativeMethods.Rf_mkChar(s))
		{
		}

		public static implicit operator string(InternalString s)
		{
			return s.ToString();
		}

		public override string ToString()
		{
			IntPtr pointer = Utility.OffsetPointer(this.handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
			return Marshal.PtrToStringAnsi(pointer);
		}
	}
}
