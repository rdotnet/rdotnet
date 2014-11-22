using System.Collections;
using System.Collections.Generic;

namespace RDotNet.Client
{
    public class PairList : SymbolicExpression, IEnumerable<Symbol>
    {
        public PairList(IRSafeHandle handle)
            : base(handle)
        { }

        public int Count
        {
            get { return GetLength(); }
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            return GetSymbols().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
