using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter
{
    [StructLayout(LayoutKind.Sequential)]
    public struct sxpinfo
    {
        private readonly uint bits;

        public SymbolicExpressionType type
        {
            get { return (SymbolicExpressionType)(bits & 31u); }
        }

        public uint obj
        {
            get { return ((bits & 32u)/32); }
        }

        public uint named
        {
            get { return ((bits & 192u)/64); }
        }

        public uint gp
        {
            get { return ((bits & 16776960u)/256); }
        }

        public uint mark
        {
            get { return ((bits & 16777216u)/16777216); }
        }

        public uint debug
        {
            get { return ((bits & 33554432u)/33554432); }
        }

        public uint trace
        {
            get { return ((bits & 67108864u)/67108864); }
        }

        public uint spare
        {
            get { return ((bits & 134217728u)/134217728); }
        }

        public uint gcgen
        {
            get { return ((bits & 268435456u)/268435456); }
        }

        public uint gccls
        {
            get { return ((bits & 3758096384u)/536870912); }
        }
    }
}
