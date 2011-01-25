using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// An expression in R environment.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class SymbolicExpression : SafeHandle, IEquatable<SymbolicExpression>
	{
		public override bool IsInvalid
		{
			get
			{
				return IsClosed || this.handle == IntPtr.Zero;
			}
		}

		private readonly REngine engine;
		/// <summary>
		/// Gets the <see cref="REngine"/> to which this expression belongs.
		/// </summary>
		public REngine Engine
		{
			get
			{
				return engine;
			}
		}

		private bool isProtected;
		/// <summary>
		/// Gets whether this expression is protected from the garbage collection.
		/// </summary>
		public bool IsProtected
		{
			get
			{
				return isProtected;
			}
		}

		private readonly SEXPREC sexp;

		/// <summary>
		/// Gets the <see cref="SymbolicExpressionType"/>.
		/// </summary>
		public SymbolicExpressionType Type
		{
			get
			{
				return sexp.sxpinfo.type;
			}
		}

		internal protected SymbolicExpression(REngine engine, IntPtr pointer)
			: base(IntPtr.Zero, true)
		{
			this.engine = engine;
			this.sexp = (SEXPREC)Marshal.PtrToStructure(pointer, typeof(SEXPREC));
			SetHandle(pointer);
		}

		public static explicit operator IntPtr(SymbolicExpression expression)
		{
			return expression.handle;
		}

		/// <summary>
		/// Gets all value names.
		/// </summary>
		/// <returns>The names of attributes</returns>
		public string[] GetAttributeNames()
		{
			int length = NativeMethods.Rf_length(sexp.attrib);
			var names = new string[length];
			IntPtr pointer = sexp.attrib;
			for (int index = 0; index < length; index++)
			{
				SEXPREC node = (SEXPREC)Marshal.PtrToStructure(pointer, typeof(SEXPREC));
				SEXPREC attribute = (SEXPREC)Marshal.PtrToStructure(node.listsxp.tagval, typeof(SEXPREC));
				IntPtr name = attribute.symsxp.pname;
				names[index] = (string)new InternalString(Engine, name);
				pointer = node.listsxp.cdrval;
			}
			return names;
		}

		/// <summary>
		/// Gets the value of the specified name.
		/// </summary>
		/// <param name="attributeName">The name of attribute.</param>
		/// <returns>The attribute.</returns>
		public SymbolicExpression GetAttribute(string attributeName)
		{
			if (attributeName == null)
			{
				throw new ArgumentNullException();
			}
			if (attributeName == string.Empty)
			{
				throw new ArgumentException();
			}

			IntPtr installedName = NativeMethods.Rf_install(attributeName);
			IntPtr attribute = NativeMethods.Rf_getAttrib((IntPtr)this, installedName);
			if (attribute == (IntPtr)Engine.NilValue)
			{
				return null;
			}
			return new SymbolicExpression(Engine, attribute);
		}

		internal SymbolicExpression GetAttribute(SymbolicExpression symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException();
			}
			if (symbol.Type != SymbolicExpressionType.Symbol)
			{
				throw new ArgumentException();
			}

			IntPtr attribute = NativeMethods.Rf_getAttrib((IntPtr)this, (IntPtr)symbol);
			if (attribute == (IntPtr)Engine.NilValue)
			{
				return null;
			}
			return new SymbolicExpression(Engine, attribute);
		}

		/// <summary>
		/// Sets the new value to the attribute of the specified name.
		/// </summary>
		/// <param name="attributeName">The name of attribute.</param>
		/// <param name="value">The value</param>
		public void SetAttribute(string attributeName, SymbolicExpression value)
		{
			if (attributeName == null)
			{
				throw new ArgumentNullException();
			}
			if (attributeName == string.Empty)
			{
				throw new ArgumentException();
			}

			if (value == null)
			{
				value = Engine.NilValue;
			}

			IntPtr installedName = NativeMethods.Rf_install(attributeName);
			NativeMethods.Rf_setAttrib((IntPtr)this, installedName, (IntPtr)value);
		}

		internal void SetAttribute(SymbolicExpression symbol, SymbolicExpression value)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException();
			}
			if (symbol.Type != SymbolicExpressionType.Symbol)
			{
				throw new ArgumentException();
			}

			if (value == null)
			{
				value = Engine.NilValue;
			}

			NativeMethods.Rf_setAttrib((IntPtr)this, (IntPtr)symbol, (IntPtr)value);
		}

		public void Protect()
		{
			if (!IsInvalid)
			{
				NativeMethods.Rf_protect(this.handle);
				isProtected = true;
			}
		}

		public void Unprotect()
		{
			if (!IsInvalid && IsProtected)
			{
				NativeMethods.Rf_unprotect_ptr(this.handle);
				isProtected = false;
			}
		}

		protected override bool ReleaseHandle()
		{
			if (IsProtected)
			{
				Unprotect();
			}
			return true;
		}

		public override int GetHashCode()
		{
			return this.handle.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as SymbolicExpression);
		}

		public bool Equals(SymbolicExpression other)
		{
			return other != null && this.handle == other.handle;
		}
	}
}
