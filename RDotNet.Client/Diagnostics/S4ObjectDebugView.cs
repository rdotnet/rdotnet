using System.Diagnostics;
using System.Linq;

namespace RDotNet.Client.Diagnostics
{
   internal class S4ObjectDebugView
   {
      private readonly S4Object _s4Obj;

      public S4ObjectDebugView(S4Object obj)
      {
         _s4Obj = obj;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public S4ObjectSlotDisplay[] Slots
      {
         get
         {
            return _s4Obj.GetSlotNames()
               .Select(name => new S4ObjectSlotDisplay(_s4Obj, name))
               .ToArray();
         }
      }
   }
}
