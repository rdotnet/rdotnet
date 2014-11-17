using System.Collections.Generic;
using System.ServiceModel;

namespace RDotNet.Server.Services
{
    [ServiceContract]
    public interface IROutputService
    {
        [OperationContract]
        void ClearAll();

        [OperationContract]
        void ClearPlots();

        [OperationContract]
        void ClearText();

        [OperationContract]
        int GetPendingPlotCount();

        [OperationContract]
        IEnumerable<string> GetAllPlots();

        [OperationContract]
        IEnumerable<string> GetText();
    }
}
