using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.Configuration;

namespace RDotNet.Client
{
    public interface IHostConfiguration
    {
        string GetProcessType();
        string GetBindingType();
        string GetHostUri();
        string GetHostPath();
        IDictionary<String, String> GetEnvironmentVariables();
        Uri CreateUriFromConfiguration();
        Binding CreateBindingFromConfiguration();
    }

    public class HostConfiguration : IHostConfiguration
    {
        public string GetProcessType()
        {
            var setting = WebConfigurationManager.AppSettings["RProcessType"];
            if (string.IsNullOrWhiteSpace(setting)) return "OutOfProcess";

            return setting;
        }

        public string GetBindingType()
        {
            var setting = WebConfigurationManager.AppSettings["RBindingType"];
            if (string.IsNullOrWhiteSpace(setting)) return "net.pipe";

            return setting;
        }

        public string GetHostUri()
        {
            var setting = WebConfigurationManager.AppSettings["RRootUri"];
            if (string.IsNullOrWhiteSpace(setting)) return "localhost";

            return setting;
        }

        public string GetHostPath()
        {
            var binPath = WebConfigurationManager.AppSettings["binPath"];
            var setting = WebConfigurationManager.AppSettings["RHostPath"];
            if (string.IsNullOrWhiteSpace(setting))
                return Path.Combine(binPath, "RDotNet.ProcessHost.exe");

            return setting;
        }

        public IDictionary<string, string> GetEnvironmentVariables()
        {
            var environmentVariables = new Dictionary<string, string>();
            var rProfileUserPath = WebConfigurationManager.AppSettings["RProfileUserPath"];
            if (!String.IsNullOrWhiteSpace(rProfileUserPath))
            {
                environmentVariables.Add( "R_PROFILE_USER", rProfileUserPath );
            }
            return environmentVariables;
        }

        public Uri CreateUriFromConfiguration()
        {
            var bindingType = GetBindingType();
            var hostUri = GetHostUri();
            var fullUri = new Uri(string.Format("{0}://{1}", bindingType, hostUri));
            return fullUri;
        }

        public Binding CreateBindingFromConfiguration()
        {
            var bindingType = GetBindingType();
            switch (bindingType)
            {
                case "net.tcp":
                    return new NetTcpBinding { MaxReceivedMessageSize = Int32.MaxValue };
                case "net.pipe":
                    return new NetNamedPipeBinding { MaxReceivedMessageSize = Int32.MaxValue };
                case "https":
                    return new BasicHttpsBinding { MaxReceivedMessageSize = Int32.MaxValue };
                default:
                    return new BasicHttpBinding { MaxReceivedMessageSize = Int32.MaxValue };
            }
        }

    }
}
