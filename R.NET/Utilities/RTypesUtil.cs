using System;
using System.Numerics;

namespace RDotNet.Utilities
{
    /// <summary>
    /// An internal helper class to convert types of arrays, primarily for data operations necessary for .NET types to/from R concepts.
    /// </summary>
    internal static class RTypesUtil
    {
        internal static double[] SerializeComplexToDouble(Complex[] values)
        {
            var data = new double[2 * values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                data[2 * i] = values[i].Real;
                data[2 * i + 1] = values[i].Imaginary;
            }
            return data;
        }

        internal static Complex[] DeserializeComplexFromDouble(double[] data)
        {
            int dblLen = data.Length;
            if (dblLen % 2 != 0) throw new ArgumentException("Serialized definition of complexes must be of even length");
            int n = dblLen / 2;
            var res = new Complex[n];
            for (int i = 0; i < n; i++)
                res[i] = new Complex(data[2 * i], data[2 * i + 1]);
            return res;
        }
    }
}