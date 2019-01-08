using RDotNet.Internals;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// A sequence of byte values.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class RawVector : Vector<byte>
    {
        /// <summary>
        /// Creates a new RawVector with the specified length.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="length">The length.</param>
        public RawVector(REngine engine, int length)
            : base(engine, SymbolicExpressionType.RawVector, length)
        { }

        /// <summary>
        /// Creates a new RawVector with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="vector">The values.</param>
        /// <seealso cref="REngineExtension.CreateRawVector(REngine, IEnumerable{byte})"/>
        public RawVector(REngine engine, IEnumerable<byte> vector)
            : base(engine, SymbolicExpressionType.RawVector, vector)
        { }

        /// <summary>
        /// Creates a new RawVector with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="vector">The values.</param>
        /// <seealso cref="REngineExtension.CreateRawVector(REngine, int)"/>
        public RawVector(REngine engine, byte[] vector)
            : base(engine, SymbolicExpressionType.RawVector, vector.Length)
        {
            Marshal.Copy(vector, 0, DataPointer, vector.Length);
        }

        /// <summary>
        /// Creates a new instance for a raw vector.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="coerced">The pointer to a raw vector.</param>
        /// <seealso cref="REngineExtension.CreateRawVector(REngine, IEnumerable{byte})"/>
        protected internal RawVector(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected override byte GetValue(int index)
        {
            int offset = GetOffset(index);
            return Marshal.ReadByte(DataPointer, offset);
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected override byte GetValueAltRep(int index)
        {
            return GetFunction<RAW_ELT>()(this.DangerousGetHandle(), (IntPtr)index);
        }

        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to set</param>
        protected override void SetValue(int index, byte value)
        {
            int offset = GetOffset(index);
            Marshal.WriteByte(DataPointer, offset, value);
        }

        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to set</param>
        protected override void SetValueAltRep(int index, byte value)
        {
            GetFunction<SET_RAW_ELT>()(this.DangerousGetHandle(), (IntPtr)index, value);
        }

        /// <summary>
        /// Efficient conversion from R vector representation to the array equivalent in the CLR
        /// </summary>
        /// <returns>Array equivalent</returns>
        protected override byte[] GetArrayFast()
        {
            var res = new byte[this.Length];
            Marshal.Copy(DataPointer, res, 0, res.Length);
            return res;
        }

        /// <summary>
        /// Sets the values of this RawVector
        /// </summary>
        /// <param name="values">Managed values, to be converted to unmanaged equivalent</param>
        protected override void SetVectorDirect(byte[] values)
        {
            Marshal.Copy(values, 0, DataPointer, values.Length);
        }

        /// <summary>
        /// Gets the size of a byte value in byte.
        /// </summary>
        protected override int DataSize
        {
            get { return sizeof(byte); }
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
            if (sourceIndex < 0 || Length < sourceIndex + length)
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