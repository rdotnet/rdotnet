using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace RDotNet.ProcessHost
{
    public class RServiceHost
    {
        private readonly Uri _uri;
        private readonly TextWriter _output;
        private ServiceHost _msHost;
        private ServiceHost _lsHost;
        private ServiceHost _osHost;

        public RServiceHost(Uri uri, TextWriter output)
        {
            if (uri == null) throw new ArgumentNullException("uri");
            if (output == null) throw new ArgumentNullException("output");

            _uri = uri;
            _output = output;
        }

        public void Start(int id)
        {
            var managementUri = new Uri(_uri + "RManagementService/" + id);
            _msHost = new ServiceHost(typeof(Server.Services.RManagementService), managementUri);
            var languageUri = new Uri(_uri + "RLanguageService/" + id);
            _lsHost = new ServiceHost(typeof(Server.Services.RLanguageService), languageUri);
            var outputUri = new Uri(_uri + "ROutputService/" + id);
            _osHost = new ServiceHost(typeof(Server.Services.ROutputService), outputUri);

            _output.Write("Starting RManagementService ({0})...", managementUri);
            var sd = _msHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            sd.IncludeExceptionDetailInFaults = true;
            _msHost.Open();
            _output.WriteLine("OK");

            _output.Write("Starting RLanguageService   ({0}).....", languageUri);
            sd = _lsHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            sd.IncludeExceptionDetailInFaults = true;
            _lsHost.Open();
            _output.WriteLine("OK");

            _output.Write("Starting ROutputService     ({0}).......", outputUri, 1);
            sd = _osHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            sd.IncludeExceptionDetailInFaults = true;
            _osHost.Open();
            _output.WriteLine("OK");
        }

        public void Stop()
        {
            _lsHost.Close();
            _msHost.Close();
            _osHost.Close();
        }
    }
}
