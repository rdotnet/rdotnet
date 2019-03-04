using RDotNet.Internals;
using RDotNet.Utilities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// A collection of complex numbers.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class ComplexVector : Vector<Complex>
    {
        /// <summary>
        /// Creates a new empty ComplexVector with the specified length.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="length">The length.</param>
        /// <seealso cref="REngineExtension.CreateComplexVector(REngine, int)"/>
        public ComplexVector(REngine engine, int length)
            : base(engine, SymbolicExpressionType.ComplexVector, length)
        { }

        /// <summary>
        /// Creates a new ComplexVector with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="vector">The values.</param>
        /// <seealso cref="REngineExtension.CreateComplexVector(REngine, System.Collections.Generic.IEnumerable{System.Numerics.Complex})"/>
        public ComplexVector(REngine engine, IEnumerable<Complex> vector)
            : base(engine, SymbolicExpressionType.ComplexVector, vector)
        { }

        /// <summary>
        /// Creates a new instance for a complex number vector.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="coerced">The pointer to a complex number vector.</param>
        protected internal ComplexVector(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }
        
        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected override Complex GetValue(int index)
        {
            var data = new double[2];
            int offset = GetOffset(index);
            IntPtr pointer = IntPtr.Add(DataPointer, offset);
            Marshal.Copy(pointer, data, 0, data.Length);
            return new Complex(data[0], data[1]);
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected override Complex GetValueAltRep(int index)
        {
            var data = GetFunction<COMPLEX_ELT>()(this.DangerousGetHandle(), (IntPtr)index);
            return new Complex(data.r, data.i);
        }

        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to set</param>
        protected override void SetValue(int index, Complex value)
        {
            var data = new[] { value.Real, value.Imaginary };
            int offset = GetOffset(index);
            IntPtr pointer = IntPtr.Add(DataPointer, offset);
            Marshal.Copy(data, 0, pointer, data.Length);
        }

        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to set</param>
        protected override void SetValueAltRep(int index, Complex value)
        {
            GetFunction<SET_COMPLEX_ELT>()(this.DangerousGetHandle(), (IntPtr)index,
                        RTypesUtil.SerializeComplexToRComplex(value));
        }

        /// <summary>
        /// Gets an array representation in the CLR of a vector in R.
        /// </summary>
        /// <returns></returns>
        protected override Complex[] GetArrayFast()
        {
            int n = this.Length;
            var data = new double[2 * n];
            Marshal.Copy(DataPointer, data, 0, 2 * n);
            return RTypesUtil.DeserializeComplexFromDouble(data);
        }

        /// <summary>
        /// Efficient initialisation of R vector values from an array representation in the CLR
        /// </summary>
        protected override void SetVectorDirect(Complex[] values)
        {
            double[] data = RTypesUtil.SerializeComplexToDouble(values);
            IntPtr pointer = IntPtr.Add(DataPointer, 0);
            Marshal.Copy(data, 0, pointer, data.Length);
        }

        /// <summary>
        /// Gets the size of a complex number in byte.
        /// </summary>
        protected override int DataSize
        {
            get { return Marshal.SizeOf(typeof(Complex)); }
        }
    }
}