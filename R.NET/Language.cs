using RDotNet.Internals;
using System;

namespace RDotNet
{
   /// <summary>
   /// A language object.
   /// </summary>
   public class Language : SymbolicExpression
   {
      /// <summary>
      /// Creates a language object.
      /// </summary>
      /// <param name="engine">The engine</param>
      /// <param name="pointer">The pointer.</param>
      protected internal Language(REngine engine, IntPtr pointer)
         : base(engine, pointer)
      { }

      /// <summary>
      /// Gets function calls.
      /// </summary>
      public Pairlist FunctionCall
      {
         get
         {
            int count = Engine.GetFunction<Rf_length>()(handle);
            // count == 1 for empty call.
            if (count < 2)
            {
               return null;
            }
            SEXPREC sexp = GetInternalStructure();
            return new Pairlist(Engine, sexp.listsxp.cdrval);
         }
      }
   }
}