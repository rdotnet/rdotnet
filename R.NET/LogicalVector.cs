using RDotNet.Internals;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// A collection of Boolean values.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class LogicalVector : Vector<bool>
    {
        /// <summary>
        /// Creates a new empty LogicalVector with the specified length.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="length">The length.</param>
        /// <seealso cref="REngineExtension.CreateLogicalVector(REngine, int)"/>
        public LogicalVector(REngine engine, int length)
            : base(engine, SymbolicExpressionType.LogicalVector, length)
        { }

        /// <summary>
        /// Creates a new LogicalVector with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="vector">The values.</param>
        /// <seealso cref="REngineExtension.CreateLogicalVector(REngine, IEnumerable{bool})"/>
        public LogicalVector(REngine engine, IEnumerable<bool> vector)
            : base(engine, SymbolicExpressionType.LogicalVector, vector)
        { }

        /// <summary>
        /// Creates a new instance for a Boolean vector.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="coerced">The pointer to a Boolean vector.</param>
        protected internal LogicalVector(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }

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
                    var data = GetFunction<LOGICAL_ELT>()(this.DangerousGetHandle(), (ulong)index);
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
                    //int offset = GetOffset(index);
                    int data = Convert.ToInt32(value);
                    //Marshal.WriteInt32(DataPointer, offset, data);
                    GetFunction<SET_LOGICAL_ELT>()(this.DangerousGetHandle(), (ulong)index, data);
                }
            }
        }

        /// <summary>
        /// Efficient conversion from R vector representation to the array equivalent in the CLR
        /// </summary>
        /// <returns>Array equivalent</returns>
        protected override bool[] GetArrayFast()
        {
            int[] intValues = new int[this.Length];
            Marshal.Copy(DataPointer, intValues, 0, intValues.Length);
            return Array.ConvertAll(intValues, Convert.ToBoolean);
        }

        /// <summary>
        /// Efficient initialisation of R vector values from an array representation in the CLR
        /// </summary>
        protected override void SetVectorDirect(bool[] values)
        {
            var intValues = Array.ConvertAll(values, Convert.ToInt32);
            Marshal.Copy(intValues, 0, DataPointer, values.Length);
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
    }
}