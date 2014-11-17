using System;
using System.Runtime.InteropServices;
using RDotNet.R.Adapter.Graphics;

namespace RDotNet.R.Adapter
{
    public class InterruptManager : IDisposable
    {
        private readonly RInstance _dll;
        private readonly bool _oldState;

        public InterruptManager(RInstance dll)
        {
            _dll = dll;
            _oldState = GetInterruptsSuspended();
            SetInterruptsSuspended(true);
        }

        public static InterruptManager SuspendInterrupts(RInstance dll)
        {
            return new InterruptManager(dll);
        }

        public void SetInterruptsSuspended(bool value)
        {
            var pointer = _dll.GetPointer("R_interrupts_suspended");
            Marshal.WriteInt32(pointer, Convert.ToInt32(value));
        }

        public bool GetInterruptsSuspended()
        {
            var pointer = _dll.GetPointer("R_interrupts_suspended");
            return Convert.ToBoolean(Marshal.ReadInt32(pointer));
        }

        public bool GetInterruptsPending()
        {
            var pointer = _dll.GetPointer("R_interrupts_pending");
            return Convert.ToBoolean(Marshal.ReadInt32(pointer));
        }

        private void OinkOink()
        {
            if (!GetInterruptsPending() || GetInterruptsSuspended()) return;

            _dll.GetFunction<Rf_onintr>();
        }
        
        public void Dispose()
        {
            SetInterruptsSuspended(_oldState);
            OinkOink();
        }
    }
}
