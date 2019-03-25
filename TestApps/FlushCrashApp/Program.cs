using RDotNet;

namespace FlushCrashApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = REngine.GetInstance();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            engine.Evaluate("flush.console()");
        }
    }
}
