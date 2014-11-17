using System;
using System.Collections.Generic;
using System.Linq;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
    public class SymbolicExpression : IDisposable
    {
        private readonly IRObjectAPI _api;

        public SymbolicExpression(IRSafeHandle handle)
        {
            if (handle == null || handle.IsInvalid) throw new ArgumentNullException("handle");

            Handle = handle;
            _api = handle.API;
            Type = Handle.Context.Type;
        }

        public IRSafeHandle Handle { get; private set; }
        public SymbolicExpressionType Type { get; private set; }

        public bool HasOrderedFactors()
        {
            return _api.IsOrdered(Handle);
        }

        public bool IsDataFrame()
        {
            return _api.IsFrame(Handle);
        }

        public bool IsS4()
        {
            return _api.IsS4(Handle);
        }

        public bool IsVector()
        {
            return _api.IsVector(Handle);
        }

        public bool IsMatrix()
        {
            return _api.IsMatrix(Handle);
        }

        public bool IsEnvironment()
        {
            return _api.IsEnvironment(Handle);
        }

        public bool IsExpression()
        {
            return _api.IsExpression(Handle);
        }

        public bool IsSymbol()
        {
            return _api.IsSymbol(Handle);
        }

        public bool IsLanguage()
        {
            return _api.IsLanguage(Handle);
        }

        public bool IsFunction()
        {
            return _api.IsFunction(Handle);
        }

        public bool IsFactor()
        {
            return _api.IsFactor(Handle);
        }

        public bool IsPromise()
        {
            return _api.IsPromise(Handle);
        }

        public bool IsList()
        {
            return _api.IsList(Handle);
        }

        public Symbol ToSymbol()
        {
            return new Symbol(Handle);
        }

        public S4Object ToS4Object()
        {
            return new S4Object(Handle);
        }

        public CharacterVector ToCharacterVector()
        {
            var result = this as CharacterVector;
            if (result != null) return result;

            result = new CharacterVector(Handle);
            return result;
        }

        public PairList ToPairList()
        {
            //Vector ?
            //var result = _api.VectorToPairList(Handle);
            return new PairList(Handle);
        }

        public GenericVector ToGenericVector()
        {
            var result = this as GenericVector;
            if (result != null) return result;

            const string statement = "as.list";
            var s = _api.MakeString(statement);
            ParseStatus status;
            var parsed = _api.ParseVector(s, 1, out status);
            if (status != ParseStatus.Ok) throw new ParseException(status, statement, String.Empty);

            var expression = new Expression(parsed);
            var evaluated = expression.Evaluate(GetGlobalEnvironment());

            result = new GenericVector(new[] { evaluated }, _api);
            return result;
        }

        public IntegerVector ToIntegerVector()
        {
            var result = this as IntegerVector;
            if (result != null) return result;

            result = new IntegerVector(Handle);
            return result;
        }

        public Expression ToExpression()
        {
            var result = this as Expression;
            if (result != null) return result;

            result = new Expression(Handle);
            return result;
        }

        public NumericVector ToNumericVector()
        {
            var result = this as NumericVector;
            if (result != null) return result;

            result = new NumericVector(Handle);
            return result;
        }

        public ComplexVector ToComplexVector()
        {
            var result = this as ComplexVector;
            if (result != null) return result;

            result = new ComplexVector(Handle);
            return result;
        }

        public LogicalVector ToLogicalVector()
        {
            var result = this as LogicalVector;
            if (result != null) return result;

            result = new LogicalVector(Handle);
            return result;
        }

        public Function ToFunction()
        {
            switch (Type)
            {
                case SymbolicExpressionType.Closure:
                    return new Closure(Handle);

                case SymbolicExpressionType.SpecialFunction:
                    return new SpecialFunction(Handle);

                case SymbolicExpressionType.BuiltinFunction:
                    return new BuiltinFunction(Handle);
            }

            return null;
        }

        public DataFrame ToDataFrame()
        {
            var result = this as DataFrame;
            if (result != null) return result;

            result = new DataFrame(Handle);
            return result;
        }

        protected REnvironment SetAsNewEnvironment(SymbolicExpression parent)
        {
            var result = _api.NewEnvironment(parent.Handle);
            Handle = result;
            return new REnvironment(Handle);
        }

        public REnvironment GetParentEnvironment()
        {
            var result = _api.GetParentEnvironment(Handle);
            return new REnvironment(result);
        }

        public REnvironment GetGlobalEnvironment()
        {
            var result = _api.GetGlobalEnvironment();
            return new REnvironment(result);
        }

        protected bool EnvironmentTryEvaluate(Expression expression, out SymbolicExpression evaluated)
        {
            bool errorOccurred;
            var evaluatedHandle = _api.TryEvaluate(expression.Handle, Handle, out errorOccurred);
            evaluated = errorOccurred ? null : new SymbolicExpression(evaluatedHandle);

            return !errorOccurred;
        }

        protected SymbolicExpression EnvironmentEvaluate(Expression expression)
        {
            var result = _api.Evaluate(expression.Handle, Handle);
            return new SymbolicExpression(result);
        }

        protected void ClearVector(SymbolicExpressionType type, int length)
        {
            _api.ClearVector(Handle, type, length);
        }

        protected void InitializeVector<T>(SymbolicExpressionType type, IList<T> vector)
        {
            //IS vector?
            _api.InitializeVector(Handle, type, vector);
        }

        protected void CoerceVector(SymbolicExpressionType type)
        {
            //Is vector?
            if (type == SymbolicExpressionType.CharacterVector && IsFactor())
            {
                Handle = _api.AsCharacterFactor(Handle);
            }

            Handle = _api.CoerceVector(Handle, type);
        }

        protected T GetVectorValueAt<T>(SymbolicExpressionType type, int index)
        {
            object result;
            if (type == SymbolicExpressionType.ExpressionVector)
            {
                var handle = _api.GetVectorValueAt<RSafeHandle>(Handle, type, index);
                result = new Expression(handle);
            }
            else
            {
                result = _api.GetVectorValueAt<T>(Handle, type, index);
            }

            return (T)result;
        }

        protected void SetVectorValueAt<T>(SymbolicExpressionType type, int index, T value)
        {
            _api.SetVectorValueAt(Handle, type, index, value);
        }

        protected IList<T> GetVectorValues<T>(SymbolicExpressionType type)
        {
            var result = _api.GetVectorValues<T>(Handle, type);
            return result;
        }

        protected void InitializeMatrix<T>(SymbolicExpressionType type, T[,] matrix)
        {
            _api.InitializeMatrix(Handle, type, matrix);
        }

        protected T GetMatrixValueAt<T>(SymbolicExpressionType type, int row, int column)
        {
            var result = _api.GetMatrixValueAt<T>(Handle, type, row, column);
            return result;
        }

        protected void SetMatrixValueAt<T>(SymbolicExpressionType type, int row, int column, T value)
        {
            _api.SetMatrixValueAt(Handle, type, row, column, value);
        }

        protected int GetNumberOfRows()
        {
            //matrix?
            return _api.NumberOfRows(Handle);
        }

        protected int GetNumberOfColumns()
        {
            return _api.NumberOfColumns(Handle);
        }

        protected int GetLength()
        {
            return _api.GetLength(Handle);
        }

        protected IList<string> GetAttributeNames()
        {
            return _api.GetAttributeNames(Handle);
        }

        public SymbolicExpression GetAttribute(Symbol symbol)
        {
            if (symbol == null) throw new ArgumentException();

            var attribute = _api.GetAttribute(Handle, symbol.Handle);
            return new SymbolicExpression(attribute);
        }

        public void SetAttribute(Symbol symbol, SymbolicExpression value)
        {
            _api.SetAttribute(Handle, symbol.Handle, value.Handle);
        }

        protected SymbolicExpression GetAttribute(string attributeName)
        {
            if (string.IsNullOrEmpty(attributeName)) throw new ArgumentException();

            var installedName = _api.Install(attributeName);
            var attribute = _api.GetAttribute(Handle, installedName);

            return new SymbolicExpression(attribute);
        }

        protected void SetAttribute(string attributeName, SymbolicExpression value)
        {
            if (string.IsNullOrEmpty(attributeName)) throw new ArgumentNullException();

            var installedName = _api.Install(attributeName);
            _api.SetAttribute(Handle, installedName, value.Handle);
        }

        protected IEnumerable<Symbol> GetSymbols()
        {
            var symbols = _api.GetSymbols(Handle);
            return symbols.Select(s => new Symbol(s));
        }

        protected Symbol GetNamesSymbol()
        {
            var names = _api.GetNames();
            return new Symbol(names);
        }

        protected string GetPrintName()
        {
            var result = _api.GetPrintName(Handle);
            return result;
        }

        protected void SetPrintName(string value)
        {
            _api.SetPrintName(Handle, value);
        }

        protected SymbolicExpression GetValue()
        {
            var result = _api.GetValue(Handle);
            return new SymbolicExpression(result);
        }

        protected SymbolicExpression GetInternal()
        {
            var result =  _api.GetInternal(Handle);
            return new SymbolicExpression(result);
        }

        protected Expression Cons(Expression expression)
        {
            var result = _api.Cons(Handle, expression.Handle);
            return new Expression(result);
        }

        protected Expression LCons(Expression last)
        {
            var result = _api.LCons(Handle, last.Handle);
            return new Expression(result);
        }

        protected Expression LCons(PairList list)
        {
            var result = _api.LCons(Handle, list.Handle);
            return new Expression(result);
        }
        
        protected Symbol GetDimNames()
        {
            var result = _api.GetDimNames();
            return new Symbol(result);
        }

        protected void DoSlotAssign(string name, SymbolicExpression value)
        {
            var s = _api.MakeString(name);
            _api.DoSlotAssign(Handle, s, value.Handle);
        }

        protected SymbolicExpression DoSlot(string name)
        {
            var s = _api.MakeString(name);
            var result = _api.DoSlot(Handle, s);
            return new SymbolicExpression(result);
        }

        public bool HasSlot(string name)
        {
            var s = _api.MakeString(name);
            return _api.HasSlot(Handle, s);
        }

        protected Symbol GetClass()
        {
            var result = _api.GetClass(Handle);
            return new Symbol(result);
        }

        protected SymbolicExpression GetClassDefinition(string className)
        {
            var result = _api.GetClassDefinition(className);
            return new SymbolicExpression(result);
        }

        protected SymbolicExpression FindVar(SymbolicExpression variable)
        {
            if (Type != SymbolicExpressionType.Environment) throw new Exception(); //Warning.

            var result = _api.FindVar(variable.Handle, Handle);
            return new SymbolicExpression(result);
        }

        protected void DefineVar(SymbolicExpression name, SymbolicExpression value)
        {
            _api.DefineVar(name.Handle, value.Handle, Handle);
        }

        protected SymbolicExpression Install(string name)
        {
            var result = _api.Install(name);
            return new SymbolicExpression(result);
        }

        protected IEnumerable<string> LsInternal(bool all)
        {
            //Environment?
            var result = _api.LsInternal(Handle, all);
            var vectorized = new CharacterVector(result);
            return vectorized.ToList();
        }

        protected internal IList<string> GetSlotNames()
        {
            var result = InternalEvaluate(".slotNames");
            return result.ToCharacterVector().ToList();
        }

        protected Symbol GetFactorLevels()
        {
            var result = _api.GetFactorLevels(Handle);
            return new Symbol(result);
        }

        protected PairList GetClosureArguments()
        {
            var result = _api.GetClosureFormals(Handle);
            return new PairList(result);
        }

        protected Language GetClosureBody()
        {
            var result = _api.GetClosureBody(Handle);
            return new Language(result);
        }

        protected REnvironment GetClosureEnvironment()
        {
            var result = _api.GetClosureEnvironment(Handle);
            return new REnvironment(result);
        }

        protected PairList GetLanguageList()
        {
            var result = _api.GetLanguageList(Handle);
            return new PairList(result);
        }

        protected T[,] GetMatrixValues<T>(SymbolicExpressionType type)
        {
            var result = _api.GetMatrixValues<T>(Handle, type);
            return result;
        }

        protected SymbolicExpression InternalEvaluate(string statement)
        {
            var s = _api.MakeString(statement);
            ParseStatus status;
            var parsed = _api.ParseVector(s, 1, out status);
            if (status != ParseStatus.Ok) throw new ParseException(status, statement, String.Empty);

            var vector = new ExpressionVector(parsed);
            if (vector.Length == 0) throw new ParseException(status, statement, "Failed to create expression vector!");

            var result = vector.EvaulateExpressionAt(0);
            return result;
        }

        protected bool InternalTryEvaluate(string statement, out SymbolicExpression expression)
        {
            var s = _api.MakeString(statement);
            ParseStatus status;
            var parsed = _api.ParseVector(s, 1, out status);
            if (status != ParseStatus.Ok) throw new ParseException(status, statement, String.Empty);

            var vector = new ExpressionVector(parsed);
            if (vector.Length == 0) throw new ParseException(status, statement, "Failed to create expression vector!");

            var result = vector.TryEvaulateExpressionAt(0, out expression);
            return result;
        }

        public void Dispose()
        {
            var disposable = Handle as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
