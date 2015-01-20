using System;
using System.Runtime.InteropServices;

namespace RDotNet.Graphics
{
    internal static class Utility
    {
        public static double ReadDouble(IntPtr pointer, int offset)
        {
            var value = new double[1];
            Marshal.Copy(IntPtr.Add(pointer, offset), value, 0, value.Length);
            return value[0];
        }
    }
}