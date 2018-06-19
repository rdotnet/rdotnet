using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// A vector of S expressions
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class ExpressionVector : Vector<Expression>
    {
        /// <summary>
        /// Creates a new instance for an expression vector.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="coerced">The pointer to an expression vector.</param>
        internal ExpressionVector(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }

        /// <summary>
        /// Gets an array representation of a vector of SEXP in R. Note that the implementation cannot be particularly "fast" in spite of the name.
        /// </summary>
        /// <returns></returns>
        protected override Expression[] GetArrayFast()
        {
            var res = new Expression[this.Length];
            bool useAltRep = (Engine.Compatibility == REngine.CompatibilityMode.ALTREP);
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = (useAltRep ? GetValueAltRep(i) : GetValue(i));
            }
            return res;
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected override Expression GetValue(int index)
        {
            int offset = GetOffset(index);
            IntPtr pointer = Marshal.ReadIntPtr(DataPointer, offset);
            return new Expression(Engine, pointer);
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        protected override Expression GetValueAltRep(int index)
        {
            return GetValue(index);
        }

        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <remarks>Used for pre-R 3.5 </remarks>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to set</param>
        protected override void SetValue(int index, Expression value)
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
        protected override void SetValueAltRep(int index, Expression value)
        {
            SetValue(index, value);
        }

        /// <summary>
        /// Efficient initialisation of R vector values from an array representation in the CLR
        /// </summary>
        protected override void SetVectorDirect(Expression[] values)
        {
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