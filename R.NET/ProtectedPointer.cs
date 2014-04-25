using RDotNet.Internals;
using System;
using System.Security.Permissions;

namespace RDotNet
{
   [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
   internal class ProtectedPointer : IDisposable
   {
      private readonly REngine engine;
      protected TDelegate GetFunction<TDelegate>() where TDelegate : class
      {
         return engine.GetFunction<TDelegate>();
      }

      private readonly IntPtr sexp;

      public ProtectedPointer(REngine engine, IntPtr sexp)
      {
         this.sexp = sexp;
         this.engine = engine;

         this.GetFunction<Rf_protect>()(this.sexp);
      }

      public ProtectedPointer(SymbolicExpression sexp)
      {
         this.sexp = sexp.DangerousGetHandle();
         this.engine = sexp.Engine;

         this.GetFunction<Rf_protect>()(this.sexp);
      }

      #region IDisposable Members

      public void Dispose()
      {
         this.GetFunction<Rf_unprotect_ptr>()(this.sexp);
      }

      #endregion IDisposable Members

      public static implicit operator IntPtr(ProtectedPointer p)
      {
         return p.sexp;
      }
   }
}