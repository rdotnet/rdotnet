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
                    return GetValue(index);
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
                    SetValue(index, value);
                }
            }
        }

        /// <summary>
        /// Gets an array representation of this R vector. Note that the implementation is not as fast as for numeric vectors.
        /// </summary>
        /// <returns></returns>
        protected override string[] GetArrayFast()
        {
            int n = this.Length;
            string[] res = new string[n];
            for (int i = 0; i < n; i++)
                res[i] = GetValue(i);
            return res;
        }

        private string GetValue(int index)
        {
            int offset = GetOffset(index);
            IntPtr pointerItem = Marshal.ReadIntPtr(DataPointer, offset);
            if (pointerItem == Engine.NaStringPointer)
            {
                return null;
            }
            IntPtr pointer = IntPtr.Add(pointerItem, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            return InternalString.StringFromNativeUtf8(pointer);
        }

        private Rf_mkChar _mkChar = null;

        private IntPtr mkChar(string value)
        {
            if (_mkChar == null)
                _mkChar = this.GetFunction<Rf_mkChar>();
            return _mkChar(value);
        }

        private void SetValue(int index, string value)
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
            for (int i = 0; i < values.Length; i++)
                SetValue(i, values[i]);
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