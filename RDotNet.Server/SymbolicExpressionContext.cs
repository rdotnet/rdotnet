using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using RDotNet.R.Adapter;

namespace RDotNet.Server
{
    [DataContract]
    public class SymbolicExpressionContext : IEquatable<SymbolicExpressionContext>
    {
        //NOTE: [RMEL] For testing.
        public SymbolicExpressionContext()
        { }

        public SymbolicExpressionContext(IntPtr handle)
        {
            if (handle == null) throw new ArgumentNullException("handle");

            Handle = handle;

            RawSEXP = (SEXPREC)Marshal.PtrToStructure(Handle, typeof (SEXPREC));
            Type = RawSEXP.sxpinfo.type;
        }

        [DataMember]
        public virtual IntPtr Handle { get; private set; }

        [DataMember]
        public virtual SymbolicExpressionType Type { get; private set; }

        [DataMember]
        public SEXPREC RawSEXP { get; private set; }

        public bool Equals(SymbolicExpressionContext other)
        {
            if (other == null) return false;
            return Handle == other.Handle;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SymbolicExpressionContext);
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }
    }
}
