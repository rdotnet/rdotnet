using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class IntegerMatrix : Matrix<int>, IIntegerMatrix
   {
      public IntegerMatrix(int rowCount, int columnCount, IRObjectAPI api)
           : base(rowCount, columnCount, SymbolicExpressionType.IntegerVector, api)
      { }

      public IntegerMatrix(int[,] matrix, IRObjectAPI api)
         : base(matrix, SymbolicExpressionType.IntegerVector, api)
      { }
   }
}
