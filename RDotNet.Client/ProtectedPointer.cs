using System;

namespace RDotNet.Client
{
    public class ProtectedPointer : IDisposable
    {
        private readonly IRObjectAPI _api;
        private readonly IRSafeHandle _sexp;

        private ProtectedPointer(IRObjectAPI api)
        {
            if (api == null) throw new ArgumentNullException("api");

            _api = api;
        }

        public ProtectedPointer(IRObjectAPI api, IRSafeHandle sexp)
            : this(api)
        {
            _sexp = sexp;
            api.Protect(_sexp);
        }

        public static ProtectedPointer Protect(IRObjectAPI api, IRSafeHandle sexp)
        {
            return new ProtectedPointer(api, sexp);
        }

        public static ProtectedPointer Protect(IRObjectAPI api, SymbolicExpression sexp)
        {
            return Protect(api, sexp.Handle);
        }

        public void Dispose()
        {
            _api.UnprotectPtr(_sexp);
        }
    }
}
