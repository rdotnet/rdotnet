using System.Numerics;

namespace RDotNet.Client
{
    public interface IBuiltinFunction : IFunction { }
    public interface IClosure : IFunction { }
    public interface ICharacterMatrix : IMatrix<string> { }
    public interface ICharacterVector : IVector<string> { }
    public interface IComplexMatrix : IMatrix<Complex> { }
    public interface IComplexVector : IVector<Complex> { }
    public interface IDynamicVector : IVector<object> {}
    public interface IIntegerMatrix : IMatrix<int> { }
    public interface IIntegerVector : IVector<int> { }
    public interface ILogicalMatrix : IMatrix<bool> { }
    public interface ILogicalVector : IVector<bool> { }
    public interface INumericMatrix : IMatrix<double> { }
    public interface INumericVector : IVector<double> { }
    public interface IRawMatrix : IMatrix<byte> { }
    public interface IRawVector : IVector<byte> { }
    public interface ISpecialFunction : IFunction { }
    public interface IGenericVector : IVector<RSafeHandle> { }
    public interface IMatrix<T> { }
    public interface IVector<T> { }
    public interface IFunction { }
    public interface IDataFrame { }
}
