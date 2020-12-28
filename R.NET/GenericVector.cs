using RDotNet.Dynamic;
using RDotNet.Internals;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// A generic list. This is also known as list in R.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class GenericVector : Vector<SymbolicExpression>
    {
        /// <summary>
        /// Creates a new empty GenericVector with the specified length.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="length">The length.</param>
        public GenericVector(REngine engine, int length)
            : base(engine, engine.GetFunction<Rf_allocVector>()(SymbolicExpressionType.List, length))
        { }

        /// <summary>
        /// Creates a new GenericVector with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="list">The values.</param>
        public GenericVector(REngine engine, IEnumerable<SymbolicExpression> list)
            : base(engine, SymbolicExpressionType.List, list)
        { }

        /// <summary>
        /// Creates a new instance for a list.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="coerced">The pointer to a list.</param>
        protected internal GenericVector(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected override SymbolicExpression GetValue(int index)
        {
            int offset = GetOffset(index);
            IntPtr pointer = Marshal.ReadIntPtr(DataPointer, offset);
            return new SymbolicExpression(Engine, pointer);
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected override SymbolicExpression GetValueAltRep(int index)
        {
            return GetValue(index);
        }

        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to set</param>
        protected override void SetValue(int index, SymbolicExpression value)
        {
            int offset = GetOffset(index);
            Marshal.WriteIntPtr(DataPointer, offset, (value ?? Engine.NilValue).DangerousGetHandle());
        }

        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to set</param>
        protected override void SetValueAltRep(int index, SymbolicExpression value)
        {
            SetValue(index, value);
        }

        /// <summary>
        /// Efficient conversion from R vector representation to the array equivalent in the CLR
        /// </summary>
        /// <returns>Array equivalent</returns>
        protected override SymbolicExpression[] GetArrayFast()
        {
            var res = new SymbolicExpression[this.Length];
            bool useAltRep = (Engine.Compatibility == REngine.CompatibilityMode.ALTREP);
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = (useAltRep ? GetValueAltRep(i) : GetValue(i));
            }
            return res;
        }

        /// <summary>
        /// Efficient initialisation of R vector values from an array representation in the CLR
        /// </summary>
        protected override void SetVectorDirect(SymbolicExpression[] values)
        {
            bool useAltRep = (Engine.Compatibility == REngine.CompatibilityMode.ALTREP);
            for (int i = 0; i < values.Length; i++)
            {
                if (useAltRep)
                {
                    SetValueAltRep(i, values[i]);
                }
                else
                {
                    SetValue(i, values[i]);
                }
            }
        }

        /// <summary>
        /// Gets the size of each item in this vector
        /// </summary>
        protected override int DataSize
        {
            get { return Marshal.SizeOf(typeof(IntPtr)); }
        }

        /// <summary>
        /// Converts into a <see cref="RDotNet.Pairlist"/>.
        /// </summary>
        /// <returns>The pairlist.</returns>
        public Pairlist ToPairlist()
        {
            return new Pairlist(Engine, this.GetFunction<Rf_VectorToPairList>()(handle));
        }

        /// <summary>
        /// returns a new ListDynamicMeta for this Generic Vector
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            return new ListDynamicMeta(parameter, this);
        }

        /// <summary> Sets the names of the vector. </summary>
        ///
        /// <param name="names"> A variable-length parameters list containing names.</param>
        public void SetNames(params string[] names)
        {
            CharacterVector cv = new CharacterVector(this.Engine, names);
            SetNames(cv);
        }

        /// <summary> Sets the names of the vector.</summary>
        ///
        /// <exception cref="ArgumentException"> Incorrect length, not equal to vector length</exception>
        ///
        /// <param name="names"> A variable-length parameters list containing names.</param>
        public void SetNames(CharacterVector names)
        {
            if (names.Length != this.Length)
            {
                throw new ArgumentException("Names vector must be same length as list");
            }
            SymbolicExpression namesSymbol = Engine.GetPredefinedSymbol("R_NamesSymbol");
            SetAttribute(namesSymbol, names);
        }
    }
}