using System;
using System.ServiceModel;
using RDotNet.Client.ProcessMonitor;

namespace RDotNet.Client
{
    public interface IAPIFactory
    {
        IRManagementAPI CreateRManagementAPI(int id);
        IRObjectAPI CreateRObjectAPI(int id);
        IROutputAPI CreateROutputAPI(int id);
        IProcessMonitor CreateProcessMonitorAPI(int id);
    }

    public class RRemoteAPIFactory : IAPIFactory
    {
        private readonly IHostConfiguration _hostConfiguration;

        public RRemoteAPIFactory(IHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
        }

        public IRManagementAPI CreateRManagementAPI(int id)
        {
            var binding = _hostConfiguration.CreateBindingFromConfiguration();
            var uri = _hostConfiguration.CreateUriFromConfiguration();

            return new RemoteRManagementAPI(binding, uri, id);
        }

        public IRObjectAPI CreateRObjectAPI(int id)
        {
            var binding = _hostConfiguration.CreateBindingFromConfiguration();
            var uri = _hostConfiguration.CreateUriFromConfiguration();

            return new RemoteRObjectAPI(binding, uri, id);
        }

        public IROutputAPI CreateROutputAPI(int id)
        {
            var binding = _hostConfiguration.CreateBindingFromConfiguration();
            var uri = _hostConfiguration.CreateUriFromConfiguration();

            return new RemoteROuputAPI(binding, uri, id);
        }

        public IProcessMonitor CreateProcessMonitorAPI(int id)
        {
            var binding = _hostConfiguration.CreateBindingFromConfiguration();
            binding.OpenTimeout = new TimeSpan(0, 0, 0, 10);
            var uri = _hostConfiguration.CreateUriFromConfiguration();
            var address = new EndpointAddress(uri + "ProcessMonitor/" + id);
            return new ProcessMonitorClient(binding, address);
        }
    }
}
