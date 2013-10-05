using System.Diagnostics;

namespace RDotNet.Diagnostics
{
   internal class MatrixDebugView<T>
   {
      private readonly Matrix<T> matrix;

      public MatrixDebugView(Matrix<T> matrix)
      {
         this.matrix = matrix;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public T[,] Value
      {
         get
         {
            var array = new T[this.matrix.RowCount, this.matrix.ColumnCount];
            this.matrix.CopyTo(array, this.matrix.RowCount, this.matrix.ColumnCount);
            return array;
         }
      }
   }
}