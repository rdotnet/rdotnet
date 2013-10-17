using RDotNet.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RDotNet
{
   /// <summary>
   /// A pairlist.
   /// </summary>
   public class Pairlist : SymbolicExpression, IEnumerable<Symbol>
   {
      /// <summary>
      /// Creates a pairlist.
      /// </summary>
      /// <param name="engine">The engine</param>
      /// <param name="pointer">The pointer.</param>
      protected internal Pairlist(REngine engine, IntPtr pointer)
         : base(engine, pointer)
      { }

      /// <summary>
      /// Gets the number of nodes.
      /// </summary>
      public int Count
      {
         get { return Engine.GetFunction<Rf_length>()(handle); }
      }

      #region IEnumerable<Symbol> Members

      public IEnumerator<Symbol> GetEnumerator()
      {
         if (Count != 0)
         {
            for (SEXPREC sexp = GetInternalStructure(); sexp.sxpinfo.type != SymbolicExpressionType.Null; sexp = (SEXPREC)Marshal.PtrToStructure(sexp.listsxp.cdrval, typeof(SEXPREC)))
            {
               yield return new Symbol(Engine, sexp.listsxp.tagval);
            }
         }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion IEnumerable<Symbol> Members
   }
}