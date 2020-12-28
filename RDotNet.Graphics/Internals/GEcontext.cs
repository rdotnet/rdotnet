using System.Runtime.InteropServices;

namespace RDotNet.Graphics.Internals
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct GEcontext
    {
        internal Color col;
        internal Color fill;
        internal double gamma;
        internal double lwd;
        internal LineType lty;
        internal LineEnd lend;
        internal LineJoin ljoin;
        internal double lmitre;
        internal double cex;
        internal double ps;
        internal double lineheight;
        internal FontFace fontface;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
        internal string fontfamily;
    }
}