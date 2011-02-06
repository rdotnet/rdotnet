using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A collection of real numbers in double precision.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class NumericVector : Vector<double>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override double this[int index]
		{
			get
			{
				if (index < 0 || Length <= index)
				{
					throw new ArgumentOutOfRangeException();
				}
				using (new ProtectedPointer(this))
				{
					double[] data = new double[1];
					int offset = GetOffset(index);
					IntPtr pointer = Utility.OffsetPointer(DataPointer, offset);
					Marshal.Copy(pointer, data, 0, data.Length);
					return data[0];
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
					double[] data = new double[] { value };
					int offset = GetOffset(index);
					IntPtr pointer = Utility.OffsetPointer(DataPointer, offset);
					Marshal.Copy(data, 0, pointer, data.Length);
				}
			}
		}

		/// <summary>
		/// Gets the size of a real number in byte.
		/// </summary>
		protected override int DataSize
		{
			get
			{
				return sizeof(double);
			}
		}

		/// <summary>
		/// Creates a new empty NumericVector with the specified length.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="length">The length.</param>
		/// <seealso cref="REngineExtension.CreateNumericVector(REngine, int)"/>
		public NumericVector(REngine engine, int length)
			: base(engine, SymbolicExpressionType.NumericVector, length)
		{
		}

		/// <summary>
		/// Creates a new NumericVector with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="vector">The values.</param>
		/// <seealso cref="REngineExtension.CreateNumericVector(REngine, double[])"/>
		public NumericVector(REngine engine, double[] vector)
			: base(engine, SymbolicExpressionType.NumericVector, vector)
		{
		}

		/// <summary>
		/// Creates a new instance for a numeric vector.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="coerced">The pointer to a numeric vector.</param>
		internal protected NumericVector(REngine engine, IntPtr coerced)
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
		public new void CopyTo(double[] destination, int length, int sourceIndex = 0, int destinationIndex = 0)
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
			IntPtr pointer = Utility.OffsetPointer(DataPointer, offset);
			Marshal.Copy(pointer, destination, destinationIndex, length);
		}
	}
}
