using System.Runtime.InteropServices;

namespace RDotNet.Internals.PreALTREP
{
    // Definition of the struct available at: https://cran.r-project.org/doc/manuals/r-release/R-ints.html#Rest-of-header
    // Formally defined in Rinternals.h: https://github.com/wch/r-source/blob/trunk/src/include/Rinternals.h
    // Note that this structure was greatly changed in the R 3.5 release, using the platform-dependent pointer size (represented
    //   here as IntPtr), with fields added and the order changed.
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