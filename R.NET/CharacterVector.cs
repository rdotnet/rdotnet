using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A collection of strings.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class CharacterVector : Vector<string>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public override string this[int index]
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
					return new InternalString(Engine, pointer);
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
					SymbolicExpression s = value == null ? Engine.NilValue : new InternalString(Engine, value);
					using (new ProtectedPointer(s))
					{
						Marshal.WriteIntPtr(DataPointer, offset, s.DangerousGetHandle());
					}
				}
			}
		}

		/// <summary>
		/// Gets the size of a pointer in byte.
		/// </summary>
		protected override int DataSize
		{
			get
			{
				return Marshal.SizeOf(typeof(IntPtr));
			}
		}

		/// <summary>
		/// Creates a new empty CharacterVector with the specified length.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="length">The length.</param>
		/// <seealso cref="REngineExtension.CreateCharacterVector(REngine, int)"/>
		public CharacterVector(REngine engine, int length)
			: base(engine, SymbolicExpressionType.CharacterVector, length)
		{
		}

		/// <summary>
		/// Creates a new CharacterVector with the specified values.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="vector">The values.</param>
		/// <seealso cref="REngineExtension.CreateCharacterVector(REngine, string[])"/>
		public CharacterVector(REngine engine, string[] vector)
			: base(engine, SymbolicExpressionType.CharacterVector, vector)
		{
		}

		/// <summary>
		/// Creates a new instance for a string vector.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="coerced">The pointer to a string vector.</param>
		internal protected CharacterVector(REngine engine, IntPtr coerced)
			: base(engine, coerced)
		{
		}
	}
}
