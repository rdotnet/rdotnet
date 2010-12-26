using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class InternalString : SymbolicExpression
	{
		private readonly string internalString;

		public InternalString(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{
			IntPtr stringPointer = Utility.OffsetPointer(this.handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
			internalString = Marshal.PtrToStringAnsi(stringPointer);
		}

		public InternalString(REngine engine, string s)
			: base(engine, NativeMethods.Rf_mkChar(s))
		{
			internalString = s;
		}

		public static implicit operator string(InternalString s)
		{
			return s.ToString();
		}

		public override string ToString()
		{
			return internalString;
		}
	}
}
