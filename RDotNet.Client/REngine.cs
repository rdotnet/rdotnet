using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
    public class REngine
    {
        private readonly IRManagementAPI _managementAPI;
        private readonly IRObjectAPI _objectAPI;
        private readonly IROutputAPI _outputAPI;

        public REngine(int id, IRManagementAPI managementAPI, IRObjectAPI objectAPI, IROutputAPI outputAPI)
        {
            if (managementAPI == null) throw new ArgumentNullException("managementAPI");
            if (objectAPI == null) throw new ArgumentNullException("objectAPI");
            if (outputAPI == null) throw new ArgumentNullException("outputAPI");

            _managementAPI = managementAPI;
            _objectAPI = objectAPI;
            _outputAPI = outputAPI;
            ID = id;
        }

        public int ID { get; private set; }
        public string EngineName { get { return "R.NET"; } }

        public SymbolicExpression Evaluate(string statement)
        {
            return Defer(statement).LastOrDefault();
        }

        public SymbolicExpression Evaluate(Stream stream)
        {
            return Defer(stream).LastOrDefault();
        }

        private IEnumerable<SymbolicExpression> Defer(string statement)
        {
            using (TextReader reader = new StringReader(statement))
            {
                var incompleteStatement = new StringBuilder();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    foreach (var segment in Segment(line))
                    {
                        var result = Parse(segment, incompleteStatement);
                        if (result != null)
                        {
                            yield return result;
                        }
                    }
                }
            }
        }

        public IEnumerable<SymbolicExpression> Defer(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException();
            if (!stream.CanRead) throw new ArgumentException();

            using (var reader = new StreamReader(stream))
            {
                var incompleteStatement = new StringBuilder();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    foreach (var segment in Segment(line))
                    {
                        var result = Parse(segment, incompleteStatement);
                        if (result != null)
                        {
                            yield return result;
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> Segment(string line)
        {
            var segments = line.Split(';');
            for (var index = 0; index < segments.Length; index++)
            {
                if (index == segments.Length - 1)
                {
                    if (segments[index] != string.Empty)
                    {
                        yield return segments[index] + "\n";
                    }
                }
                else
                {
                    yield return segments[index] + ";";
                }
            }
        }

        private SymbolicExpression Parse(string statement, StringBuilder incompleteStatement)
        {
            incompleteStatement.Append(statement);
            var s = _objectAPI.MakeString(incompleteStatement.ToString());
            using (ProtectedPointer.Protect(_objectAPI, s))
            {
                ParseStatus status;
                var handle = _objectAPI.ParseVector(s, -1, out status);
                string errorStatement;
                switch (status)
                {
                    case ParseStatus.Ok:
                        using (var vector = new ExpressionVector(handle))
                        using (ProtectedPointer.Protect(_objectAPI, vector))
                        {
                            incompleteStatement.Clear();
                            if (vector.Length == 0) return null;

                            var result = vector.EvaulateExpressionAt(0);
                            return result;
                        }
                    case ParseStatus.Incomplete:
                        return null;

                    case ParseStatus.Error:
                        var parseError = _objectAPI.GetParseError();
                        errorStatement = incompleteStatement.ToString();
                        incompleteStatement.Clear();
                        throw new ParseException(status, errorStatement, parseError);

                    default:
                        errorStatement = incompleteStatement.ToString();
                        incompleteStatement.Clear();
                        throw new ParseException(status, errorStatement, string.Empty);
                }
            }
        }

        public void ClearGlobalEnvironment(bool garbageCollectR = true, bool removeHiddenRVars = false, bool detachPackages = false, IEnumerable<string> toDetach = null)
        {
            if (detachPackages)
                DoDetachPackages(toDetach);
            var rmStatement = removeHiddenRVars ? "rm(list=ls(all.names=TRUE))" : "rm(list=ls())";
            Evaluate(rmStatement);
            if (garbageCollectR)
                _managementAPI.ForceGarbageCollection();
        }

        private void DoDetachPackages(IEnumerable<string> toDetach)
        {
            var packages = toDetach != null ? new List<string>(toDetach) : GetDetachablePackages();
            packages.ForEach(x => Evaluate(string.Format("detach{{'{0}'}}", x)));
        }

        private List<string> GetDetachablePackages()
        {
            var result = Evaluate("search()[2:(which(search()=='package:stats')-1)]");
            return new List<string>(result.ToCharacterVector());
        }

        public void ResetConsole()
        {
            _outputAPI.ResetConsole();
        }

        public void ResetPlots()
        {
            _outputAPI.ResetPlots();
        }

        public List<string> GetPlots()
        {
            var plots =  _outputAPI.GetPlots();
            return plots;
        }

        public List<string> GetConsole()
        {
            var console = _outputAPI.GetConsole();
            return console;
        }

        public REnvironment CreateEnvironment(REnvironment parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");

            var environment = new REnvironment(parent);
            return environment;
        }

        public REnvironment CreateIsolatedEnvironment()
        {
            var environment = _objectAPI.CreateIsolatedEnvironment();
            var result = new REnvironment(environment);
            return result;
        }

        public CharacterVector CreateCharacterVector(int length)
        {
            var vector =  new CharacterVector(length, _objectAPI);
            return vector;
        }

        public ComplexVector CreateComplexVector(int length)
        {
            var vector = new ComplexVector(length, _objectAPI);
            return vector;
        }

        public IntegerVector CreateIntegerVector(int length)
        {
            var vector = new IntegerVector(length, _objectAPI);
            return vector;
        }

        public LogicalVector CreateLogicalVector(int length)
        {
            var vector = new LogicalVector(length, _objectAPI);
            return vector;
        }

        public NumericVector CreateNumericVector(int length)
        {
            var vector = new NumericVector(length, _objectAPI);
            return vector;
        }

        public RawVector CreateRawVector(int length)
        {
            var vector = new RawVector(length, _objectAPI);
            return vector;
        }

        public CharacterVector CreateCharacterVector(IList<string> values)
        {
            var vector = new CharacterVector(values, _objectAPI);
            return vector;
        }

        public ComplexVector CreateComplexVector(IList<Complex> values)
        {
            var vector = new ComplexVector(values, _objectAPI);
            return vector;
        }

        public IntegerVector CreateIntegerVector(IList<int> values)
        {
            var vector = new IntegerVector(values, _objectAPI);
            return vector;
        }

        public LogicalVector CreateLogicalVector(IList<bool> values)
        {
            var vector = new LogicalVector(values, _objectAPI);
            return vector;
        }

        public NumericVector CreateNumericVector(IList<double> values)
        {
            var vector = new NumericVector(values, _objectAPI);
            return vector;
        }

        public RawVector CreateRawVector(IList<byte> values)
        {
            var vector = new RawVector(values, _objectAPI);
            return vector;
        }

        public CharacterMatrix CreateCharacterMatrix(int rowCount, int columnCount)
        {
            var matrix = new CharacterMatrix(rowCount, columnCount, _objectAPI);
            return matrix;
        }

        public ComplexMatrix CreateComplexMatrix(int rowCount, int columnCount)
        {
           var matrix = new ComplexMatrix(rowCount, columnCount, _objectAPI);
           return matrix;
        }

        public IntegerMatrix CreateIntegerMatrix(int rowCount, int columnCount)
        {
            var matrix = new IntegerMatrix(rowCount, columnCount, _objectAPI);
            return matrix;
        }

        public LogicalMatrix CreateLogicalMatrix(int rowCount, int columnCount)
        {
            var matrix = new LogicalMatrix(rowCount, columnCount, _objectAPI);
            return matrix;
        }

        public NumericMatrix CreateNumericMatrix(int rowCount, int columnCount)
        {
            var matrix = new NumericMatrix(rowCount, columnCount, _objectAPI);
            return matrix;
        }

        public RawMatrix CreateRawMatrix(int rowCount, int columnCount)
        {
            var matrix = new RawMatrix(rowCount, columnCount, _objectAPI);
            return matrix;
        }

        public CharacterMatrix CreateCharacterMatrix(string[,] values)
        {
            var matrix = new CharacterMatrix(values, _objectAPI);
            return matrix;
        }

        public ComplexMatrix CreateComplexMatrix(Complex[,] values)
        {
            var matrix = new ComplexMatrix(values, _objectAPI);
            return matrix;
        }

        public IntegerMatrix CreateIntegerMatrix(int[,] values)
        {
            var matrix = new IntegerMatrix(values, _objectAPI);
            return matrix;
        }

        public LogicalMatrix CreateLogicalMatrix(bool[,] values)
        {
            var matrix = new LogicalMatrix(values, _objectAPI);
            return matrix;
        }

        public NumericMatrix CreateNumericMatrix(double[,] values)
        {
            var matrix = new NumericMatrix(values, _objectAPI);
            return matrix;
        }

        public RawMatrix CreateRawMatrix(byte[,] values)
        {
            var matrix = new RawMatrix(values, _objectAPI);
            return matrix;
        }

        public DataFrame CreateDataFrame<T>(IList<IList<T>> columns,
                                        IList<string> columnNames = null,
                                        IList<string> rowNames = null,
                                        bool checkRows = false,
                                        bool checkNames = true,
                                        bool stringsAsFactors = true)
        {
            if (columnNames == null) columnNames = Enumerable.Repeat(string.Empty, columns.Count).ToList();

            var globalEnvironment = new REnvironment(_objectAPI.GetGlobalEnvironment());
            
            var function = globalEnvironment.GetSymbol("data.frame").ToFunction();
            function.AddArgumentRange(columnNames, columns.Select(CreateVector).ToList());
            function.AddArgument("row.names", CreateCharacterVector(rowNames));
            function.AddArgument("check.rows", CreateLogicalVector(new[] { checkRows }));
            function.AddArgument("check.names", CreateLogicalVector(new[] { checkNames }));
            function.AddArgument("stringsAsFactors", CreateLogicalVector(new[] { stringsAsFactors }));

            var result = function.Invoke().ToDataFrame();
            return result;
        }

        private SymbolicExpression CreateVector<T>(IList<T> values)
        {
            if (values == null) return null;

            var typeMap = new Dictionary<Type, Func<IList<T>, SymbolicExpression>>
            {
                {typeof (IList<int>), v => CreateIntegerVector((IList<int>)v)},
                {typeof (IList<string>), v => CreateCharacterVector((IList<string>)v)},
                {typeof (IList<Complex>), v => CreateComplexVector((IList<Complex>)v)},
                {typeof (IList<bool>), v => CreateLogicalVector((IList<bool>)v)},
                {typeof (IList<double>), v => CreateNumericVector((IList<double>)v)},
                {typeof (IList<byte>), v => CreateRawVector((IList<byte>)v)}
            };
            Func<IList<T>, SymbolicExpression> action;
            var found = typeMap.TryGetValue(values.GetType(), out action);
            if (!found) throw new InvalidOperationException(string.Format("REngine::CreateVector<T> - Type {0} not found.", values.GetType()));

            var result = action(values);
            return result;
        }
    }
}
