using RDotNet.Diagnostics;
using RDotNet.Dynamic;
using RDotNet.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
   /// <summary>
   /// An S4 object
   /// </summary>
   [DebuggerDisplay(@"SlotCount = {SlotCount}; RObjectType = {Type}")]
   [DebuggerTypeProxy(typeof(S4ObjectDebugView))]
   [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
   public class S4Object : SymbolicExpression
   {
      private static Function slotNamesFunc = null;

      /// <summary>
      /// Create a new S4 object
      /// </summary>
      /// <param name="engine">R engine</param>
      /// <param name="pointer">pointer to native S4 SEXP</param>
      protected internal S4Object(REngine engine, IntPtr pointer)
         : base(engine, pointer)
      {
         if (slotNamesFunc== null)
            slotNamesFunc = Engine.Evaluate("slotNames").AsFunction();
      }

      /// <summary>
      /// Gets/sets the value of a slot
      /// </summary>
      /// <param name="name"></param>
      /// <returns></returns>
      public SymbolicExpression this[string name]
      {
         get 
         {
            checkSlotName(name);
            var s = GetFunction<Rf_mkString>()(name);
            var slotValue = this.GetFunction<R_do_slot>()(this.DangerousGetHandle(), s);
            return new SymbolicExpression(Engine, slotValue);}
         set
         {
            checkSlotName(name);
            var s = GetFunction<Rf_mkString>()(name);
            using (new ProtectedPointer(this))
            {
               this.GetFunction<R_do_slot_assign>()(this.DangerousGetHandle(), s, value.DangerousGetHandle());
            }
         }
      }

      private void checkSlotName(string name)
      {
         if (!SlotNames.Contains(name))
            throw new ArgumentException(string.Format("Invalid slot name '{0}'", name), "name");
      }


      /// <summary>
      /// Is a slot name valid.
      /// </summary>
      /// <param name="slotName">the name of the slot</param>
      /// <returns>whether a slot name is present in the object</returns>
      public bool HasSlot(string slotName)
      {
         var s = GetFunction<Rf_mkString>()(slotName);
         return this.GetFunction<R_has_slot>()(this.DangerousGetHandle(), s);
      }

      private string[] slotNames = null;
      /// <summary>
      /// Gets the slot names for this object. The values are cached once retrieved the first time.
      /// </summary>
      public string[] SlotNames
      {
         get
         {
            if (slotNames==null)
               slotNames = slotNamesFunc.Invoke(this).AsCharacter().ToArray();
            return (string[])slotNames.Clone();
         }
      }

      /// <summary>
      /// Gets the number of slot names
      /// </summary>
      public int SlotCount
      {
         get { return SlotNames.Length; }
      }
   }
}