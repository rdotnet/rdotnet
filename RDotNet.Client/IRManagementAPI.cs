using RDotNet.Server;

namespace RDotNet.Client
{
    public interface IRManagementAPI
    {
        void Start(StartupParameter startupParameter);
        void Restart();
        void Stop();
        void ForceGarbageCollection();
        void Poke();
        bool IsAlive();
    }
}
