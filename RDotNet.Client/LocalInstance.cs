using System;
using System.ServiceModel;

namespace RDotNet.Client
{
    public interface ILocalInstance : IDisposable
    {
        void Create(Uri fullUri, int id);
    }

    public class LocalInstance : ILocalInstance
    {
        private bool _started;
        private ServiceHost _l;
        private ServiceHost _m;
        private ServiceHost _o;

        public void Create(Uri fullUri, int id)
        {
            if (_started) return;

            _started = true;
            _l = new ServiceHost(typeof(RDotNet.Server.Services.RLanguageService), new Uri(fullUri + "RLanguageService/" + id));
            _m = new ServiceHost(typeof(RDotNet.Server.Services.RManagementService), new Uri(fullUri + "RManagementService/" + id));
            _o = new ServiceHost(typeof(RDotNet.Server.Services.ROutputService), new Uri(fullUri + "ROutputService/" + id));

            _l.Open();
            _m.Open();
            _o.Open();

        }
        public void Dispose()
        {
            if (_l != null)
            {
                _l.Close();
                _l = null;
            }

            if (_m != null)
            {
                _m.Close();
                _m = null;
            }

            if (_o != null)
            {
                _o.Close();
                _o = null;
            }
        }
    }
}
