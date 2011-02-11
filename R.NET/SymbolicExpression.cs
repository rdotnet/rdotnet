using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Dynamic;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// An expression in R environment.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class SymbolicExpression : SafeHandle, IEquatable<SymbolicExpression>, IDynamicMetaObjectProvider
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

		/// <summary>
		/// Creates new instance of SymbolicExpression.
		/// </summary>
		/// <param name="engine">The engine.</param>
		/// <param name="pointer">The pointer.</param>
		internal protected SymbolicExpression(REngine engine, IntPtr pointer)
			: base(IntPtr.Zero, true)
		{
			this.engine = engine;
			this.sexp = (SEXPREC)Marshal.PtrToStructure(pointer, typeof(SEXPREC));
			SetHandle(pointer);
		}

		/// <summary>
		/// Gets the pointer.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The pointer.</returns>
		[Obsolete("Will be removed. Use DangerousGetHandle() method.")]
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
			int length = Engine.Proxy.Rf_length(sexp.attrib);
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

			IntPtr installedName = Engine.Proxy.Rf_install(attributeName);
			IntPtr attribute = Engine.Proxy.Rf_getAttrib(this.handle, installedName);
			if (Engine.CheckNil(attribute))
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

			IntPtr attribute = Engine.Proxy.Rf_getAttrib(this.handle, symbol.handle);
			if (Engine.CheckNil(attribute))
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

			IntPtr installedName = Engine.Proxy.Rf_install(attributeName);
			Engine.Proxy.Rf_setAttrib(this.handle, installedName, value.handle);
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

			Engine.Proxy.Rf_setAttrib(this.handle, symbol.handle, value.handle);
		}

		/// <summary>
		/// Prevents the expression from R's garbage collector.
		/// </summary>
		/// <seealso cref="SymbolicExpression.Unprotect"/>
		public void Protect()
		{
			if (!IsInvalid)
			{
				Engine.Proxy.Rf_protect(this.handle);
				isProtected = true;
			}
		}

		/// <summary>
		/// Stops protection.
		/// </summary>
		/// <seealso cref="SymbolicExpression.Protect"/>
		public void Unprotect()
		{
			if (!IsInvalid && IsProtected)
			{
				Engine.Proxy.Rf_unprotect_ptr(this.handle);
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

		public virtual DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
		{
			return new SymbolicExpressionDynamicMeta(parameter, this);
		}
	}
}
