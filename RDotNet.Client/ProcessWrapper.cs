using System;
using System.Diagnostics;

namespace RDotNet.Client
{
    public class ProcessWrapper : IProcess
    {
        private readonly Process _process;

        public ProcessWrapper(Process process)
        {
            if (process == null) throw new ArgumentNullException("process");
            _process = process;
        }

        public void Kill() { _process.Kill(); }
        public string MainWindowTitle { get { return _process.MainWindowTitle; } }
        public bool HasExited { get { return _process.HasExited; } }
    }
}
