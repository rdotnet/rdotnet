using System;
using Microsoft.Win32.SafeHandles;

namespace RDotNet.R.Adapter
{
    public class DynamicLibrarySafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private readonly IPlatformManager _loader;

        public DynamicLibrarySafeHandle(IntPtr handle, IPlatformManager loader, bool ownsHandle = true)
            : base(ownsHandle)
        {
            if (loader == null) throw new ArgumentNullException("loader");
            _loader = loader;
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            return _loader.FreeLibrary(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsInvalid)
            {
                ReleaseHandle();
                handle = IntPtr.Zero;
            }
            base.Dispose(disposing);
        }
    }
}
