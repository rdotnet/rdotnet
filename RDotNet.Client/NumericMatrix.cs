using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class NumericMatrix : Matrix<double>, INumericMatrix
   {
      public NumericMatrix(int rowCount, int columnCount, IRObjectAPI api)
         : base(rowCount, columnCount, SymbolicExpressionType.NumericVector, api)
      { }

      public NumericMatrix(double[,] matrix, IRObjectAPI api)
         : base(matrix, SymbolicExpressionType.NumericVector, api)
      { }
   }
}
