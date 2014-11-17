using System.Collections.Generic;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
    public class RawVector : Vector<byte>, IRawVector
    {
        public RawVector(int length, IRObjectAPI api)
            : base(length, SymbolicExpressionType.RawVector, api)
        { }

        public RawVector(IList<byte> vector, IRObjectAPI api)
            : base(vector, SymbolicExpressionType.RawVector, api)
        { }

        public RawVector(IRSafeHandle handle)
            : base(handle, SymbolicExpressionType.RawVector)
        {
            CoerceVector(SymbolicExpressionType.RawVector);
        }
    }
}
