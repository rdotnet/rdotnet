using System.Diagnostics;
using System.Linq;

namespace RDotNet.Client.Diagnostics
{
   internal class VectorDebugView<T>
   {
      private readonly Vector<T> _vector;

      public VectorDebugView(Vector<T> vector)
      {
         _vector = vector;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public T[] Value
      {
         get { return _vector.ToArray(); }
      }
   }
}
