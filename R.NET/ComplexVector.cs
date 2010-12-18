using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A collection of complex numbers.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class ComplexVector : Vector<Complex>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override Complex this[int index]
		{
			get
			{
				if (index < 0 || Length <= index)
				{
					throw new ArgumentOutOfRangeException();
				}
				using (new ProtectedPointer(this))
				{
					byte[] data = new byte[Marshal.SizeOf(typeof(Complex))];
					int offset = GetOffset(index);
					IntPtr pointer = IntPtr.Add(DataPointer, offset);
					Marshal.Copy(pointer, data, 0, data.Length);

					double real = BitConverter.ToDouble(data, 0);
					double imaginary = BitConverter.ToDouble(data, sizeof(double));
					return new Complex(real, imaginary);
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
					byte[] real = BitConverter.GetBytes(value.Real);
					byte[] imaginary = BitConverter.GetBytes(value.Imaginary);

					int offset = GetOffset(index);
					IntPtr pointer = IntPtr.Add(DataPointer, offset);
					Marshal.Copy(real, 0, pointer, real.Length);
					pointer = IntPtr.Add(pointer, real.Length);
					Marshal.Copy(imaginary, 0, pointer, imaginary.Length);
				}
			}
		}

		/// <summary>
		/// Gets the size of a complex number in byte.
		/// </summary>
		protected override int DataSize
		{
			get
			{
				return Marshal.SizeOf(typeof(Complex));
			}
		}

		/// <summary>
		/// Creates a new empty ComplexVector with the specified length.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="length">The length.</param>
		public ComplexVector(REngine engine, int size)
			: base(engine, SymbolicExpressionType.ComplexVector, size)
		{
		}

		/// <summary>
		/// Creates a new ComplexVector with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="vector">The values.</param>
		public ComplexVector(REngine engine, Complex[] vector)
			: base(engine, SymbolicExpressionType.ComplexVector, vector)
		{
		}

		internal protected ComplexVector(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}
	}
}
