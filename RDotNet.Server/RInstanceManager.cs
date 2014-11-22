using RDotNet.R.Adapter;

namespace RDotNet.Server
{
    public class RInstanceManager : IRInstanceManager
    {
        private static readonly RInstance Instance = new RInstance();

        public RInstance GetInstance()
        {
            return Instance;
        }
    }

    public interface IRInstanceManager
    {
        RInstance GetInstance();
    }
}
