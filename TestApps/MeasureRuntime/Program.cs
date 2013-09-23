using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RDotNet;
using RDotNet.Devices;

namespace MeasureRuntime
{
   class Program
   {
      private static readonly ICharacterDevice device = new ConsoleDevice();

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

         Console.WriteLine();
         Console.WriteLine("*** Faster vector creation/initialisation ***");
         Console.WriteLine();

         for (int n = 1; n < 2e7; n = n*10)
         {
            MeasureRuntimeCreateVector(r, RuntimeDiagnostics.CreateNumericVector, n, "numeric");
            MeasureRuntimeCreateVector(r, RuntimeDiagnostics.CreateIntegerVector, n, "integer");
            MeasureRuntimeCreateVector(r, RuntimeDiagnostics.CreateLogicalVector, n, "logical");
         }
         //for (int n = 1; n < 2e6; n = n * 10)
         //   MeasureRuntimeCreateVector(r, RuntimeDiagnostics.CreateCharacterVector, n, "character");

         Console.WriteLine();
         Console.WriteLine("*** Read R vector data into .NET Arrays: default and fast operation ***");
         Console.WriteLine();

         for (int n = 1; n < 2e7; n = n*10)
         {
            MeasureRuntimeCreateVector(r, RuntimeDiagnostics.RtoDotNetNumericVector, n, "numeric", operation: "Read    ");
            MeasureRuntimeCreateVector(r, RuntimeDiagnostics.RtoDotNetNumericVectorFast, n, "numeric",
                                       operation: "ReadFast");
         }
         for (int n = 1; n < 2e7; n = n * 10)
         {
            MeasureRuntimeCreateVector(r, RuntimeDiagnostics.RtoDotNetIntegerVector, n, "integer", operation: "Read    ");
            MeasureRuntimeCreateVector(r, RuntimeDiagnostics.RtoDotNetIntegerVectorFast, n, "integer", operation: "ReadFast");
         }

         Console.WriteLine();
         Console.WriteLine("*** Now keep going only with faster version of read operations ***");
         Console.WriteLine();

         const int tenMillions = 10000000;
         // Cannot go past 1 billion elements, 
         // R by default would spit the dummy, even running 64 bits, with 
         // 1: Reached total allocation of 2047Mb: see help(memory.size)
         for (int n = 1; n < 11; n++)
            MeasureRuntimeCreateVector(r, RuntimeDiagnostics.RtoDotNetNumericVectorFast, n * tenMillions, "numeric", operation: "ReadFast");
         for (int n = 1; n < 11; n++)
            MeasureRuntimeCreateVector(r, RuntimeDiagnostics.RtoDotNetIntegerVectorFast, n * tenMillions, "integer", operation: "ReadFast");
      }

      private static void MeasureRuntimeCreateVector(RuntimeDiagnostics r, Action<REngine, int, Stopwatch> fun, int n, string type, string operation="Create")
      {
         var dt = r.MeasureRuntime(fun, n);
         Console.WriteLine("{0} {1} vector; n={2:e01}, deltaT={3} ms", operation, type, n, dt);
      }
   }
}
