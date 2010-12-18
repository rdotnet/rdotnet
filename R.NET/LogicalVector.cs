using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A collection of Boolean values.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class LogicalVector : Vector<bool>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override bool this[int index]
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
					int data = Marshal.ReadInt32(DataPointer, offset);
					return Convert.ToBoolean(data);
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
					int data = Convert.ToInt32(value);
					Marshal.WriteInt32(DataPointer, offset, data);
				}
			}
		}

		/// <summary>
		/// Gets the size of a Boolean value in byte.
		/// </summary>
		protected override int DataSize
		{
			get
			{
				// Boolean is int internally.
				return sizeof(int);
			}
		}

		/// <summary>
		/// Creates a new empty LogicalVector with the specified length.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="length">The length.</param>
		public LogicalVector(REngine engine, int length)
			: base(engine, SymbolicExpressionType.LogicalVector, length)
		{
		}

		/// <summary>
		/// Creates a new LogicalVector with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="vector">The values.</param>
		public LogicalVector(REngine engine, bool[] vector)
			: base(engine, SymbolicExpressionType.LogicalVector, vector)
		{
		}

		internal protected LogicalVector(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}
	}
}
