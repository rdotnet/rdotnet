using System;
using System.Runtime.InteropServices;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A symbol object.
	/// </summary>
	public class Symbol : SymbolicExpression
	{
		/// <summary>
		/// Gets and sets the name.
		/// </summary>
		public string PrintName
		{
			get
			{
				SEXPREC sexp = GetInternalStructure();
				return new InternalString(Engine, sexp.symsxp.pname).ToString();
			}
			set
			{
				IntPtr pointer = (value == null ? Engine.NilValue : new InternalString(Engine, value)).DangerousGetHandle();
				int offset = GetOffsetOf("pname");
				Marshal.WriteIntPtr(this.handle, offset, pointer);
			}
		}

		/// <summary>
		/// Gets the internal function.
		/// </summary>
		public SymbolicExpression Internal
		{
			get
			{
				SEXPREC sexp = GetInternalStructure();
				if (Engine.CheckNil(sexp.symsxp.value))
				{
					return null;
				}
				return new SymbolicExpression(Engine, sexp.symsxp.@internal);
			}
		}

		/// <summary>
		/// Gets the symbol value.
		/// </summary>
		public SymbolicExpression Value
		{
			get
			{
				SEXPREC sexp = GetInternalStructure();
				if (Engine.CheckNil(sexp.symsxp.value))
				{
					return null;
				}
				return new SymbolicExpression(Engine, sexp.symsxp.value);
			}
		}
		
		/// <summary>
		/// Creates a symbol.
		/// </summary>
		/// <param name="engine">The engine.</param>
		/// <param name="pointer">The pointer.</param>
		internal protected Symbol(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{
		}

		private static int GetOffsetOf(string fieldName)
		{
			return Marshal.OffsetOf(typeof(SEXPREC), "u").ToInt32() + Marshal.OffsetOf(typeof(symsxp), fieldName).ToInt32();
		}
	}
}
