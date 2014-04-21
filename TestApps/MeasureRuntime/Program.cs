﻿using System;
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
         uint maxPowTwo = args.Length > 1 ? uint.Parse(args[1]) : 24;

         using (var engine = REngine.GetInstance(device: device, parameter: parameter))
         {
            printMemLimit(engine);
            var measures = new List<Measurement>();

            //engine.Evaluate("cases <- expand.grid(c(2,5,10), 10^(0:6))");
            var sizes = engine.Evaluate(string.Format("as.integer(2^(0:{0}))", maxPowTwo)).AsInteger().ToArray();

            RuntimeDiagnostics r = new RuntimeDiagnostics(engine)
            {
               Types = new[] { "numeric", "integer", "logical" }, // characters are much slower / cannot include here
               Sizes = sizes
            };

            var funMap = RuntimeDiagnostics.GetMatrixCreationFunctions();
            measures.AddRange(r.DoMeasures(funMap, what: "matrix", operation: "create", tag: ""));

            funMap = RuntimeDiagnostics.GetVectorCreationFunctions();
            measures.AddRange(r.DoMeasures(funMap, what: "vector", operation: "create", tag: ""));

            funMap = RuntimeDiagnostics.GetMatrixRetrievalFunctions();
            measures.AddRange(r.DoMeasures(funMap, what: "matrix", operation: "read", tag: ""));

            funMap = RuntimeDiagnostics.GetVectorRetrievalFunctions();
            measures.AddRange(r.DoMeasures(funMap, what: "vector", operation: "read", tag: ""));

            engine.SetSymbol("runtimes", r.CollateResults(measures));
            engine.Evaluate("write.table(runtimes, 'c:/tmp/runtimes.csv', row.names=FALSE, col.names=TRUE, quote=FALSE, sep = ',')");

            engine.Dispose();
         }
      }

      private static void printMemLimit(REngine engine)
      {
         Console.WriteLine("************");
         Console.WriteLine("*** NOTE: memory.limit() returns: " + engine.Evaluate("memory.limit()").AsNumeric()[0]);
         Console.WriteLine("************");
      }

      private static void DoMeasures(RuntimeDiagnostics r, IDictionary<string, Action<REngine, int, Stopwatch>> funMap,
         string what, int[] sizes, string operation, string[] types, string tag)
      {
         var m = new List<Measurement>();

#if DEBUG
         Console.WriteLine("NOTE Running code compiled in debug mode");
#endif
         //Console.WriteLine();
         //Console.WriteLine("*** Faster matrices creation/initialisation ***");
         //Console.WriteLine();

         foreach (var type in types)
         {
            for (int i = 0; i < sizes.Length; i++)
               m.Add(MeasureRuntimeOperation(r, funMap[type], sizes[i], type, operation, what, tag));
         }

         const int tenMillions = 10000000;
         // Cannot go past 1 billion elements, 
         // R by default would spit the dummy, even running 64 bits, with 
         // 1: Reached total allocation of 2047Mb: see help(memory.size)
         for (int n = 1; n < 11; n++)
         {
            //engine.ForceGarbageCollection();
            MeasureRuntimeOperation(r, RuntimeDiagnostics.RtoDotNetNumericMatrix, n * tenMillions, "numeric", operation: "Read    ", what: what);
         }
         for (int n = 1; n < 11; n++)
         {
            //engine.ForceGarbageCollection();
            MeasureRuntimeOperation(r, RuntimeDiagnostics.RtoDotNetIntegerMatrix, n * tenMillions, "integer", operation: "Read    ", what: what);
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

         for (int n = 1; n < 2e7; n = n * 10)
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

         for (int n = 1; n < 2e7; n = n * 10)
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

      private static Measurement MeasureRuntimeOperation(RuntimeDiagnostics r, Action<REngine, int, Stopwatch> fun, int n, string type, string operation = "Create", string what = "vector", string tag = "")
      {
         var m = r.MeasureRuntimeOperation(fun, n, type, operation, what, tag);
         Console.WriteLine(r.PrintRuntimeOperation(m));
         return m;
      }
   }
}
