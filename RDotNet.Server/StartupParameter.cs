using System.Runtime.Serialization;

namespace RDotNet.Server
{
    [DataContract]
    public class StartupParameter
    {
        public StartupParameter()
        {
            Quiet = true;
            Slave = false;
            Interactive = true;
            Verbose = false;
            LoadSiteFile = true;
            LoadInitFile = true;
            DebugInitFile = false;
            NoRenviron = false;
        }

        [DataMember]
        public bool Quiet { get; set; }

        [DataMember]
        public bool Slave { get; set; }

        [DataMember]
        public bool Interactive { get; set; }

        [DataMember]
        public bool Verbose { get; set; }

        [DataMember]
        public bool LoadSiteFile { get; set; }

        [DataMember]
        public bool LoadInitFile { get; set; }

        [DataMember]
        public bool DebugInitFile { get; set; }

        [DataMember]
        public bool NoRenviron { get; set; }

        [DataMember]
        public string RHome { get; set; }

        [DataMember]
        public string Home { get; set; }

        public static StartupParameter Default = new StartupParameter();
    }
}
