using System;
using System.Runtime.InteropServices;

namespace RDotNet.Internals
{
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

		public int Length
		{
			get
			{
				return vecsxp.length;
			}
		}

		public int TrueLength
		{
			get
			{
				return vecsxp.truelength;
			}
		}
	}
}
