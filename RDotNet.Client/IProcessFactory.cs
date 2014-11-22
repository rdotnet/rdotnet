using System.Collections.Generic;

namespace RDotNet.Client
{
    public interface IProcessFactory
    {
        IProcess Create( string path, string args, IDictionary<string, string> environmentVariables );
        List<IProcess> GetProcessesByName(string name);
    }
}
