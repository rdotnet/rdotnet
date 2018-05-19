using System.Data;
using RDotNet.Diagnostics;
using RDotNet.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// A vector base.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    [DebuggerDisplay("Length = {Length}; RObjectType = {Type}")]
    [DebuggerTypeProxy(typeof(VectorDebugView<>))]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public abstract class Vector<T> : SymbolicExpression, IEnumerable<T>
    {
        /// <summary>
        /// Creates a new vector with the specified size.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="type">The element type.</param>
        /// <param name="length">The length of vector.</param>
        protected Vector(REngine engine, SymbolicExpressionType type, int length)
            : base(engine, engine.GetFunction<Rf_allocVector>()(type, length))
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            var empty = new byte[length * DataSize];
            Marshal.Copy(empty, 0, DataPointer, empty.Length);
        }

        /// <summary>
        /// Creates a new vector with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="type">The element type.</param>
        /// <param name="vector">The elements of vector.</param>
        protected Vector(REngine engine, SymbolicExpressionType type, IEnumerable<T> vector)
            : base(engine, engine.GetFunction<Rf_allocVector>()(type, vector.Count()))
        {
            SetVector(vector.ToArray());
        }

        /// <summary>
        /// Creates a new instance for a vector.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="coerced">The pointer to a vector.</param>
        protected Vector(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public virtual T this[int index]
        {
            get
            {
                if (index < 0 || Length <= index)
                {
                    throw new ArgumentOutOfRangeException();
                }
                using (new ProtectedPointer(this))
                {
                    switch (Engine.Compatibility)
                    {
                        case REngine.CompatibilityMode.ALTREP:
                            return GetValueAltRep(index);
                        case REngine.CompatibilityMode.PreALTREP:
                            return GetValue(index);
                        default:
                            throw new Exception("Unable to access the vector element for this unknown R compatibility mode");
                    }
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
                    switch (Engine.Compatibility)
                    {
                        case REngine.CompatibilityMode.ALTREP:
                            SetValueAltRep(index, value);
                            break;
                        case REngine.CompatibilityMode.PreALTREP:
                            SetValue(index, value);
                            break;
                        default:
                            throw new Exception("Unable to set the vector element for this unknown R compatibility mode");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected virtual T GetValue(int index)
        {
            throw new NotSupportedException("GetValue handling not yet supported");
        }

        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to set</param>
        protected virtual void SetValue(int index, T value)
        {
            throw new NotSupportedException("SetValue handling not yet supported");
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected virtual T GetValueAltRep(int index)
        {
            throw new NotSupportedException("GetValueAltRep handling not yet supported");
        }

        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to set</param>
        protected virtual void SetValueAltRep(int index, T value)
        {
            throw new NotSupportedException("SetValueAltRep handling not yet supported");
        }


        /// <summary>
        /// Initializes the content of a vector with runtime speed in mind. This method protects the R vector, then call SetVectorDirect.
        /// </summary>
        /// <param name="values">The values to put in the vector. Length must match exactly the vector size</param>
        public void SetVector(T[] values)
        {
            if (values.Length != this.Length)
                throw new ArgumentException("The length of the array provided differs from the vector length");
            using (new ProtectedPointer(this))
            {
                SetVectorDirect(values);
            }
        }

        /// <summary>
        /// A method to transfer data from native to .NET managed array equivalents fast.
        /// </summary>
        /// <returns>Array of values in the vector</returns>
        public T[] ToArray()
        {
            using (new ProtectedPointer(this))
            {
                /* 
                 * as of R 3.5 DataPointer may return null if the vector 
                 * is an alternate representation
                 * See section General Vectors in  
                 * https://svn.r-project.org/R/branches/ALTREP/ALTREP.html
                 * 
                 * If we are in compatibility mode for pre-ALTREP, we will always
                 * use the fast method.
                 */
                if (Engine.Compatibility == REngine.CompatibilityMode.PreALTREP
                    || DataPointer != IntPtr.Zero)
                {
                    return GetArrayFast();
                }
                else
                {
                    return GetAltRepArray();
                }
            }
        }

        /// <summary> Gets alternate rep array.</summary>
        ///
        /// <exception cref="NotSupportedException"> Thrown when the requested operation is not supported.</exception>
        ///
        /// <returns> An array of t.</returns>
        public virtual T[] GetAltRepArray()
        {
            throw new NotSupportedException("ALTVEC handling not yet supported");
        }

        /// <summary>
        /// Gets a representation as a one dimensional array of an R vector, with efficiency in mind for the unmanaged to managed transition, if possible.
        /// </summary>
        /// <returns></returns>
        protected abstract T[] GetArrayFast();

        /// <summary>
        /// Initializes the content of a vector with runtime speed in mind. The vector must already be protected before calling this method.
        /// </summary>
        /// <param name="values">The values to put in the vector. Length must match exactly the vector size</param>
        protected abstract void SetVectorDirect(T[] values);

        /// <summary>
        /// Gets or sets the element at the specified name.
        /// </summary>
        /// <param name="name">The name of the element to get or set.</param>
        /// <returns>The element at the specified name.</returns>
        public virtual T this[string name]
        {
            get
            {
                int index = getIndex(name);
                return this[index];
            }
            set
            {
                int index = getIndex(name);
                this[index] = value;
            }
        }

        private int getIndex(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name", "indexing a vector by name requires a non-null name argument");
            string[] names = Names;
            if (names == null)
                throw new NotSupportedException("The vector has no names defined - indexing it by name cannot be supported");
            int index = Array.IndexOf(names, name);
            return index;
        }

        /// <summary>
        /// Gets the number of elements.
        /// </summary>
        public int Length
        {
            get { return this.GetFunction<Rf_length>()(handle); }
        }

        /// <summary>
        /// Gets the names of elements.
        /// </summary>
        public string[] Names
        {
            get
            {
                SymbolicExpression namesSymbol = Engine.GetPredefinedSymbol("R_NamesSymbol");
                SymbolicExpression names = GetAttribute(namesSymbol);
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
                var result = new string[length];
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
                switch (Engine.Compatibility)
                {
                    case REngine.CompatibilityMode.ALTREP:
                        return GetFunction<DATAPTR_OR_NULL>()(this.DangerousGetHandle());
                    case REngine.CompatibilityMode.PreALTREP:
                        return IntPtr.Add(handle, Marshal.SizeOf(typeof(Internals.PreALTREP.VECTOR_SEXPREC)));
                    default:
                        throw new MemberAccessException("Unable to translate the DataPointer for this R compatibility mode");
                }
            }
        }

        /// <summary>
        /// Gets the size of an element in byte.
        /// </summary>
        protected abstract int DataSize { get; }

        #region IEnumerable<T> Members

        /// <summary>
        /// Gets enumerator
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            for (int index = 0; index < Length; index++)
            {
                yield return this[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable<T> Members

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
            if (sourceIndex < 0 || Length < sourceIndex + length)
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

        /// <summary>
        /// Gets the offset for the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The offset.</returns>
        protected int GetOffset(int index)
        {
            return DataSize * index;
        }
    }
}