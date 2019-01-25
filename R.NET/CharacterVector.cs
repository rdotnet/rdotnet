using RDotNet.Internals;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// A collection of strings.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class CharacterVector : Vector<string>
    {
        /// <summary>
        /// Creates a new empty CharacterVector with the specified length.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="length">The length.</param>
        /// <seealso cref="REngineExtension.CreateCharacterVector(REngine, int)"/>
        public CharacterVector(REngine engine, int length)
            : base(engine, SymbolicExpressionType.CharacterVector, length)
        { }

        /// <summary>
        /// Creates a new CharacterVector with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="vector">The values.</param>
        /// <seealso cref="REngineExtension.CreateCharacterVector(REngine, IEnumerable{string})"/>
        public CharacterVector(REngine engine, IEnumerable<string> vector)
            : base(engine, SymbolicExpressionType.CharacterVector, vector)
        { }

        /// <summary>
        /// Creates a new instance for a string vector.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="coerced">The pointer to a string vector.</param>
        protected internal CharacterVector(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }


        /// <summary>
        /// Gets an array representation of this R vector. Note that the implementation is not as fast as for numeric vectors.
        /// </summary>
        /// <returns></returns>
        protected override string[] GetArrayFast()
        {
            int n = this.Length;
            string[] res = new string[n];
            bool useAltRep = (Engine.Compatibility == REngine.CompatibilityMode.ALTREP);
            for (int i = 0; i < n; i++)
            {
                res[i] = (useAltRep ? GetValueAltRep(i) : GetValue(i));
            }
            return res;
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected override string GetValueAltRep(int index)
        {
            // To work with ALTREP (introduced in R 3.5.0) and non-ALTREP objects, we will get strings
            // via STRING_ELT, instead of offseting the DataPointer.  This lets R manage the details of
            // ALTREP conversion for us.
            IntPtr objPointer = GetFunction<STRING_ELT>()(this.DangerousGetHandle(), (IntPtr)index);
            if (objPointer == Engine.NaStringPointer)
            {
                return null;
            }

            IntPtr stringData = GetFunction<DATAPTR_OR_NULL>()(objPointer);
            return InternalString.StringFromNativeUtf8(stringData);
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected override string GetValue(int index)
        {
            int offset = GetOffset(index);
            IntPtr pointerItem = Marshal.ReadIntPtr(DataPointer, offset);
            if (pointerItem == Engine.NaStringPointer)
            {
                return null;
            }
            IntPtr pointer = IntPtr.Add(pointerItem, Marshal.SizeOf(typeof(Internals.PreALTREP.VECTOR_SEXPREC)));
            return InternalString.StringFromNativeUtf8(pointer);
        }

        /// <summary> Gets alternate rep array.</summary>
        ///
        /// <exception cref="NotSupportedException"> Thrown when the requested operation is not supported.</exception>
        ///
        /// <returns> An array of t.</returns>
        public override string[] GetAltRepArray()
        {
            return GetArrayFast();
        }

        private Rf_mkChar _mkChar = null;

        private IntPtr mkChar(string value)
        {
            if (_mkChar == null)
                _mkChar = this.GetFunction<Rf_mkChar>();
            return _mkChar(value);
        }

        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to set</param>
        protected override void SetValueAltRep(int index, string value)
        {
            SetValue(index, value);
        }

        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to set</param>
        protected override void SetValue(int index, string value)
        {
            int offset = GetOffset(index);
            IntPtr stringPointer = value == null ? Engine.NaStringPointer : mkChar(value);
            Marshal.WriteIntPtr(DataPointer, offset, stringPointer);
        }

        /// <summary>
        /// Efficient initialisation of R vector values from an array representation in the CLR
        /// </summary>
        protected override void SetVectorDirect(string[] values)
        {
            // Possibly not the fastest implementation, but faster may require C code.
            // TODO check the behavior of P/Invoke on array of strings (VT_ARRAY|VT_LPSTR?)
            bool useAltRep = (Engine.Compatibility == REngine.CompatibilityMode.ALTREP);
            for (int i = 0; i < values.Length; i++)
            {
                if (useAltRep)
                {
                    SetValueAltRep(i, values[i]);
                }
                {
                    SetValue(i, values[i]);
                }
            }
        }

        /// <summary>
        /// Gets the size of a pointer in byte.
        /// </summary>
        protected override int DataSize
        {
            get { return Marshal.SizeOf(typeof(IntPtr)); }
        }
    }
}