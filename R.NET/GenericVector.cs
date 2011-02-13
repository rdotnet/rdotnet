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
	/// A generic list. This is also known as list in R.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class GenericVector : Vector<SymbolicExpression>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override SymbolicExpression this[int index]
		{
			get
			{
				if (index < 0 || Length <= index)
				{
					throw new ArgumentOutOfRangeException();
				}
				using (new ProtectedPointer(this))
				{
					int offset = GetOffset(index);
					IntPtr pointer = Marshal.ReadIntPtr(DataPointer, offset);
					return new SymbolicExpression(Engine, pointer);
				}
			}
			set
			{
				if (index < 0 || Length <= index)
				{
					throw new ArgumentOutOfRangeException();
				}
				using (new ProtectedPointer(this))
				{
					int offset = GetOffset(index);
					Marshal.WriteIntPtr(DataPointer, offset, (value ?? Engine.NilValue).DangerousGetHandle());
				}
			}
		}

		protected override int DataSize
		{
			get
			{
				return Marshal.SizeOf(typeof(IntPtr));
			}
		}

		/// <summary>
		/// Creates a new empty GenericVector with the specified length.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="length">The length.</param>
		public GenericVector(REngine engine, int length)
			: base(engine, engine.Proxy.Rf_allocVector(SymbolicExpressionType.List, length))
		{
		}

		/// <summary>
		/// Creates a new GenericVector with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="list">The values.</param>
		public GenericVector(REngine engine, SymbolicExpression[] list)
			: base(engine, engine.Proxy.Rf_allocVector(SymbolicExpressionType.List, list.Length))
		{
			for (int index = 0; index < list.Length; index++)
			{
				this[index] = list[index];
			}
		}

		/// <summary>
		/// Creates a new instance for a list.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="coerced">The pointer to a list.</param>
		internal protected GenericVector(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}

		/// <summary>
		/// Converts into a <see cref="RDotNet.Pairlist"/>.
		/// </summary>
		/// <returns>The pairlist.</returns>
		public Pairlist ToPairlist()
		{
			return new Pairlist(Engine, Engine.Proxy.Rf_VectorToPairList(this.handle));
		}

		public override DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
		{
			return new ListDynamicMeta(parameter, this);
		}
	}
}
