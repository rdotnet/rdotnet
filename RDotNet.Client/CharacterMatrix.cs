using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class CharacterMatrix : Matrix<string>, ICharacterMatrix
    {
      public CharacterMatrix(int rowCount, int columnCount, IRObjectAPI api)
         : base(rowCount, columnCount, SymbolicExpressionType.CharacterVector, api)
      { }

      public CharacterMatrix(string[,] matrix, IRObjectAPI api)
         : base(matrix, SymbolicExpressionType.CharacterVector, api)
      { }
   }
}
