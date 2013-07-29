using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// Internal string.
	/// </summary>
	[DebuggerDisplay("Content = {ToString()}; RObjectType = {Type}")]
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class InternalString : SymbolicExpression
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="pointer">The pointer to a string.</param>
		public InternalString(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="s">The string</param>
		public InternalString(REngine engine, string s)
			: base(engine, engine.GetFunction<Rf_mkChar>("Rf_mkChar")(s))
		{}

		/// <summary>
		/// Converts to the string into .NET Framework string.
		/// </summary>
		/// <param name="s">The R string.</param>
		/// <returns>The .NET Framework string.</returns>
		public static implicit operator string(InternalString s)
		{
			return s.ToString();
		}

		public override string ToString()
		{
			IntPtr pointer = IntPtr.Add(handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
			return Marshal.PtrToStringAnsi(pointer);
		}
	}
}
