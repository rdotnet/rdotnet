using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using RDotNet.Client.ROutputService;

namespace RDotNet.Client
{
    public class RemoteROuputAPI : IROutputAPI
    {
        private readonly ROutputServiceClient _client;

        public RemoteROuputAPI(Binding binding, Uri rootUri, int id)
        {
            _client = new ROutputServiceClient(binding, new EndpointAddress(rootUri + "ROutputService/" + id));
        }

        public void ResetConsole()
        {
            _client.ClearText();
        }

        public void ResetPlots()
        {
            _client.ClearPlots();
        }

        public List<string> GetPlots()
        {
            var result = _client.GetAllPlots();
            return result;
        }

        public List<string> GetConsole()
        {
            var result = _client.GetText();
            return result;
        }
    }
}
