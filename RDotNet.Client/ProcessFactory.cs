using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RDotNet.Client
{
    public class ProcessFactory : IProcessFactory
    {
        public IProcess Create(string path, string args, IDictionary<string, string> environmentVariables )
        {
            var psi = new ProcessStartInfo(path, args)
            {
                
                CreateNoWindow = false,
                ErrorDialog = false,
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = true,
            };

            foreach (var environmentVariable in environmentVariables)
            {
                Environment.SetEnvironmentVariable( environmentVariable.Key, environmentVariable.Value );
            }
            var process = Process.Start(psi);
            return new ProcessWrapper(process);
        }

        public List<IProcess> GetProcessesByName(string name)
        {
            return new List<IProcess>(Process.GetProcessesByName(name).Select(s => new ProcessWrapper(s)));
        }
    }
}
