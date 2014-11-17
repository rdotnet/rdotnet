using System.Collections.Generic;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
    public class CharacterVector : Vector<string>, ICharacterVector
    {
        public CharacterVector(int length, IRObjectAPI api)
            : base(length, SymbolicExpressionType.CharacterVector, api)
        { }

        public CharacterVector(IList<string> vector, IRObjectAPI api)
            : base(vector, SymbolicExpressionType.CharacterVector, api)
        { }

        public CharacterVector(IRSafeHandle handle)
            : base(handle, SymbolicExpressionType.CharacterVector)
        {
            CoerceVector(SymbolicExpressionType.CharacterVector);
        }
    }
}
