using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using RDotNet.Client.RLanguageService;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
    public class RemoteRObjectAPI : IRObjectAPI
    {
        private readonly RLanguageServiceClient _client;

        public RemoteRObjectAPI(Binding binding, Uri rootUri, int id)
        {
            _client = new RLanguageServiceClient(binding, new EndpointAddress(rootUri + "RLanguageService/" + id));
        }

        public void PreserveObject(IRSafeHandle handle)
        {
            _client.PreserveObject(handle.Context);
        }

        public void ReleaseObject(IRSafeHandle handle)
        {
            try
            {
                _client.ReleaseObject(handle.Context);
            }
            catch (NullReferenceException)
            { }
            catch (CommunicationException)
            { }
        }

        public IRSafeHandle Protect(IRSafeHandle handle)
        {
            //TODO: Return the same or a new one?
            var result = _client.Protect(handle.Context);
            return new RSafeHandle(result, this);
        }

        public void Unprotect(int count)
        {
            _client.Unprotect(count);
        }

        public void UnprotectPtr(IRSafeHandle handle)
        {
            _client.UnprotectPtr(handle.Context);
        }

        public IRSafeHandle Install(string s)
        {
            var installed = _client.Install(s);
            return new RSafeHandle(installed, this);
        }

        public IRSafeHandle MakeString(string s)
        {
            var result = _client.MakeString(s);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle AsCharacterFactor(IRSafeHandle handle)
        {
            var result = _client.AsCharacterFactor(handle.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle AllocateVector(SymbolicExpressionType type, int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException("length");

            var result = _client.AllocateVector(type, length);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle CoerceVector(IRSafeHandle handle, SymbolicExpressionType type)
        {
            var result = _client.CoerceVector(handle.Context, type);
            return new RSafeHandle(result, this);
        }

        public bool IsVector(IRSafeHandle handle)
        {
            var result = _client.IsVector(handle.Context);
            return result;
        }

        public bool IsFrame(IRSafeHandle handle)
        {
            var result = _client.IsFrame(handle.Context);
            return result;
        }

        public bool IsS4(IRSafeHandle handle)
        {
            var result = _client.IsS4(handle.Context);
            return result;
        }

        public int GetLength(IRSafeHandle handle)
        {
            var result = _client.GetLength(handle.Context);
            return result;
        }

        public IRSafeHandle AllocateMatrix(SymbolicExpressionType type, int rowCount, int columnCount)
        {
            var result = _client.AllocateMatrix(type, rowCount, columnCount);
            return new RSafeHandle(result, this);
        }

        public bool IsMatrix(IRSafeHandle handle)
        {
            var result = _client.IsMatrix(handle.Context);
            return result;
        }

        public int NumberOfRows(IRSafeHandle handle)
        {
            var result = _client.NumberOfRows(handle.Context);
            return result;
        }

        public int NumberOfColumns(IRSafeHandle handle)
        {
            var result = _client.NumberOfColumns(handle.Context);
            return result;
        }

        public IRSafeHandle AllocateList(int length)
        {
            var result = _client.AllocateList(length);
            return new RSafeHandle(result, this);
        }

        public bool IsList(IRSafeHandle handle)
        {
            var result = _client.IsList(handle.Context);
            return result;
        }

        public IRSafeHandle Evaluate(IRSafeHandle handle, IRSafeHandle environment)
        {
            var result = _client.Evaluate(handle.Context, environment.Context);
            return new RSafeHandle(result, this);
        }
            
        public IRSafeHandle TryEvaluate(IRSafeHandle handle,
                                        IRSafeHandle environment,
                                        out bool errorOccurred)
        {
            var result = _client.TryEvaluate(handle.Context, environment.Context, out errorOccurred);
            return errorOccurred ? null : new RSafeHandle(result, this);
        }

        public IRSafeHandle ParseVector(IRSafeHandle handle, int statementCount, out ParseStatus status)
        {
            var result = _client.ParseVector(handle.Context, statementCount, out status);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle FindVar(IRSafeHandle name, IRSafeHandle environment)
        {
            var result = _client.FindVar(name.Context, environment.Context);

            var handle = new RSafeHandle(result, this);
            if (IsUnbound(handle)) throw new EvaluationException(string.Format("Error: object '{0}' not found", name));

            return handle;
        }

        public bool IsUnbound(IRSafeHandle handle)
        {
            var result = _client.GetUnboundValue();
            return result.Handle == handle.Context.Handle;
        }

        public void SetVar(IRSafeHandle name, IRSafeHandle value, IRSafeHandle environment)
        {
            _client.SetVar(name.Context, value.Context, environment.Context);
        }

        public void DefineVar(IRSafeHandle name, IRSafeHandle value, IRSafeHandle environment)
        {
            _client.DefineVar(name.Context, value.Context, environment.Context);
        }

        public IRSafeHandle GetAttribute(IRSafeHandle handle, IRSafeHandle name)
        {
            var result = _client.GetAttrib(name.Context, name.Context);
            var newHandle = new RSafeHandle(result, this);
            return !IsNil(newHandle) ? newHandle : null;
        }

        public IRSafeHandle SetAttribute(IRSafeHandle handle, IRSafeHandle name, IRSafeHandle value)
        {
            var result = _client.SetAttrib(name.Context, name.Context, value.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle DoSlot(IRSafeHandle handle, IRSafeHandle name)
        {
            var result = _client.DoSlot(handle.Context, name.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle DoSlotAssign(IRSafeHandle handle, IRSafeHandle name, IRSafeHandle value)
        {
            var result = _client.DoSlotAssign(handle.Context, name.Context, value.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle GetClassDefinition(string what)
        {
            var result = _client.GetClassDefinition(what);
            return new RSafeHandle(result, this);
        }

        public bool HasSlot(IRSafeHandle handle, IRSafeHandle name)
        {
            var result = _client.HasSlot(handle.Context, name.Context);
            return result;
        }

        public bool IsEnvironment(IRSafeHandle handle)
        {
            var result = _client.IsEnvironment(handle.Context);
            return result;
        }

        public bool IsExpression(IRSafeHandle handle)
        {
            var result = _client.IsExpression(handle.Context);
            return result;
        }

        public bool IsSymbol(IRSafeHandle handle)
        {
            var result = _client.IsSymbol(handle.Context);
            return result;
        }

        public bool IsLanguage(IRSafeHandle handle)
        {
            var result = _client.IsLanguage(handle.Context);
            return result;
        }

        public bool IsFunction(IRSafeHandle handle)
        {
            var result = _client.IsFunction(handle.Context);
            return result;
        }

        public bool IsFactor(IRSafeHandle handle)
        {
            var result = _client.IsFactor(handle.Context);
            return result;
        }

        public bool IsOrdered(IRSafeHandle handle)
        {
            var result = _client.IsOrdered(handle.Context);
            return result;
        }

        public IRSafeHandle LsInternal(IRSafeHandle environment, bool all)
        {
            var result = _client.LsInternal(environment.Context, all);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle ApplyClosure(IRSafeHandle call,
                                        IRSafeHandle value,
                                        IRSafeHandle arguments,
                                        IRSafeHandle environment,
                                        IRSafeHandle suppliedEnvironment)
        {
            var result = _client.ApplyClosure(environment.Context,
                                            value.Context,
                                            arguments.Context,
                                            environment.Context,
                                            suppliedEnvironment.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle VectorToPairList(IRSafeHandle handle)
        {
            var result = _client.VectorToPairList(handle.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle AllocateSEXP(SymbolicExpressionType type)
        {
            var result = _client.AllocateSEXP(type);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle CreateIsolatedEnvironment()
        {
            var result = AllocateSEXP(SymbolicExpressionType.Environment);
            return result;
        }

        public IRSafeHandle NewEnvironment(IRSafeHandle parent)
        {
            var nil = GetNilValue();
            var result = _client.NewEnvironment(nil.Context, nil.Context, parent.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle Cons(IRSafeHandle handle, IRSafeHandle next)
        {
            var result = _client.Cons(handle.Context, next.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle LCons(IRSafeHandle handle, IRSafeHandle next)
        {
            var result = _client.LCons(handle.Context, next.Context);
            return new RSafeHandle(result, this);
        }

        public bool IsNil(IRSafeHandle handle)
        {
            var result = _client.IsNil(handle.Context);
            return result;
        }

        public bool IsNaString(IRSafeHandle handle)
        {
            var result = _client.IsNaString(handle.Context);
            return result;
        }

        public string GetParseError()
        {
            var result = _client.GetParseError();
            return result;
        }

        public IRSafeHandle GetGlobalEnvironment()
        {
            var result = _client.GetGlobalEnvironment();
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle GetNames()
        {
            var names = _client.GetNames();
            return new RSafeHandle(names, this);
        }

        private IRSafeHandle GetNilValue()
        {
            var result = _client.GetNilValue();
            return new RSafeHandle(result, this);
        }

        public void InitializeVector<T>(IRSafeHandle handle, SymbolicExpressionType type, IList<T> vector)
        {
            object o = vector;
            switch (type)
            {
                case SymbolicExpressionType.NumericVector:
                    _client.InitializeNumericVector(handle.Context, (List<double>)o);
                    break;
                case SymbolicExpressionType.IntegerVector:
                    _client.InitializeIntegerVector(handle.Context, (List<int>)o);
                    break;
                case SymbolicExpressionType.CharacterVector:
                    _client.InitializeCharacterVector(handle.Context, (List<string>)o);
                    break;
                case SymbolicExpressionType.LogicalVector:
                    _client.InitializeLogicalVector(handle.Context, (List<bool>)o);
                    break;
                case SymbolicExpressionType.RawVector:
                    _client.InitializeRawVector(handle.Context, (List<byte>)o);
                    break;
                case SymbolicExpressionType.ComplexVector:
                    _client.InitializeComplexVector(handle.Context, (List<Complex>)o);
                    break;
            }
        }

        public IList<string> GetAttributeNames(IRSafeHandle handle)
        {
            var result =  _client.GetAttributeNames(handle.Context);
            return result;
        }

        public string GetPrintName(IRSafeHandle handle)
        {
            var result = _client.GetPrintName(handle.Context);
            return result;
        }

        public void SetPrintName(IRSafeHandle handle, string value)
        {
            _client.SetPrintName(handle.Context, value);
        }

        public IRSafeHandle GetDimNames()
        {
            var result = _client.GetDimNames();
            return new RSafeHandle(result, this);
        }

        public IEnumerable<IRSafeHandle> GetSymbols(IRSafeHandle handle)
        {
            var result = _client.GetSymbols(handle.Context);
            return result.Select(r => new RSafeHandle(r, this));
        }

        public bool IsPromise(IRSafeHandle handle)
        {
            var result = _client.IsPromise(handle.Context);
            return result;
        }

        public IRSafeHandle GetFactorLevels(IRSafeHandle handle)
        {
            var result = _client.GetLevels(handle.Context);
            return new RSafeHandle(result, this);
        }

        public IList<T> GetVectorValues<T>(IRSafeHandle handle, SymbolicExpressionType type)
        {
            IList<T> result = new List<T>();

            switch (type)
            {
                case SymbolicExpressionType.NumericVector:
                    result = (IList<T>)_client.GetNumericVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.IntegerVector:
                    result = (IList<T>)_client.GetIntegerVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.CharacterVector:
                    result = (IList<T>)_client.GetCharacterVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.LogicalVector:
                    result = (IList<T>)_client.GetLogicalVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.RawVector:
                    result = (IList<T>)_client.GetRawVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.ComplexVector:
                    result = (IList<T>)_client.GetComplexVectorValues(handle.Context);
                    break;
            }

            return result;
        }

        public void ClearVector(IRSafeHandle handle, SymbolicExpressionType type, int length)
        {
            switch (type)
            {
                case SymbolicExpressionType.NumericVector:
                    _client.GetNumericVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.IntegerVector:
                    _client.GetIntegerVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.CharacterVector:
                    _client.GetCharacterVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.LogicalVector:
                    _client.GetLogicalVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.RawVector:
                    _client.GetRawVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.ComplexVector:
                    _client.GetComplexVectorValues(handle.Context);
                    break;
            }
        }

        public void InitializeMatrix<T>(IRSafeHandle handle, SymbolicExpressionType type, T[,] matrix)
        {
            switch (type)
            {
                case SymbolicExpressionType.NumericVector:
                    _client.GetNumericVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.IntegerVector:
                    _client.GetIntegerVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.CharacterVector:
                    _client.GetCharacterVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.LogicalVector:
                    _client.GetLogicalVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.RawVector:
                    _client.GetRawVectorValues(handle.Context);
                    break;
                case SymbolicExpressionType.ComplexVector:
                    _client.GetComplexVectorValues(handle.Context);
                    break;
            }
        }

        public T GetVectorValueAt<T>(IRSafeHandle handle, SymbolicExpressionType type, int index)
        {
            object result = null;

            switch (type)
            {
                case SymbolicExpressionType.NumericVector:
                    result = _client.GetNumericVectorValueAt(handle.Context, index);
                    break;
                case SymbolicExpressionType.IntegerVector:
                    result = _client.GetIntegerVectorValueAt(handle.Context, index);
                    break;
                case SymbolicExpressionType.CharacterVector:
                    result = _client.GetCharacterVectorValueAt(handle.Context, index);
                    break;
                case SymbolicExpressionType.LogicalVector:
                    result = _client.GetLogicalVectorValueAt(handle.Context, index);
                    break;
                case SymbolicExpressionType.RawVector:
                    result = _client.GetRawVectorValueAt(handle.Context, index);
                    break;
                case SymbolicExpressionType.ComplexVector:
                    result = _client.GetComplexVectorValueAt(handle.Context, index);
                    break;
                case SymbolicExpressionType.ExpressionVector:
                    var context = _client.GetExpressionVectorValueAt(handle.Context, index);
                    result = new RSafeHandle(context, this);
                    break;
            }

            return (T) result;
        }

        public void SetVectorValueAt<T>(IRSafeHandle handle, SymbolicExpressionType type, int index, T value)
        {
            object o = value;
            switch (type)
            {
                case SymbolicExpressionType.NumericVector:
                    _client.SetNumericVectorValueAt(handle.Context, index, (double) o);
                    break;
                case SymbolicExpressionType.IntegerVector:
                    _client.SetIntegerVectorValueAt(handle.Context, index, (int) o);
                    break;
                case SymbolicExpressionType.CharacterVector:
                    _client.SetCharacterVectorValueAt(handle.Context, index, (string) o);
                    break;
                case SymbolicExpressionType.LogicalVector:
                    _client.SetLogicalVectorValueAt(handle.Context, index, (bool) o);
                    break;
                case SymbolicExpressionType.RawVector:
                    _client.SetRawVectorValueAt(handle.Context, index, (byte) o);
                    break;
                case SymbolicExpressionType.ComplexVector:
                    _client.SetComplexVectorValueAt(handle.Context, index, (Complex) o);
                    break;
            }
        }

        public IRSafeHandle GetParentEnvironment(IRSafeHandle handle)
        {
            var result = _client.GetParentEnvironment(handle.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle GetClosureBody(IRSafeHandle handle)
        {
            var result = _client.GetClosureBody(handle.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle GetClosureFormals(IRSafeHandle handle)
        {
            var result = _client.GetClosureFormals(handle.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle GetClosureEnvironment(IRSafeHandle handle)
        {
            var result = _client.GetClosureEnvironment(handle.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle GetLanguageList(IRSafeHandle handle)
        {
            var result = _client.GetLanguageList(handle.Context);
            return new RSafeHandle(result, this);
        }

        public T GetMatrixValueAt<T>(IRSafeHandle handle, SymbolicExpressionType type, int row, int column)
        {
            object result = null;

            switch (type)
            {
                case SymbolicExpressionType.NumericVector:
                    result = _client.GetNumericMatrixValueAt(handle.Context, row, column);
                    break;
                case SymbolicExpressionType.IntegerVector:
                    result = _client.GetIntegerMatrixValueAt(handle.Context, row, column);
                    break;
                case SymbolicExpressionType.CharacterVector:
                    result = _client.GetCharacterMatrixValueAt(handle.Context, row, column);
                    break;
                case SymbolicExpressionType.LogicalVector:
                    result = _client.GetLogicalMatrixValueAt(handle.Context, row, column);
                    break;
                case SymbolicExpressionType.RawVector:
                    result = _client.GetRawMatrixValueAt(handle.Context, row, column);
                    break;
                case SymbolicExpressionType.ComplexVector:
                    result = _client.GetComplexMatrixValueAt(handle.Context, row, column);
                    break;
            }

            return (T)result;
        }

        public void SetMatrixValueAt<T>(IRSafeHandle handle, SymbolicExpressionType type, int row, int column, T value)
        {
            object o = value;

            switch (type)
            {
                case SymbolicExpressionType.NumericVector:
                    _client.SetNumericMatrixValueAt(handle.Context, row, column, (double)o);
                    break;
                case SymbolicExpressionType.IntegerVector:
                    _client.SetIntegerMatrixValueAt(handle.Context, row, column, (int)o);
                    break;
                case SymbolicExpressionType.CharacterVector:
                    _client.SetCharacterMatrixValueAt(handle.Context, row, column, (string)o);
                    break;
                case SymbolicExpressionType.LogicalVector:
                    _client.SetLogicalMatrixValueAt(handle.Context, row, column, (bool)o);
                    break;
                case SymbolicExpressionType.RawVector:
                    _client.SetRawMatrixValueAt(handle.Context, row, column, (byte)o);
                    break;
                case SymbolicExpressionType.ComplexVector:
                    _client.SetComplexMatrixValueAt(handle.Context, row, column, (Complex)o);
                    break;
            }
        }

        public IRSafeHandle GetValue(IRSafeHandle handle)
        {
            var result = _client.GetValue(handle.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle GetInternal(IRSafeHandle handle)
        {
            var result = _client.GetInternal(handle.Context);
            return new RSafeHandle(result, this);
        }

        public IRSafeHandle GetClass(IRSafeHandle handle)
        {
            var result = _client.GetClass(handle.Context);
            return new RSafeHandle(result, this);
        }

        public T[,] GetMatrixValues<T>(IRSafeHandle handle, SymbolicExpressionType type)
        {
            object result = null;

            switch (type)
            {
                case SymbolicExpressionType.NumericVector:
                    result = _client.GetNumericMatrixValues(handle.Context);
                    break;
                case SymbolicExpressionType.IntegerVector:
                    result = _client.GetIntegerMatrixValues(handle.Context);
                    break;
                case SymbolicExpressionType.CharacterVector:
                    result = _client.GetCharacterMatrixValues(handle.Context);
                    break;
                case SymbolicExpressionType.LogicalVector:
                    result = _client.GetLogicalMatrixValues(handle.Context);
                    break;
                case SymbolicExpressionType.RawVector:
                    result = _client.GetRawMatrixValues(handle.Context);
                    break;
                case SymbolicExpressionType.ComplexVector:
                    result = _client.GetComplexMatrixValues(handle.Context);
                    break;
            }

            return (T[,])result;
        }
    }
}
