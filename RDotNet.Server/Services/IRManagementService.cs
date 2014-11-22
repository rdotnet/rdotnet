using System;
using System.ServiceModel;

namespace RDotNet.Server.Services
{
    [ServiceContract]
    public interface IRManagementService
    {
        [OperationContract]
        string GetRUser();

        [OperationContract]
        void Start(StartupParameter parameter);

        [OperationContract]
        void Terminate();

        [OperationContract]
        bool IsStarted();

        [OperationContract]
        bool IsAlive();

        [OperationContract]
        bool Poke();

        [OperationContract]
        TimeSpan LastPoke();

        [OperationContract]
        void ForceGarbageCollection();
    }
}
