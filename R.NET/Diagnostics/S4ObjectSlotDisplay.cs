using System;
using System.Diagnostics;
using System.Linq;

namespace RDotNet.Diagnostics
{
    [DebuggerDisplay("{Display,nq}")]
    internal class S4ObjectSlotDisplay
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly S4Object s4obj;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string name;

        public S4ObjectSlotDisplay(S4Object obj, string name)
        {
            this.s4obj = obj;
            this.name = name;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public SymbolicExpression Value
        {
            get
            {
                return s4obj[name];
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Display
        {
            get
            {
                var slot = this.Value;
                var names = this.s4obj.SlotNames;
                if (names == null || !names.Contains(name))
                {
                    return String.Format("NA ({0})", slot.Type);
                }
                else
                {
                    return String.Format("\"{0}\" ({1})", name, slot.Type);
                }
            }
        }
    }
}