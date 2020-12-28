using RDotNet.Internals;
using RDotNet.Utilities;
using System;
using System.Runtime.InteropServices;

namespace RDotNet
{
    /// <summary>
    /// A symbol object.
    /// </summary>
    public class Symbol : SymbolicExpression
    {
        /// <summary>
        /// Creates a symbol.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="pointer">The pointer.</param>
        protected internal Symbol(REngine engine, IntPtr pointer)
            : base(engine, pointer)
        { }

        /// <summary>
        /// Gets and sets the name.
        /// </summary>
        public string PrintName
        {
            get
            {
                dynamic sexp = GetInternalStructure();
                return new InternalString(Engine, sexp.symsxp.pname).ToString();
            }
            set
            {
                IntPtr pointer = (value == null ? Engine.NilValue : new InternalString(Engine, value)).DangerousGetHandle();
                int offset = GetOffsetOf("pname");
                Marshal.WriteIntPtr(handle, offset, pointer);
            }
        }

        /// <summary>
        /// Gets the internal function.
        /// </summary>
        public SymbolicExpression Internal
        {
            get
            {
                dynamic sexp = GetInternalStructure();
                if (Engine.EqualsRNilValue((IntPtr)sexp.symsxp.value))
                {
                    return null;
                }
                return new SymbolicExpression(Engine, sexp.symsxp.@internal);
            }
        }

        /// <summary>
        /// Gets the symbol value.
        /// </summary>
        public SymbolicExpression Value
        {
            get
            {
                dynamic sexp = GetInternalStructure();
                if (Engine.EqualsRNilValue((IntPtr)sexp.symsxp.value))
                {
                    return null;
                }
                return new SymbolicExpression(Engine, sexp.symsxp.value);
            }
        }

        private int GetOffsetOf(string fieldName)
        {
            return Marshal.OffsetOf(this.Engine.GetSEXPRECType(), "u").ToInt32() + Marshal.OffsetOf(this.Engine.GetSymSxpType(), fieldName).ToInt32();
        }
    }
}