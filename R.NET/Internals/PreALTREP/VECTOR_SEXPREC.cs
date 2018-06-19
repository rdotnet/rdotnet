using System;
using System.Runtime.InteropServices;

namespace RDotNet.Internals.PreALTREP
{
    // In R 3.5, the length & true length values went from 32 to 64 bits in length.  These are defined in R as R_xlen_t
    // (previously R_len_t) - https://github.com/wch/r-source/blob/trunk/src/include/Rinternals.h

    [StructLayout(LayoutKind.Sequential)]
    internal struct VECTOR_SEXPREC
    {
        private SEXPREC_HEADER header;
        private vecsxp vecsxp;

        public sxpinfo sxpinfo
        {
            get
            {
                return header.sxpinfo;
            }
        }

        public IntPtr attrib
        {
            get
            {
                return header.attrib;
            }
        }

        public IntPtr gengc_next_node
        {
            get
            {
                return header.gengc_next_node;
            }
        }

        public IntPtr gengc_prev_node
        {
            get
            {
                return header.gengc_prev_node;
            }
        }

        public long Length
        {
            get
            {
                return vecsxp.length;
            }
        }

        public long TrueLength
        {
            get
            {
                return vecsxp.truelength;
            }
        }
    }
}