using System.Numerics;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class ComplexMatrix : Matrix<Complex>, IComplexMatrix 
   {
      public ComplexMatrix(int rowCount, int columnCount, IRObjectAPI api)
         : base(rowCount, columnCount, SymbolicExpressionType.ComplexVector, api)
      { }

      public ComplexMatrix(Complex[,] matrix, IRObjectAPI api)
         : base(matrix, SymbolicExpressionType.CharacterVector, api)
      { }
   }
}
