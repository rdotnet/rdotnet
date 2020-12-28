using System.Diagnostics;

namespace RDotNet.Diagnostics
{
    internal class VectorDebugView<T>
    {
        private readonly Vector<T> vector;

        public VectorDebugView(Vector<T> vector)
        {
            this.vector = vector;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Value
        {
            get
            {
                var array = new T[this.vector.Length];
                this.vector.CopyTo(array, array.Length);
                return array;
            }
        }
    }
}