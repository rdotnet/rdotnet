using System;
using System.Collections.Generic;
using System.Linq;
using RDotNet.Internals;

namespace RDotNet
{
   /// <summary>
   /// S4 objects.
   /// </summary>
   public class S4Object : SymbolicExpression
   {
      protected internal S4Object(REngine engine, IntPtr pointer)
         : base(engine, pointer)
      { }

      /// <summary>
      /// Gets the class representation.
      /// </summary>
      /// <returns>The class representation of the S4 class.</returns>
      public S4Object GetClassDefinition()
      {
         var classSymbol = Engine.GetPredefinedSymbol("R_ClassSymbol");
         var className = GetAttribute(classSymbol).AsCharacter().First();
         var definition = Engine.GetFunction<R_getClassDef>()(className);
         return new S4Object(Engine, definition);
      }

      /// <summary>
      /// Gets slot names.
      /// </summary>
      /// <returns>Slot names.</returns>
      public IDictionary<string, string> GetSlotTypes()
      {
         var definition = GetClassDefinition();
         var slots = definition.GetSlot("slots");
         var namesSymbol = Engine.GetPredefinedSymbol("R_NamesSymbol");
         return slots.GetAttribute(namesSymbol).AsCharacter()
            .Zip(slots.AsCharacter(), (name, type) => new { Name = name, Type = type })
            .ToDictionary(t => t.Name, t => t.Type);
      }

      /// <summary>
      /// Gets slot.
      /// </summary>
      /// <param name="slotName">The slot name.</param>
      /// <returns>The slot.</returns>
      public SymbolicExpression GetSlot(string slotName)
      {
         if (slotName == null)
         {
            throw new ArgumentNullException();
         }
         var installedName = Engine.GetFunction<Rf_install>()(slotName);
         var slot = Engine.GetFunction<R_do_slot>()(this.handle, installedName);
         if (Engine.CheckNil(slot))
         {
            return null;
         }
         return new SymbolicExpression(Engine, slot);
      }

      /// <summary>
      /// Set slot.
      /// </summary>
      /// <param name="slotName">The slot name.</param>
      /// <param name="value">The new value.</param>
      public void SetSlot(string slotName, SymbolicExpression value)
      {
         if (slotName == null)
         {
            throw new ArgumentNullException();
         }
         var installedName = Engine.GetFunction<Rf_install>()(slotName);
         Engine.GetFunction<R_do_slot_assign>()(this.handle, installedName, value.DangerousGetHandle());
      }

      /// <summary>
      /// Checks if the specified name is defined in the class.
      /// </summary>
      /// <param name="name">The slot name.</param>
      /// <returns><c>True</c> if the class has the slot; otherwise, <c>false</c>.</returns>
      public bool HasSlot(string name)
      {
         var installedName = Engine.GetFunction<Rf_install>()(name);
         return Engine.GetFunction<R_has_slot>()(this.handle, installedName);
      }
   }
}
