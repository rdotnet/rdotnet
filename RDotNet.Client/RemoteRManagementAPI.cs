using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using RDotNet.Client.RManagementService;
using RDotNet.Server;

namespace RDotNet.Client
{
    public class RemoteRManagementAPI : IRManagementAPI
    {
        private readonly RManagementServiceClient _client;
        private StartupParameter _startupParameter;

        public RemoteRManagementAPI(Binding binding, Uri rootUri, int id)
        {
            _client = new RManagementServiceClient(binding, new EndpointAddress(rootUri + "RManagementService/" + id));
        }

        public void Start(StartupParameter startupParameter)
        {
            _client.Start(startupParameter);
            _startupParameter = startupParameter;
        }

        public void Restart()
        {
            _client.Terminate();
            _client.Start(_startupParameter);
        }

        public void Stop()
        {
            _client.Terminate();
        }

        public void ForceGarbageCollection()
        {
            _client.ForceGarbageCollection();
        }

        public void Poke()
        {
            _client.Poke();
        }

        public TimeSpan LastPoke()
        {
            return _client.LastPoke();
        }

        public bool IsAlive()
        {
            return _client.IsAlive();
        }
    }
}
