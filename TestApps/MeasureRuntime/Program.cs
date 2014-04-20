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
         REngine.SetEnvironmentVariables();
         StartupParameter parameter = args.Length > 0 ?
            new StartupParameter() { MaxMemorySize = ulong.Parse(args[0]) * (1024 * 1024) } :
            new StartupParameter() { };

         using (var engine = REngine.GetInstance(device:device, parameter:parameter))
         {
            //DoMeasuresVectors(engine);
            DoMeasuresMatrices(engine);
            engine.Dispose();
         }
      }

      private static void DoMeasuresMatrices(REngine engine)
      {
         RuntimeDiagnostics r = new RuntimeDiagnostics(engine);
#if DEBUG
         Console.WriteLine("NOTE Running code compiled in debug mode");
#endif
         Console.WriteLine();
         Console.WriteLine("*** Faster matrices creation/initialisation ***");
         Console.WriteLine();

         for (int n = 1; n < 2e7; n = n * 10)
         {
            MeasureRuntimeOperation(r, RuntimeDiagnostics.CreateNumericMatrix, n, "numeric", "Create", "matrix");
            MeasureRuntimeOperation(r, RuntimeDiagnostics.CreateIntegerMatrix, n, "integer", "Create", "matrix");
            MeasureRuntimeOperation(r, RuntimeDiagnostics.CreateLogicalMatrix, n, "logical", "Create", "matrix");
         }

         Console.WriteLine();
         Console.WriteLine("*** Now keep going only with faster version of read operations ***");
         Console.WriteLine();

         const int tenMillions = 10000000;
         // Cannot go past 1 billion elements, 
         // R by default would spit the dummy, even running 64 bits, with 
         // 1: Reached total allocation of 2047Mb: see help(memory.size)
         for (int n = 1; n < 11; n++)
         {
            engine.ForceGarbageCollection();
            MeasureRuntimeOperation(r, RuntimeDiagnostics.RtoDotNetNumericMatrix, n * tenMillions, "numeric", operation: "Read    ", what: "matrix");
         }
         for (int n = 1; n < 11; n++)
         {
            engine.ForceGarbageCollection();
            MeasureRuntimeOperation(r, RuntimeDiagnostics.RtoDotNetIntegerMatrix, n * tenMillions, "integer", operation: "Read    ", what: "matrix");
         }
      }

      private static void DoMeasuresVectors(REngine engine)
      {
         RuntimeDiagnostics r = new RuntimeDiagnostics(engine);
#if DEBUG
         Console.WriteLine("NOTE Running code compiled in debug mode");
#endif

         Console.WriteLine();
         Console.WriteLine("*** Faster vector creation/initialisation ***");
         Console.WriteLine();

         for (int n = 1; n < 2e7; n = n*10)
         {
            MeasureRuntimeOperation(r, RuntimeDiagnostics.CreateNumericVector, n, "numeric");
            MeasureRuntimeOperation(r, RuntimeDiagnostics.CreateIntegerVector, n, "integer");
            MeasureRuntimeOperation(r, RuntimeDiagnostics.CreateLogicalVector, n, "logical");
         }
         //for (int n = 1; n < 2e6; n = n * 10)
         //   MeasureRuntimeCreateVector(r, RuntimeDiagnostics.CreateCharacterVector, n, "character");

         Console.WriteLine();
         Console.WriteLine("*** Read R vector data into .NET Arrays: default and fast operation ***");
         Console.WriteLine();

         for (int n = 1; n < 2e7; n = n*10)
         {
            MeasureRuntimeOperation(r, RuntimeDiagnostics.RtoDotNetNumericVector, n, "numeric", operation: "Read    ");
         }
         for (int n = 1; n < 2e7; n = n * 10)
         {
            MeasureRuntimeOperation(r, RuntimeDiagnostics.RtoDotNetIntegerVector, n, "integer", operation: "Read    ");
         }

         const int tenMillions = 10000000;
         // Cannot go past 1 billion elements, 
         // R by default would spit the dummy, even running 64 bits, with 
         // 1: Reached total allocation of 2047Mb: see help(memory.size)
         for (int n = 1; n < 11; n++)
            MeasureRuntimeOperation(r, RuntimeDiagnostics.RtoDotNetNumericVector, n * tenMillions, "numeric", operation: "Read    ");
         for (int n = 1; n < 11; n++)
            MeasureRuntimeOperation(r, RuntimeDiagnostics.RtoDotNetIntegerVector, n * tenMillions, "integer", operation: "Read    ");
      }

      private static void MeasureRuntimeOperation(RuntimeDiagnostics r, Action<REngine, int, Stopwatch> fun, int n, string type, string operation = "Create", string what = "vector")
      {
         Console.WriteLine(r.PrintRuntimeOperation(fun, n, type, operation, what));
      }
   }
}
