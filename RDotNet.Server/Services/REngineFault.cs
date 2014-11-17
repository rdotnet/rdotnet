using System.Runtime.Serialization;

namespace RDotNet.Server.Services
{
    [DataContract]
    public class REngineFault
    {
        [DataMember]
        public string Message { get; set; }
    }
}
