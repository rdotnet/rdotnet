using System;
using System.Collections.Generic;

namespace RDotNet.Client
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DataFrameColumnAttribute : Attribute
    {

        public int Index { get; private set; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (Index < 0 && value == null)
                {
                    throw new ArgumentNullException("value", "Name must not be null when Index is not defined.");
                }
                _name = value;
            }
        }

        public DataFrameColumnAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this._name = name;
            Index = -1;
        }

        public DataFrameColumnAttribute(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            _name = null;
            Index = index;
        }

        internal int GetIndex(IList<string> names)
        {
            if (Index >= 0) return Index;
            if (names == null) return -1;
            return names.IndexOf(Name);
        }
    }
}
