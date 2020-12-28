using System.Runtime.InteropServices;

namespace RDotNet.Internals.ALTREP
{
    // Definition of the struct available at: https://cran.r-project.org/doc/manuals/r-release/R-ints.html#Rest-of-header
    // Formally defined in Rinternals.h: https://github.com/wch/r-source/blob/trunk/src/include/Rinternals.h
    // Note that this structure was greatly changed in the R 3.5 release, using the platform-dependent pointer size (represented
    //   here as IntPtr), with fields added and the order changed.
    [StructLayout(LayoutKind.Sequential)]
    internal struct sxpinfo
    {
        private ulong bits;

        private const int NAMED_BITS = 16;

        public SymbolicExpressionType type  // 5 bits
        {
            get { return (SymbolicExpressionType)(this.bits & 31u); }
        }

        public uint scalar    // 1 bit
        {
            get { return (uint)((this.bits & 32u) / 32); }
        }

        public uint obj   // 1 bit
        {
            get { return (uint)((this.bits & 64u) / 64); }
        }

        public uint alt   // 1 bit
        {
            get { return (uint)((this.bits & 128u) / 128); }
        }

        public uint gp    // 16 bits
        {
            get { return (uint)((this.bits & 16776960u) / 256); }
        }

        public uint mark  // 1 bit
        {
            get { return (uint)((this.bits & 16777216u) / 16777216); }
        }

        public uint debug // 1 bit
        {
            get { return (uint)((this.bits & 33554432u) / 33554432); }
        }

        public uint trace // 1 bit
        {
            get { return (uint)((this.bits & 67108864u) / 67108864); }
        }

        public uint spare // 1 bit
        {
            get { return (uint)((this.bits & 134217728u) / 134217728); }
        }

        public uint gcgen // 1 bit
        {
            get { return (uint)((this.bits & 268435456u) / 268435456); }
        }

        public uint gccls // 3 bits
        {
            get { return (uint)((this.bits & 3758096384u) / 536870912); }
        }

        public uint named // NAMED_BITS
        {
            get { return (uint)((this.bits & 281470681743360u) / 4294967296); }
        }

        public uint extra // 32 - NAMED_BITS
        {
            get { return (uint)((this.bits & 18446462598732800000u) / 281474976710656); }
        }
    }
}