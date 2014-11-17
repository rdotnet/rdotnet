using System;

namespace RDotNet.ProcessHost
{
    public class ProcessMonitor : IProcessMonitor
    {
        private readonly Action _killCallback;

        public ProcessMonitor(int id, ServiceState state, Action killCallback)
        {
            if (killCallback == null) throw new ArgumentNullException("killCallback");

            Id = id;
            State = state;
            _killCallback = killCallback;
            LastAliveCheck = DateTime.Now;
        }

        public int Id { get; set; }
        public ServiceState State { get; set; }
        public DateTime LastAliveCheck { get; private set; }

        public bool IsAlive()
        {
            LastAliveCheck = DateTime.Now;
            return true;
        }

        public void Kill()
        {
            _killCallback();
        }
    }
}
