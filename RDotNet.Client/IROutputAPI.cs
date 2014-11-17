using System.Collections.Generic;

namespace RDotNet.Client
{
    public interface IROutputAPI
    {
        void ResetConsole();
        void ResetPlots();
        List<string> GetPlots();
        List<string> GetConsole();
    }
}
