using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;

namespace MeasureRuntime
{
    class Program
    {
        private static readonly MockDevice device = new MockDevice();

        private static void Main(string[] args)
        {
            Helper.SetEnvironmentVariables();
            var engineName = "R_engine";
            using (var engine = REngine.CreateInstance(engineName))
            {
                engine.Initialize(device: device);
                DoMeasures(engine);
                engine.Dispose();
            }
        }

        private static void DoMeasures(REngine engine)
        {
            RuntimeDiagnostics r = new RuntimeDiagnostics(engine);
            long dt;
#if DEBUG
            Console.WriteLine("NOTE Running code compiled in debug mode");
#endif
            for (int n = 10; n < 2e6; n = n*10)
            {
                dt = r.MeasureRuntime(RuntimeDiagnostics.CreateNumericVector, n);
                Console.WriteLine("numeric vector; n={0}, deltaT={1}", n, dt);
            }
        }
    }
}
