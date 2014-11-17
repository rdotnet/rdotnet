using RDotNet.Client.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
    [DebuggerDisplay("Length = {Length}; RObjectType = {Type}")]
    [DebuggerTypeProxy(typeof (VectorDebugView<>))]
    public class Vector<T> : SymbolicExpression, IEnumerable<T>
    {
        private readonly SymbolicExpressionType _type;

        public Vector(int length, SymbolicExpressionType type, IRObjectAPI api)
            : base(api.AllocateVector(type, length))
        {
            _type = type;
            ClearVector(type, length);
        }

        public Vector(IList<T> vector, SymbolicExpressionType type, IRObjectAPI api)
            : this(vector.Count(), type, api)
        {
            InitializeVector(type, vector);
        }

        public Vector(IRSafeHandle handle, SymbolicExpressionType type)
            : base(handle)
        {
            _type = type;
        }

        public T this[string name]
        {
            get
            {
                var index = GetIndex(name);
                return this[index];
            }
            set
            {
                var index = GetIndex(name);
                this[index] = value;
            }
        }

        private int GetIndex(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException();

            var names = GetNames();
            if (names == null) throw new InvalidOperationException();

            var index = names.IndexOf(name);
            return index;
        }

        public int Length
        {
            get { return GetLength(); }
        }

        public T this[int index]
        {
            get { return GetVectorValueAt<T>(_type, index); }
            set { SetVectorValueAt(_type, index, value); }
        }

        public IList<T> ToList()
        {
            return GetVectorValues<T>(_type);
        }

        public IList<string> GetNames()
        {
            var namesSymbol = GetNamesSymbol();
            var names = GetAttribute(namesSymbol);
            if (names == null) return null;

            var result = names.ToCharacterVector().ToList();
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var index = 0; index < Length; index++)
            {
                yield return this[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
