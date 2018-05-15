using RDotNet.Internals;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// A collection of real numbers in double precision.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class NumericVector : Vector<double>
    {
        /// <summary>
        /// Creates a new empty NumericVector with the specified length.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="length">The length.</param>
        /// <seealso cref="REngineExtension.CreateNumericVector(REngine, int)"/>
        public NumericVector(REngine engine, int length)
            : base(engine, SymbolicExpressionType.NumericVector, length)
        { }

        /// <summary>
        /// Creates a new NumericVector with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="vector">The values.</param>
        /// <seealso cref="REngineExtension.CreateNumericVector(REngine, IEnumerable{double})"/>
        public NumericVector(REngine engine, IEnumerable<double> vector)
            : base(engine, SymbolicExpressionType.NumericVector, vector)
        { }

        /// <summary>
        /// Creates a new NumericVector with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="vector">The values.</param>
        /// <seealso cref="REngineExtension.CreateNumericVector(REngine, IEnumerable{double})"/>
        public NumericVector(REngine engine, double[] vector)
            : base(engine, SymbolicExpressionType.NumericVector, vector.Length)
        {
            Marshal.Copy(vector, 0, DataPointer, vector.Length);
        }

        /// <summary>
        /// Creates a new instance for a numeric vector.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="coerced">The pointer to a numeric vector.</param>
        protected internal NumericVector(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }

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
                    return GetFunction<REAL_ELT>()(this.DangerousGetHandle(), (ulong)index);
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
                    GetFunction<SET_REAL_ELT>()(this.DangerousGetHandle(), (ulong)index, value);
                }
            }
        }

        /// <summary>
        /// Efficient conversion from R vector representation to the array equivalent in the CLR
        /// </summary>
        /// <returns>Array equivalent</returns>
        protected override double[] GetArrayFast()
        {
            var res = new double[this.Length];
            Marshal.Copy(DataPointer, res, 0, res.Length);
            return res;
        }

        /// <summary> Gets alternate rep array.</summary>
        ///
        /// <exception cref="NotSupportedException"> Thrown when the requested operation is not supported.</exception>
        ///
        /// <returns> An array of t.</returns>
        public override double[] GetAltRepArray()
        {
            // by inference from `static SEXP compact_intseq_Duplicate(SEXP x, Rboolean deep)`  in altrep.c
            var res = new double[this.Length];
            GetFunction<REAL_GET_REGION>()(this.DangerousGetHandle(), (ulong)0, (ulong)this.Length, res);
            return res;
        }


        /// <summary>
        /// Efficient initialisation of R vector values from an array representation in the CLR
        /// </summary>
        protected override void SetVectorDirect(double[] values)
        {
            IntPtr pointer = IntPtr.Add(DataPointer, 0);
            Marshal.Copy(values, 0, pointer, values.Length);
        }

        /// <summary>
        /// Gets the size of a real number in byte.
        /// </summary>
        protected override int DataSize
        {
            get { return sizeof(double); }
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