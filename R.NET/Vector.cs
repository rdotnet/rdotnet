using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public abstract class Vector<T> : SymbolicExpression, IEnumerable<T>
	{
		private const string NamesAttributeName = "names";

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public abstract T this[int index]
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the number of elements.
		/// </summary>
		public int Length
		{
			get
			{
				return NativeMethods.Rf_length(this.handle);
			}
		}

		/// <summary>
		/// Gets the names of elements.
		/// </summary>
		public string[] Names
		{
			get
			{
				SymbolicExpression names = GetAttribute(NamesAttributeName);
				if (names == null)
				{
					return null;
				}
				CharacterVector namesVector = names.AsCharacter();
				if (namesVector == null)
				{
					return null;
				}

				int length = namesVector.Length;
				string[] result = new string[length];
				namesVector.CopyTo(result, length);
				return result;
			}
		}

		/// <summary>
		/// Gets the pointer for the first element.
		/// </summary>
		protected IntPtr DataPointer
		{
			get
			{
				return IntPtr.Add(this.handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
			}
		}

		/// <summary>
		/// Gets the size of an element in byte.
		/// </summary>
		protected abstract int DataSize
		{
			get;
		}

		protected Vector(REngine engine, SymbolicExpressionType type, int length)
			: base(engine, NativeMethods.Rf_allocVector(type, length))
		{
			if (length <= 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
		}

		protected Vector(REngine engine, SymbolicExpressionType type, T[] vector)
			: base(engine, NativeMethods.Rf_allocVector(type, vector.Length))
		{
			for (int index = 0; index < vector.Length; index++)
			{
				this[index] = vector[index];
			}
		}

		protected Vector(REngine engine, IntPtr coerced)
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
		public void CopyTo(T[] destination, int length, int sourceIndex = 0, int destinationIndex = 0)
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

			while (--length >= 0)
			{
				destination[destinationIndex++] = this[sourceIndex++];
			}
		}

		protected int GetOffset(int index)
		{
			return DataSize * index;
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int index = 0; index < Length; index++)
			{
				yield return this[index];
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
