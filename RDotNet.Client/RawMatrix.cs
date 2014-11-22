using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class RawMatrix : Matrix<byte>, IRawMatrix
   {
      public RawMatrix(int rowCount, int columnCount, IRObjectAPI api)
         : base(rowCount, columnCount, SymbolicExpressionType.RawVector, api)
      { }

      public RawMatrix(byte[,] matrix, IRObjectAPI api)
         : base(matrix, SymbolicExpressionType.RawVector, api)
      { }
   }
}
