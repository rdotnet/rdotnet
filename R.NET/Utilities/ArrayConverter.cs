using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDotNet.Utilities
{
    /// <summary>
    /// A set of helpers to convert, query and transform arrays
    /// </summary>
    public static class ArrayConverter
    {
        /// <summary>
        /// Convert all elements of a rectangular array, using a function to cast/transform each element.
        /// </summary>
        /// <typeparam name="T">The type of the input array elements</typeparam>
        /// <typeparam name="U">The type of the output array elements</typeparam>
        /// <param name="array">Input array</param>
        /// <param name="fun">A conversion function taking in an object of type T and returning one of type U</param>
        /// <returns></returns>
        public static U[,] ArrayConvertAll<T, U>(T[,] array, Func<T, U> fun)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            U[,] res = new U[rows, cols];
            for (int i = 0; i < rows; i++) // TODO: what is the best indexing order to avoid memory cache misses, rowfirst or colfirst?
                for (int j = 0; j < cols; j++)
                    res[i, j] = fun(array[i, j]);
            return res;
        }

        /// <summary>
        /// Convert all elements of a rectangular, jagged array, using a function to cast/transform each element.
        /// </summary>
        /// <typeparam name="T">The type of the input array elements</typeparam>
        /// <typeparam name="U">The type of the output array elements</typeparam>
        /// <param name="array">Input array</param>
        /// <param name="fun">A conversion function taking in an object of type T and returning one of type U</param>
        /// <returns></returns>
        public static U[][] ArrayConvertAll<T, U>(T[][] array, Func<T, U> fun)
        {
            int rows = array.Length;
            int cols = 0;
            if (rows == 0)
                return CreateMatrixJagged<U>(rows, cols);
            else
                cols = array[0].Length;
            U[][] res = CreateMatrixJagged<U>(rows, cols);
            for (int i = 0; i < rows; i++)
            {
                if (array[i].Length != cols)
                    throw new ArgumentException("Each element of the input jagged array must have the same length. Failed for index " + i.ToString());
                for (int j = 0; j < cols; j++)
                    res[i][j] = fun(array[i][j]);
            }
            return res;
        }

        public static T[][] CreateMatrixJagged<T>(int outerDim, int innerDim)
        {
            var result = new T[outerDim][];
            for (int i = 0; i < outerDim; i++)
                result[i] = new T[innerDim];
            return result;
        }

        /// <summary>
        /// Convert all elements of a rectangular array to a vector, using a function to cast/transform each element. 
        /// The dimension reduction is column-first, appending each line of the input array into the result vector.
        /// </summary>
        /// <typeparam name="T">The type of the input array elements</typeparam>
        /// <typeparam name="U">The type of the output array elements</typeparam>
        /// <param name="array">Input array</param>
        /// <param name="fun">A conversion function taking in an object of type T and returning one of type U</param>
        /// <returns></returns>
        public static U[] ArrayConvertAllOneDim<T, U>(T[,] array, Func<T, U> fun)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            U[] res = new U[rows * cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    res[rows * j + i] = fun(array[i, j]);
            return res;
        }

        /// <summary>
        /// Convert a rectangular array to a vector. 
        /// The dimension reduction is column-first, appending each line of the input array into the result vector.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static U[] ArrayConvertOneDim<U>(U[,] array)
        {
            return ArrayConvertAllOneDim(array, value => value);
        }


        // TODO: probably room for extension methods around Matrix inheritors
        /// <summary>
        /// Convert all elements of a vector into a rectangular array, using a function to cast/transform each element. 
        /// Vector to matrix augmentation is done column first, i.e. "appending" successive lines to the bottom of the new matrix
        /// </summary>
        /// <typeparam name="T">The type of the input array elements</typeparam>
        /// <typeparam name="U">The type of the output array elements</typeparam>
        /// <param name="array">Input array</param>
        /// <param name="fun">A conversion function taking in an object of type T and returning one of type U</param>
        /// <param name="rows">The number of rows in the output</param>
        /// <param name="cols">The number of columns in the output</param>
        /// <returns></returns>
        public static U[,] ArrayConvertAllTwoDim<T, U>(T[] array, Func<T, U> fun, int rows, int cols)
        {
            if (cols < 0) throw new ArgumentException("negative number for column numbers");
            if (rows < 0) throw new ArgumentException("negative number for row numbers");
            if (array.Length < (rows * cols)) throw new ArgumentException("input array has less than rows*cols elements");
            U[,] res = new U[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    res[i, j] = fun(array[rows * j + i]);
            return res;
        }

        // TODO: probably room for extension methods around Matrix inheritors
        /// <summary>
        /// Converts a vector into a rectangular array. 
        /// Vector to matrix augmentation is done column first, i.e. "appending" successive lines to the bottom of the new matrix
        /// </summary>
        /// <typeparam name="U">The type of the output array elements</typeparam>
        /// <param name="array">Input array</param>
        /// <param name="rows">The number of rows in the output</param>
        /// <param name="cols">The number of columns in the output</param>
        /// <returns></returns>
        public static U[,] ArrayConvertAllTwoDim<U>(U[] array, int rows, int cols)
        {
            return ArrayConvertAllTwoDim(array, value => value, rows, cols);
        }

        /// <summary>
        /// Subset an array
        /// </summary>
        /// <typeparam name="T">The type of the input array elements</typeparam>
        /// <param name="array">Input array</param>
        /// <param name="from">Index of the first element to subset</param>
        /// <param name="to">Index of the last element to subset</param>
        /// <returns></returns>
        public static T[] Subset<T>(T[] array, int from, int to)
        {
            if (from > to) throw new ArgumentException("Inconsistent subset: from > to");
            int count = (to - from) + 1;
            var res = new T[count];
            Array.Copy(array, from, res, 0, count);
            return res;
        }

        internal static T[] Prepend<T>(T value, T[] array)
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

    }
}
