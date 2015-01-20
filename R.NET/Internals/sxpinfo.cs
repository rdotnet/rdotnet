using System.Runtime.InteropServices;

namespace RDotNet.Internals
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct sxpinfo
    {
        private uint bits;

        public SymbolicExpressionType type
        {
            get { return (SymbolicExpressionType)(this.bits & 31u); }
        }

        public uint obj
        {
            get { return ((this.bits & 32u) / 32); }
        }

        public uint named
        {
            get { return ((this.bits & 192u) / 64); }
        }

        public uint gp
        {
            get { return ((this.bits & 16776960u) / 256); }
        }

        public uint mark
        {
            get { return ((this.bits & 16777216u) / 16777216); }
        }

        public uint debug
        {
            get { return ((this.bits & 33554432u) / 33554432); }
        }

        public uint trace
        {
            get { return ((this.bits & 67108864u) / 67108864); }
        }

        public uint spare
        {
            get { return ((this.bits & 134217728u) / 134217728); }
        }

        public uint gcgen
        {
            get { return ((this.bits & 268435456u) / 268435456); }
        }

        public uint gccls
        {
            get { return ((this.bits & 3758096384u) / 536870912); }
        }
    }
}