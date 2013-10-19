using System;
using System.Numerics;

namespace RDotNet
{
   /// <summary>
   /// An internal helper class to convert types of arrays, primarily for data operations necessary for .NET types to/from R concepts.
   /// </summary>
   internal static class Utility
   {
      public static T[] AddFirst<T>(T value, T[] array)
      {
         if (array == null)
         {
            return new[] { value };
         }
         var newArray = new T[array.Length + 1];
         newArray[0] = value;
         Array.Copy(array, 0, newArray, 1, array.Length);
         return newArray;
      }

      internal static bool CheckNil(this REngine engine, IntPtr pointer)
      {
         return engine.NilValue.DangerousGetHandle() == pointer;
      }

      internal static bool CheckUnbound(this REngine engine, IntPtr pointer)
      {
         return engine.UnboundValue.DangerousGetHandle() == pointer;
      }

      internal static U[,] ArrayConvertAll<T, U>(T[,] array, Func<T, U> fun)
      {
         int rows = array.GetLength(0);
         int cols = array.GetLength(1);
         U[,] res = new U[rows, cols];
         for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
               res[i, j] = fun(array[i, j]);
         return res;
      }

      internal static U[] ArrayConvertAllOneDim<T, U>(T[,] array, Func<T, U> fun)
      {
         int rows = array.GetLength(0);
         int cols = array.GetLength(1);
         U[] res = new U[rows * cols];
         for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
               res[rows * j + i] = fun(array[i, j]);
         return res;
      }

      // TODO: probably room for extension methods around Matrix inheritors
      internal static U[,] ArrayConvertAllTwoDim<T, U>(T[] array, Func<T, U> fun, int rows, int cols)
      {
         U[,] res = new U[rows,cols];
         for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
               res[i, j] = fun(array[rows * j + i]);
         return res;
      }

      internal static U[] ArrayConvertOneDim<U>(U[,] array)
      {
         return ArrayConvertAllOneDim(array, value => value);
      }

      // TODO: probably room for extension methods around Matrix inheritors
      internal static U[,] ArrayConvertAllTwoDim<U>(U[] array, int rows, int cols)
      {
         return ArrayConvertAllTwoDim(array, value => value, rows, cols);
      }

      internal static double[] SerializeComplexToDouble(Complex[] values)
      {
         double[] data = new double[2 * values.Length];
         for (int i = 0; i < data.Length; i++)
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

      internal static T[] Subset<T>(T[] array, int from, int to)
      {
         var res = new T[(to - from) + 1];
         for (int i = 0; i < res.Length; i++)
            res[i] = array[from + i];
         return res;
      }
   }
}