using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class ExpressionVector : Vector<Expression>
	{
		/// <summary>
		/// Creates a new instance for an expression vector.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="coerced">The pointer to an expression vector.</param>
		internal ExpressionVector(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{}

		public override Expression this[int index]
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
					return new Expression(Engine, pointer);
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

		/// <summary>
		/// Gets the size of a pointer in byte.
		/// </summary>
		protected override int DataSize
		{
			get { return Marshal.SizeOf(typeof(IntPtr)); }
		}
	}
}
