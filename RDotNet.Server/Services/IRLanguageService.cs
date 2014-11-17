using System.Collections.Generic;
using System.Numerics;
using System.ServiceModel;
using RDotNet.R.Adapter;

namespace RDotNet.Server.Services
{
    [ServiceContract]
    public interface IRLanguageService
    {
        [OperationContract]
        void PreserveObject(SymbolicExpressionContext sexp);

        [OperationContract]
        void ReleaseObject(SymbolicExpressionContext sexp);

        [OperationContract]
        SymbolicExpressionContext Protect(SymbolicExpressionContext sexp);

        [OperationContract]
        void Unprotect(int count);

        [OperationContract]
        void UnprotectPtr(SymbolicExpressionContext sexp);

        [OperationContract]
        SymbolicExpressionContext Install(string s);

        [OperationContract]
        SymbolicExpressionContext MakeString(string s);

        [OperationContract]
        SymbolicExpressionContext MakeChar(string s);

        [OperationContract]
        SymbolicExpressionContext AsCharacterFactor(SymbolicExpressionContext sexp);

        [OperationContract]
        SymbolicExpressionContext AllocateVector(SymbolicExpressionType type, int length);

        [OperationContract]
        SymbolicExpressionContext CoerceVector(SymbolicExpressionContext sexp, SymbolicExpressionType type);

        [OperationContract]
        bool IsVector(SymbolicExpressionContext sexp);

        [OperationContract]
        bool IsFrame(SymbolicExpressionContext sexp);

        [OperationContract]
        bool IsS4(SymbolicExpressionContext sexp);

        [OperationContract]
        int GetLength(SymbolicExpressionContext sexp);

        [OperationContract]
        SymbolicExpressionContext AllocateMatrix(SymbolicExpressionType type, int rowCount, int columnCount);

        [OperationContract]
        bool IsMatrix(SymbolicExpressionContext sexp);

        [OperationContract]
        int NumberOfRows(SymbolicExpressionContext sexp);

        [OperationContract]
        int NumberOfColumns(SymbolicExpressionContext sexp);

        [OperationContract]
        SymbolicExpressionContext AllocateList(int length);

        [OperationContract]
        bool IsList(SymbolicExpressionContext sexp);

        [OperationContract]
        SymbolicExpressionContext Evaluate(SymbolicExpressionContext statement, SymbolicExpressionContext environment);

        [OperationContract]
        SymbolicExpressionContext TryEvaluate(SymbolicExpressionContext statement, SymbolicExpressionContext environment, out bool errorOccurred);

        [OperationContract]
        SymbolicExpressionContext ParseVector(SymbolicExpressionContext statement, int statementCount, out ParseStatus status);

        [OperationContract]
        SymbolicExpressionContext FindVar(SymbolicExpressionContext name, SymbolicExpressionContext environment);

        [OperationContract]
        void SetVar(SymbolicExpressionContext name, SymbolicExpressionContext value, SymbolicExpressionContext environment);

        [OperationContract]
        void DefineVar(SymbolicExpressionContext name, SymbolicExpressionContext value, SymbolicExpressionContext environment);

        [OperationContract]
        SymbolicExpressionContext GetAttrib(SymbolicExpressionContext sexp, SymbolicExpressionContext name);

        [OperationContract]
        SymbolicExpressionContext SetAttrib(SymbolicExpressionContext sexp, SymbolicExpressionContext name, SymbolicExpressionContext value);

        [OperationContract]
        SymbolicExpressionContext DoSlot(SymbolicExpressionContext sexp, SymbolicExpressionContext name);

        [OperationContract]
        SymbolicExpressionContext DoSlotAssign(SymbolicExpressionContext sexp, SymbolicExpressionContext name, SymbolicExpressionContext value);

        [OperationContract]
        SymbolicExpressionContext GetClassDefinition(string what);

        [OperationContract]
        bool HasSlot(SymbolicExpressionContext context, SymbolicExpressionContext name);

        [OperationContract]
        bool IsEnvironment(SymbolicExpressionContext context);

        [OperationContract]
        bool IsExpression(SymbolicExpressionContext context);

        [OperationContract]
        bool IsSymbol(SymbolicExpressionContext context);

        [OperationContract]
        bool IsLanguage(SymbolicExpressionContext context);

        [OperationContract]
        bool IsFunction(SymbolicExpressionContext context);

        [OperationContract]
        bool IsFactor(SymbolicExpressionContext context);

        [OperationContract]
        bool IsOrdered(SymbolicExpressionContext context);

        [OperationContract]
        SymbolicExpressionContext LsInternal(SymbolicExpressionContext context, bool all);

        [OperationContract]
        SymbolicExpressionContext ApplyClosure(SymbolicExpressionContext call,
                                                SymbolicExpressionContext value,
                                                SymbolicExpressionContext arguments,
                                                SymbolicExpressionContext environment,
                                                SymbolicExpressionContext suppliedEnvironment);

        [OperationContract]
        SymbolicExpressionContext VectorToPairList(SymbolicExpressionContext context);

        [OperationContract]
        SymbolicExpressionContext AllocateSEXP(SymbolicExpressionType type);

        [OperationContract]
        SymbolicExpressionContext NewEnvironment(SymbolicExpressionContext names, SymbolicExpressionContext values, SymbolicExpressionContext parent);

        [OperationContract]
        SymbolicExpressionContext Cons(SymbolicExpressionContext context, SymbolicExpressionContext next);

        [OperationContract]
        SymbolicExpressionContext LCons(SymbolicExpressionContext context, SymbolicExpressionContext next);

        [OperationContract]
        bool IsNil(SymbolicExpressionContext attribute);

        [OperationContract]
        bool IsNaString(SymbolicExpressionContext rstring);

        [OperationContract]
        string GetParseError();

        [OperationContract]
        SymbolicExpressionContext GetGlobalEnvironment();

        [OperationContract]
        SymbolicExpressionContext GetNames();

        [OperationContract]
        List<string> GetCharacterVectorValues(SymbolicExpressionContext context);

        [OperationContract]
        List<SymbolicExpressionContext> GetExpressionVectorValues(SymbolicExpressionContext context);

        [OperationContract]
        List<int> GetIntegerVectorValues(SymbolicExpressionContext context);

        [OperationContract]
        List<double> GetNumericVectorValues(SymbolicExpressionContext context);

        [OperationContract]
        List<bool> GetLogicalVectorValues(SymbolicExpressionContext context);

        [OperationContract]
        List<byte> GetRawVectorValues(SymbolicExpressionContext context);

        [OperationContract]
        List<Complex> GetComplexVectorValues(SymbolicExpressionContext context);

        [OperationContract]
        string GetCharacterVectorValueAt(SymbolicExpressionContext context, int index);

        [OperationContract]
        void SetCharacterVectorValueAt(SymbolicExpressionContext context, int index, string value);

        [OperationContract]
        SymbolicExpressionContext GetExpressionVectorValueAt(SymbolicExpressionContext context, int index);

        [OperationContract]
        void SetExpressionVectorValueAt(SymbolicExpressionContext context, int index, SymbolicExpressionContext value);

        [OperationContract]
        int GetIntegerVectorValueAt(SymbolicExpressionContext context, int index);

        [OperationContract]
        void SetIntegerVectorValueAt(SymbolicExpressionContext context, int index, int value);

        [OperationContract]
        double GetNumericVectorValueAt(SymbolicExpressionContext context, int index);

        [OperationContract]
        void SetNumericVectorValueAt(SymbolicExpressionContext context, int index, double value);

        [OperationContract]
        bool GetLogicalVectorValueAt(SymbolicExpressionContext context, int index);

        [OperationContract]
        void SetLogicalVectorValueAt(SymbolicExpressionContext context, int index, bool value);

        [OperationContract]
        byte GetRawVectorValueAt(SymbolicExpressionContext context, int index);

        [OperationContract]
        void SetRawVectorValueAt(SymbolicExpressionContext context, int index, byte value);

        [OperationContract]
        Complex GetComplexVectorValueAt(SymbolicExpressionContext context, int index);

        [OperationContract]
        void SetComplexVectorValueAt(SymbolicExpressionContext context, int index, Complex value);

        [OperationContract]
        void InitializeCharacterVector(SymbolicExpressionContext context, List<string> vector);

        [OperationContract]
        void InitializeIntegerVector(SymbolicExpressionContext context, List<int> vector);

        [OperationContract]
        void InitializeNumericVector(SymbolicExpressionContext context, List<double> vector);

        [OperationContract]
        void InitializeLogicalVector(SymbolicExpressionContext context, List<bool> vector);

        [OperationContract]
        void InitializeRawVector(SymbolicExpressionContext context, List<byte> vector);

        [OperationContract]
        void InitializeComplexVector(SymbolicExpressionContext context, List<Complex> vector);

        [OperationContract]
        List<SymbolicExpressionContext> GetSymbols(SymbolicExpressionContext context);

        [OperationContract]
        SymbolicExpressionContext GetUnboundValue();

        [OperationContract]
        SymbolicExpressionContext GetNilValue();

        [OperationContract]
        List<string> GetAttributeNames(SymbolicExpressionContext context);

        [OperationContract]
        string GetPrintName(SymbolicExpressionContext context);

        [OperationContract]
        void SetPrintName(SymbolicExpressionContext context, string value);

        [OperationContract]
        SymbolicExpressionContext GetDimNames();

        [OperationContract]
        bool IsPromise(SymbolicExpressionContext context);

        [OperationContract]
        SymbolicExpressionContext GetClosureBody(SymbolicExpressionContext context);

        [OperationContract]
        SymbolicExpressionContext GetClosureFormals(SymbolicExpressionContext context);

        [OperationContract]
        SymbolicExpressionContext GetClosureEnvironment(SymbolicExpressionContext context);

        [OperationContract]
        SymbolicExpressionContext GetLanguageList(SymbolicExpressionContext context);
        
        [OperationContract]
        SymbolicExpressionContext GetClass(SymbolicExpressionContext context);

        [OperationContract]
        SymbolicExpressionContext GetInternal(SymbolicExpressionContext context);

        [OperationContract]
        SymbolicExpressionContext GetValue(SymbolicExpressionContext context);

        [OperationContract]
        SymbolicExpressionContext GetLevels(SymbolicExpressionContext context);

        [OperationContract]
        List<List<double>> GetNumericMatrixValues(SymbolicExpressionContext context);

        [OperationContract]
        List<List<int>> GetIntegerMatrixValues(SymbolicExpressionContext context);

        [OperationContract]
        List<List<string>> GetCharacterMatrixValues(SymbolicExpressionContext context);

        [OperationContract]
        List<List<bool>> GetLogicalMatrixValues(SymbolicExpressionContext context);

        [OperationContract]
        List<List<byte>> GetRawMatrixValues(SymbolicExpressionContext context);

        [OperationContract]
        List<List<Complex>> GetComplexMatrixValues(SymbolicExpressionContext context);

        [OperationContract]
        string GetCharacterMatrixValueAt(SymbolicExpressionContext context, int row, int column);

        [OperationContract]
        void SetCharacterMatrixValueAt(SymbolicExpressionContext context, int row, int column, string value);

        [OperationContract]
        int GetIntegerMatrixValueAt(SymbolicExpressionContext context, int row, int column);

        [OperationContract]
        void SetIntegerMatrixValueAt(SymbolicExpressionContext context, int row, int column, int value);

        [OperationContract]
        double GetNumericMatrixValueAt(SymbolicExpressionContext context, int row, int column);

        [OperationContract]
        void SetNumericMatrixValueAt(SymbolicExpressionContext context, int row, int column, double value);

        [OperationContract]
        bool GetLogicalMatrixValueAt(SymbolicExpressionContext context, int row, int column);

        [OperationContract]
        void SetLogicalMatrixValueAt(SymbolicExpressionContext context, int row, int column, bool value);

        [OperationContract]
        byte GetRawMatrixValueAt(SymbolicExpressionContext context, int row, int column);

        [OperationContract]
        void SetRawMatrixValueAt(SymbolicExpressionContext context, int row, int column, byte value);

        [OperationContract]
        Complex GetComplexMatrixValueAt(SymbolicExpressionContext context, int row, int column);

        [OperationContract]
        void SetComplexMatrixValueAt(SymbolicExpressionContext context, int row, int column, Complex value);

        [OperationContract]
        void InitializeCharacterMatrix(SymbolicExpressionContext context, List<List<string>> matrix);

        [OperationContract]
        void InitializeIntegerMatrix(SymbolicExpressionContext context, List<List<int>> matrix);

        [OperationContract]
        void InitializeNumericMatrix(SymbolicExpressionContext context, List<List<double>> matrix);

        [OperationContract]
        void InitializeLogicalMatrix(SymbolicExpressionContext context, List<List<bool>> matrix);

        [OperationContract]
        void InitializeRawMatrix(SymbolicExpressionContext context, List<List<byte>> matrix);

        [OperationContract]
        void InitializeComplexMatrix(SymbolicExpressionContext context, List<List<Complex>> matrix);

        [OperationContract]
        SymbolicExpressionContext GetParentEnvironment(SymbolicExpressionContext context);
    }
}
