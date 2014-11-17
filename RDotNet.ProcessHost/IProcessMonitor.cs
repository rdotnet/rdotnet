using System.ServiceModel;

namespace RDotNet.ProcessHost
{
    [ServiceContract]
    public interface IProcessMonitor
    {
        int Id
        {
            [OperationContract]
            get;
        }

        [OperationContract]
        bool IsAlive();

        ServiceState State
        {
            [OperationContract]
            get;
        }

        [OperationContract]
        void Kill();
    }
}
