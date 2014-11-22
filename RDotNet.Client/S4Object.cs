using System.Linq;
using RDotNet.Client.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RDotNet.Client
{
    [DebuggerDisplay(@"SlotCount = {SlotCount}; RObjectType = {Type}")]
    [DebuggerTypeProxy(typeof (S4ObjectDebugView))]
    public class S4Object : SymbolicExpression
    {
        public S4Object(IRSafeHandle handle)
            : base(handle)
        { }

        public SymbolicExpression this[string name]
        {
            get
            {
                CheckSlotName(name);
                return DoSlot(name);
            }
            set
            {
                CheckSlotName(name);
                DoSlotAssign(name, value);
            }
        }

        private void CheckSlotName(string name)
        {
            var result = GetSlotNames();

            if (!result.Contains(name))
                throw new ArgumentException(string.Format("Invalid slot name '{0}'", name), "name");
        }


        public S4Object GetClassDefinition()
        {
            var classSymbol = GetClass();
            var attribute = GetAttribute(classSymbol);
            var className = attribute.ToCharacterVector();
            var definition = GetClassDefinition(className[0]);
            return definition.ToS4Object();
        }

        public IDictionary<string, string> GetSlotTypes()
        {
            var definition = GetClassDefinition();
            var slots = definition["slots"];
            var namesSymbol = GetNamesSymbol();
            var attribute = slots.GetAttribute(namesSymbol);
            var result = attribute.ToCharacterVector();
            return result.Zip(slots.ToCharacterVector(), (name, type) => new {Name = name, Type = type})
                .ToDictionary(t => t.Name, t => t.Type);
        }
    }
}
