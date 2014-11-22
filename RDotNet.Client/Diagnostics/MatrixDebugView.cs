using System.Diagnostics;

namespace RDotNet.Client.Diagnostics
{
   internal class MatrixDebugView<T>
   {
      private readonly Matrix<T> _matrix;

      public MatrixDebugView(Matrix<T> matrix)
      {
         _matrix = matrix;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public T[,] Value
      {
         get
         {
            return _matrix.ToArray();
         }
      }
   }
}
