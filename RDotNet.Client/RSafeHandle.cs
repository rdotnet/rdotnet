using System;
using System.Runtime.InteropServices;
using RDotNet.Server;

namespace RDotNet.Client
{
    public interface IRSafeHandle
    {
        bool IsProtected { get; }
        IRObjectAPI API { get; }
        bool IsInvalid { get;  }
        SymbolicExpressionContext Context { get; }

        void Preserve();
    }

    public class RSafeHandle : SafeHandle, IRSafeHandle
    {
        public RSafeHandle(SymbolicExpressionContext context, IRObjectAPI api)
            : base(IntPtr.Zero, true)
        {
            if (api == null) throw new ArgumentNullException("api");
            if (context == null) throw new ArgumentNullException("context");

            Context = context;
            API = api;

            SetHandle(context.Handle);

            Preserve();
        }

        public bool IsProtected { get; private set; }
        public IRObjectAPI API { get; private set; }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        public SymbolicExpressionContext Context { get; private set; }

        public void Preserve()
        {
            if (IsInvalid || IsProtected) return;

            API.PreserveObject(this);

            IsProtected = true;
        }

        protected override bool ReleaseHandle()
        {
            if (IsInvalid || !IsProtected) return true;

            API.ReleaseObject(this);

            SetHandleAsInvalid();
            IsProtected = false;

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseHandle();
        }
    }
}
