using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A sequence of byte values.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class RawVector : Vector<byte>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override byte this[int index]
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
					return Marshal.ReadByte(DataPointer, offset);
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
					Marshal.WriteByte(DataPointer, offset, value);
				}
			}
		}

		/// <summary>
		/// Gets the size of a byte value in byte.
		/// </summary>
		protected override int DataSize
		{
			get
			{
				return sizeof(byte);
			}
		}

		/// <summary>
		/// Creates a new RawVector with the specified length.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="length">The length.</param>
		public RawVector(REngine engine, int length)
			: base(engine, SymbolicExpressionType.RawVector, length)
		{
		}

		/// <summary>
		/// Creates a new RawVector with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="vector">The values.</param>
		/// <seealso cref="REngineExtension.CreateRawVector(REngine, int)"/>
		public RawVector(REngine engine, byte[] vector)
			: base(engine, SymbolicExpressionType.RawVector, vector)
		{
		}

		/// <summary>
		/// Creates a new instance for a raw vector.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="coerced">The pointer to a raw vector.</param>
		/// <seealso cref="REngineExtension.CreateRawVector(REngine, byte[])"/>
		internal protected RawVector(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}

		/// <summary>
		/// Copies the elements to the specified array.
		/// </summary>
		/// <param name="destination">The destination array.</param>
		/// <param name="length">The length to copy.</param>
		/// <param name="sourceIndex">The first index of the vector.</param>
		/// <param name="destinationIndex">The first index of the destination array.</param>
		public new void CopyTo(byte[] destination, int length, int sourceIndex = 0, int destinationIndex = 0)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (length < 0)
			{
				throw new IndexOutOfRangeException("length");
			}
			if (sourceIndex < 0 || this.Length < sourceIndex + length)
			{
				throw new IndexOutOfRangeException("sourceIndex");
			}
			if (destinationIndex < 0 || destination.Length < destinationIndex + length)
			{
				throw new IndexOutOfRangeException("destinationIndex");
			}

			int offset = GetOffset(sourceIndex);
			IntPtr pointer = IntPtr.Add(DataPointer, offset);
			Marshal.Copy(pointer, destination, destinationIndex, length);
		}
	}
}
