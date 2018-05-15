using System;
using System.Numerics;

namespace RDotNet.Utilities
{
    /// <summary>
    /// An internal helper class to convert types of arrays, primarily for data operations necessary for .NET types to/from R concepts.
    /// </summary>
    public static class RTypesUtil
    {
        /// <summary> Serialize an array of complex numbers to 
        ///           an array of doubles, alternating real and imaginary values</summary>
        ///
        /// <param name="values"> The complex values to serialize</param>
        ///
        /// <returns> A double[].</returns>
        public static double[] SerializeComplexToDouble(Complex[] values)
        {
            var data = new double[2 * values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                data[2 * i] = values[i].Real;
                data[2 * i + 1] = values[i].Imaginary;
            }
            return data;
        }

        /// <summary> Deserialize complex from double.</summary>
        ///
        /// <exception cref="ArgumentException"> input length is not divisible by 2 </exception>
        ///
        /// <param name="data"> The serialised complex values, even indexes are real and odd ones imaginary</param>
        ///
        /// <returns> A Complex[].</returns>
        public static Complex[] DeserializeComplexFromDouble(double[] data)
        {
            int dblLen = data.Length;
            if (dblLen % 2 != 0) throw new ArgumentException("Serialised definition of complexes must be of even length");
            int n = dblLen / 2;
            var res = new Complex[n];
            for (int i = 0; i < n; i++)
                res[i] = new Complex(data[2 * i], data[2 * i + 1]);
            return res;
        }
    }
}