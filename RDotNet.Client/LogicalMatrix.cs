using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class LogicalMatrix : Matrix<bool>, ILogicalMatrix
   {
      public LogicalMatrix(int rowCount, int columnCount, IRObjectAPI api)
         : base(rowCount, columnCount, SymbolicExpressionType.LogicalVector, api)
      { }

      public LogicalMatrix(bool[,] matrix, IRObjectAPI api)
         : base(matrix, SymbolicExpressionType.LogicalVector, api)
      { }
   }
}
