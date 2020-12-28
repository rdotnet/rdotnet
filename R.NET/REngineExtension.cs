using RDotNet.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace RDotNet
{
    /// <summary>
    /// Provides extension methods for <see cref="REngine"/>.
    /// </summary>
    public static class REngineExtension
    {
        /// <summary>
        /// Creates a new empty CharacterVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static CharacterVector CreateCharacterVector(this REngine engine, int length)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new CharacterVector(engine, length);
        }

        /// <summary>
        /// Creates a new empty ComplexVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static ComplexVector CreateComplexVector(this REngine engine, int length)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new ComplexVector(engine, length);
        }

        /// <summary>
        /// Creates a new empty IntegerVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static IntegerVector CreateIntegerVector(this REngine engine, int length)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new IntegerVector(engine, length);
        }

        /// <summary>
        /// Creates a new empty LogicalVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static LogicalVector CreateLogicalVector(this REngine engine, int length)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new LogicalVector(engine, length);
        }

        /// <summary>
        /// Creates a new empty NumericVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static NumericVector CreateNumericVector(this REngine engine, int length)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new NumericVector(engine, length);
        }

        /// <summary>
        /// Creates a new empty RawVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static RawVector CreateRawVector(this REngine engine, int length)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new RawVector(engine, length);
        }

        /// <summary>
        /// Creates a new CharacterVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static CharacterVector CreateCharacterVector(this REngine engine, IEnumerable<string> vector)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new CharacterVector(engine, vector);
        }

        /// <summary>
        /// Creates a new ComplexVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static ComplexVector CreateComplexVector(this REngine engine, IEnumerable<Complex> vector)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new ComplexVector(engine, vector);
        }

        /// <summary>
        /// Creates a new IntegerVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static IntegerVector CreateIntegerVector(this REngine engine, IEnumerable<int> vector)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new IntegerVector(engine, vector);
        }

        /// <summary>
        /// Creates a new LogicalVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static LogicalVector CreateLogicalVector(this REngine engine, IEnumerable<bool> vector)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new LogicalVector(engine, vector);
        }

        /// <summary>
        /// Creates a new NumericVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static NumericVector CreateNumericVector(this REngine engine, IEnumerable<double> vector)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new NumericVector(engine, vector);
        }

        /// <summary>
        /// Creates a new RawVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static RawVector CreateRawVector(this REngine engine, IEnumerable<byte> vector)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new RawVector(engine, vector);
        }

        /// <summary>
        /// Create a vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static CharacterVector CreateCharacter(this REngine engine, string value)
        {
            return CreateCharacterVector(engine, new[] { value });
        }

        /// <summary>
        /// Create a vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static ComplexVector CreateComplex(this REngine engine, Complex value)
        {
            return CreateComplexVector(engine, new[] { value });
        }

        /// <summary>
        /// Create a vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static LogicalVector CreateLogical(this REngine engine, bool value)
        {
            return CreateLogicalVector(engine, new[] { value });
        }

        /// <summary>
        /// Create a vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static NumericVector CreateNumeric(this REngine engine, double value)
        {
            return CreateNumericVector(engine, new[] { value });
        }

        /// <summary>
        /// Create an integer vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static IntegerVector CreateInteger(this REngine engine, int value)
        {
            return CreateIntegerVector(engine, new[] { value });
        }

        /// <summary>
        /// Create a vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static RawVector CreateRaw(this REngine engine, byte value)
        {
            return CreateRawVector(engine, new[] { value });
        }

        /// <summary>
        /// Creates a new empty CharacterMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static CharacterMatrix CreateCharacterMatrix(this REngine engine, int rowCount, int columnCount)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new CharacterMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new empty ComplexMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static ComplexMatrix CreateComplexMatrix(this REngine engine, int rowCount, int columnCount)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new ComplexMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new empty IntegerMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static IntegerMatrix CreateIntegerMatrix(this REngine engine, int rowCount, int columnCount)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new IntegerMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new empty LogicalMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static LogicalMatrix CreateLogicalMatrix(this REngine engine, int rowCount, int columnCount)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new LogicalMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new empty NumericMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static NumericMatrix CreateNumericMatrix(this REngine engine, int rowCount, int columnCount)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new NumericMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new empty RawMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static RawMatrix CreateRawMatrix(this REngine engine, int rowCount, int columnCount)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new RawMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new CharacterMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static CharacterMatrix CreateCharacterMatrix(this REngine engine, string[,] matrix)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new CharacterMatrix(engine, matrix);
        }

        /// <summary>
        /// Creates a new ComplexMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static ComplexMatrix CreateComplexMatrix(this REngine engine, Complex[,] matrix)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new ComplexMatrix(engine, matrix);
        }

        /// <summary>
        /// Creates a new IntegerMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static IntegerMatrix CreateIntegerMatrix(this REngine engine, int[,] matrix)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new IntegerMatrix(engine, matrix);
        }

        /// <summary>
        /// Creates a new LogicalMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static LogicalMatrix CreateLogicalMatrix(this REngine engine, bool[,] matrix)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new LogicalMatrix(engine, matrix);
        }

        /// <summary>
        /// Creates a new NumericMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static NumericMatrix CreateNumericMatrix(this REngine engine, double[,] matrix)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new NumericMatrix(engine, matrix);
        }

        /// <summary>
        /// Creates a new RawMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static RawMatrix CreateRawMatrix(this REngine engine, byte[,] matrix)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new RawMatrix(engine, matrix);
        }

        /// <summary>
        /// Create an R data frame from managed arrays and objects.
        /// </summary>
        /// <param name="engine">R engine</param>
        /// <param name="columns">The columns with the values for the data frame. These must be array of supported types (double, string, bool, integer, byte)</param>
        /// <param name="columnNames">Column names. default: null.</param>
        /// <param name="rowNames">Row names. Default null.</param>
        /// <param name="checkRows">Check rows. See data.frame R documentation</param>
        /// <param name="checkNames">See data.frame R documentation</param>
        /// <param name="stringsAsFactors">Should columns of strings be considered as factors (categories). See data.frame R documentation</param>
        /// <returns></returns>
        public static DataFrame CreateDataFrame(this REngine engine, IEnumerable[] columns, string[] columnNames = null,
           string[] rowNames = null, bool checkRows = false, bool checkNames = true, bool stringsAsFactors = true)
        {
            var df = engine.GetSymbol("data.frame").AsFunction();
            SymbolicExpression[] colVectors = ToVectors(engine, columns);
            Tuple<string, SymbolicExpression>[] namedColArgs = CreateNamedArgs(colVectors, columnNames);
            var args = new List<Tuple<string, SymbolicExpression>>(namedColArgs);
            if (rowNames != null) args.Add(Tuple.Create("row.names", (SymbolicExpression)engine.CreateCharacterVector(rowNames)));
            args.Add(Tuple.Create("check.rows", (SymbolicExpression)engine.CreateLogical(checkRows)));
            args.Add(Tuple.Create("check.names", (SymbolicExpression)engine.CreateLogical(checkNames)));
            args.Add(Tuple.Create("stringsAsFactors", (SymbolicExpression)engine.CreateLogical(stringsAsFactors)));
            var result = df.InvokeNamed(args.ToArray()).AsDataFrame();
            return result;
        }

        private static Tuple<string, SymbolicExpression>[] CreateNamedArgs(SymbolicExpression[] colVectors, string[] columnNames)
        {
            if (columnNames != null && colVectors.Length != columnNames.Length)
                throw new ArgumentException("columnNames", "when not null, the number of column names must match the number of SEXP");
            var args = new List<Tuple<string, SymbolicExpression>>();
            for (int i = 0; i < colVectors.Length; i++)
                args.Add(Tuple.Create(columnNames != null ? columnNames[i] : "", colVectors[i]));
            return args.ToArray();
        }

        internal static SymbolicExpression[] ToVectors(REngine engine, IEnumerable[] columns)
        {
            return Array.ConvertAll(columns, x => ToVector(engine, x));
        }

        internal static SymbolicExpression ToVector(REngine engine, IEnumerable values)
        {
            if (values == null) throw new ArgumentNullException("values", "values to transform to an R vector must not be null");
            var ints = values as IEnumerable<int>;
            var chars = values as IEnumerable<string>;
            var cplxs = values as IEnumerable<Complex>;
            var logicals = values as IEnumerable<bool>;
            var nums = values as IEnumerable<double>;
            var raws = values as IEnumerable<byte>;
            var sexpVec = values as SymbolicExpression;

            if (sexpVec != null && sexpVec.IsVector())
                return sexpVec;
            if (ints != null)
                return engine.CreateIntegerVector(ints);
            if (chars != null)
                return engine.CreateCharacterVector(chars);
            if (cplxs != null)
                return engine.CreateComplexVector(cplxs);
            if (logicals != null)
                return engine.CreateLogicalVector(logicals);
            if (nums != null)
                return engine.CreateNumericVector(nums);
            if (raws != null)
                return engine.CreateRawVector(raws);
            throw new NotSupportedException(string.Format("Cannot convert type {0} to an R vector", values.GetType()));
        }

        /// <summary>
        /// Creates a new environment.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="parent">The parent environment.</param>
        /// <returns>The newly created environment.</returns>
        public static REnvironment CreateEnvironment(this REngine engine, REnvironment parent)
        {
            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new REnvironment(engine, parent);
        }

        /// <summary>
        /// Creates a new isolated environment.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <returns>The newly created isolated environment.</returns>
        public static REnvironment CreateIsolatedEnvironment(this REngine engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            var pointer = engine.GetFunction<Rf_allocSExp>()(SymbolicExpressionType.Environment);
            return new REnvironment(engine, pointer);
        }
    }
}