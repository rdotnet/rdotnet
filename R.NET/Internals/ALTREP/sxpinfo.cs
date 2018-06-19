using System.Runtime.InteropServices;

namespace RDotNet.Internals.ALTREP
{
    // Definition of the struct available at: https://cran.r-project.org/doc/manuals/r-release/R-ints.html#Rest-of-header
    // Formally defined in Rinternals.h: https://github.com/wch/r-source/blob/trunk/src/include/Rinternals.h
    // Note that this structure was greatly changed in the R 3.5 release, going from 32 bits to 64 bits, with fields added
    //   and the order changed.
    [StructLayout(LayoutKind.Sequential)]
    internal struct sxpinfo
    {
        private ulong bits;

        private const int NAMED_BITS = 16;

        public SymbolicExpressionType type  // 5 bits
        {
            get { return (SymbolicExpressionType)(this.bits & 31u); }
        }

        public ulong scalar    // 1 bit
        {
            get { return ((this.bits & 32u) / 32); }
        }

        public ulong obj   // 1 bit
        {
            get { return ((this.bits & 64u) / 64); }
        }

        public ulong alt   // 1 bit
        {
            get { return ((this.bits & 128u) / 128); }
        }

        public ulong gp    // 16 bits
        {
            get { return ((this.bits & 16776960u) / 256); }
        }

        public ulong mark  // 1 bit
        {
            get { return ((this.bits & 16777216u) / 16777216); }
        }

        public ulong debug // 1 bit
        {
            get { return ((this.bits & 33554432u) / 33554432); }
        }

        public ulong trace // 1 bit
        {
            get { return ((this.bits & 67108864u) / 67108864); }
        }

        public ulong spare // 1 bit
        {
            get { return ((this.bits & 134217728u) / 134217728); }
        }

        public ulong gcgen // 1 bit
        {
            get { return ((this.bits & 268435456u) / 268435456); }
        }

        public ulong gccls // 3 bits
        {
            get { return ((this.bits & 3758096384u) / 536870912); }
        }

        public ulong named // NAMED_BITS
        {
            get { return ((this.bits & 281470681743360u) / 4294967296); }
        }

        public ulong extra // 32 - NAMED_BITS
        {
            get { return ((this.bits & 18446462598732800000u) / 281474976710656); }
        }
    }
}