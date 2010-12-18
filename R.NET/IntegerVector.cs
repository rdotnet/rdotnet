using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A collection of integers from <c>-2^31 + 1</c> to <c>2^31 - 1</c>.
	/// </summary>
	/// <remarks>
	/// The minimum value of IntegerVector is different from that of System.Int32 in .NET Framework.
	/// </remarks>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class IntegerVector : Vector<int>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override int this[int index]
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
					return Marshal.ReadInt32(DataPointer, offset);
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
					Marshal.WriteInt32(DataPointer, offset, value);
				}
			}
		}

		/// <summary>
		/// Gets the size of an integer in byte.
		/// </summary>
		protected override int DataSize
		{
			get
			{
				return sizeof(int);
			}
		}

		/// <summary>
		/// Creates a new empty IntegerVector with the specified length.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="length">The length.</param>
		public IntegerVector(REngine engine, int length)
			: base(engine, SymbolicExpressionType.IntegerVector, length)
		{
		}

		/// <summary>
		/// Creates a new IntegerVector with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="vector">The values.</param>
		public IntegerVector(REngine engine, int[] vector)
			: base(engine, SymbolicExpressionType.IntegerVector, vector)
		{
		}

		internal protected IntegerVector(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}
	}
}
