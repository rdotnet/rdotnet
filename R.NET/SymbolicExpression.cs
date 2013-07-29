using System;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Dynamic;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// An expression in R environment.
   /// </summary>
   [DebuggerDisplay("RObjectType = {Type}")]
   [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class SymbolicExpression : SafeHandle, IEquatable<SymbolicExpression>, IDynamicMetaObjectProvider
	{
		private readonly REngine engine;
		private readonly SEXPREC sexp;

		private bool isProtected;

		/// <summary>
		/// Creates new instance of SymbolicExpression.
		/// </summary>
		/// <param name="engine">The engine.</param>
		/// <param name="pointer">The pointer.</param>
		protected internal SymbolicExpression(REngine engine, IntPtr pointer)
			: base(IntPtr.Zero, true)
		{
			this.engine = engine;
			this.sexp = (SEXPREC)Marshal.PtrToStructure(pointer, typeof(SEXPREC));
			SetHandle(pointer);
            Preserve();
		}

		public override bool IsInvalid
		{
			get { return IsClosed || handle == IntPtr.Zero; }
		}

		/// <summary>
		/// Gets the <see cref="REngine"/> to which this expression belongs.
		/// </summary>
		public REngine Engine
		{
			get { return this.engine; }
		}

		/// <summary>
		/// Gets whether this expression is protected from the garbage collection.
		/// </summary>
		public bool IsProtected
		{
			get { return this.isProtected; }
		}

		/// <summary>
		/// Gets the <see cref="SymbolicExpressionType"/>.
		/// </summary>
		public SymbolicExpressionType Type
		{
			get { return this.sexp.sxpinfo.type; }
		}

		#region IDynamicMetaObjectProvider Members

		public virtual DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
		{
			return new SymbolicExpressionDynamicMeta(parameter, this);
		}

		#endregion

		#region IEquatable<SymbolicExpression> Members

		public bool Equals(SymbolicExpression other)
		{
			return other != null && handle == other.handle;
		}

		#endregion

		internal SEXPREC GetInternalStructure()
		{
			return (SEXPREC)Marshal.PtrToStructure(handle, typeof(SEXPREC));
		}

		/// <summary>
		/// Gets all value names.
		/// </summary>
		/// <returns>The names of attributes</returns>
		public string[] GetAttributeNames()
		{
			int length = Engine.GetFunction<Rf_length>("Rf_length")(this.sexp.attrib);
			var names = new string[length];
			IntPtr pointer = this.sexp.attrib;
			for (int index = 0; index < length; index++)
			{
				var node = (SEXPREC)Marshal.PtrToStructure(pointer, typeof(SEXPREC));
				var attribute = (SEXPREC)Marshal.PtrToStructure(node.listsxp.tagval, typeof(SEXPREC));
				IntPtr name = attribute.symsxp.pname;
				names[index] = new InternalString(Engine, name);
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

			IntPtr installedName = Engine.GetFunction<Rf_install>("Rf_install")(attributeName);
			IntPtr attribute = Engine.GetFunction<Rf_getAttrib>("Rf_getAttrib")(handle, installedName);
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

			IntPtr attribute = Engine.GetFunction<Rf_getAttrib>("Rf_getAttrib")(handle, symbol.handle);
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

			IntPtr installedName = Engine.GetFunction<Rf_install>("Rf_install")(attributeName);
			Engine.GetFunction<Rf_setAttrib>("Rf_setAttrib")(handle, installedName, value.handle);
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

			Engine.GetFunction<Rf_setAttrib>("Rf_setAttrib")(handle, symbol.handle, value.handle);
		}

		/// <summary>
		/// Protects the expression from R's garbage collector.
		/// </summary>
		/// <seealso cref="Unpreserve"/>
		public void Preserve()
		{
			if (!IsInvalid && !isProtected)
			{
                Engine.GetFunction<R_PreserveObject>("R_PreserveObject")(handle);
				this.isProtected = true;
			}
		}

		/// <summary>
		/// Stops protection.
		/// </summary>
		/// <seealso cref="Preserve"/>
		public void Unpreserve()
		{
			if (!IsInvalid && IsProtected)
			{
                Engine.GetFunction<R_ReleaseObject>("R_ReleaseObject")(handle);
				this.isProtected = false;
			}
		}

		protected override bool ReleaseHandle()
		{
			if (IsProtected)
			{
				Unpreserve();
			}
			return true;
		}

		public override int GetHashCode()
		{
			return handle.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as SymbolicExpression);
		}
	}
}
