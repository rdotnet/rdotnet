using System;
using System.Diagnostics;

namespace RDotNet.Client.Diagnostics
{
   [DebuggerDisplay("{Display,nq}")]
   internal class S4ObjectSlotDisplay
   {
      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      private readonly S4Object _s4Obj;

      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      private readonly string name;

      public S4ObjectSlotDisplay(S4Object obj, string name)
      {
         _s4Obj = obj;
         this.name = name;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public SymbolicExpression Value
      {
         get
         {
            return _s4Obj[name];
         }
      }

      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      public string Display
      {
         get
         {
            var slot = Value;
            var names = _s4Obj.GetSlotNames();
            if (names == null || ! names.Contains(name))
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
