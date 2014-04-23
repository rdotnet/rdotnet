using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections;

namespace RDotNet
{
   /// <summary>
   /// Supports runtime performance diagnosis and measurements
   /// </summary>
   public class RuntimeDiagnostics
   {
      private REngine engine;
      public RuntimeDiagnostics(REngine engine)
      {
         this.engine = engine;
         Types = new string[0];
         Sizes = new int[0];
         What = "";
         Operation = "";
         Tag = "";

      }

      public Measurement[] DoMeasures(IDictionary<string, Action<REngine, int, Stopwatch>> funMap,
         string[] types = null, int[] sizes = null, string what = null, string operation = null, string tag = null, bool printToConsole=true)
      {
         if (types == null) types = this.Types;
         if (sizes == null) sizes = this.Sizes;
         if (what == null) what = this.What;
         if (operation == null) operation = this.Operation;
         if (tag == null) tag = this.Tag;
         var measures = new List<Measurement>();
         foreach (string type in types)
         {
            for (int i = 0; i < sizes.Length; i++)
            {
               var m = MeasureRuntimeOperation(funMap[type], sizes[i], type, operation, what, tag);
               measures.Add(m);
               if(printToConsole)
                  Console.WriteLine(PrintRuntimeOperation(m));
            }
         }
         return measures.ToArray();
      }

      public double MeasureRuntime(Action<REngine, int, Stopwatch> fun, int n)
      {
         var s = new Stopwatch();
         fun(engine, n, s);
         var ticks = s.ElapsedTicks;
         long ticksPerSec = Stopwatch.Frequency;
         return (ticks * 1000.0) / ticksPerSec; // milliseconds
      }

      public string PrintRuntimeOperation(Action<REngine, int, Stopwatch> fun, int n, string type, string operation = "Create", string what = "vector")
      {
         var m = MeasureRuntimeOperation(fun, n, type, operation, what);
         return PrintRuntimeOperation(m);
      }

      public DataFrame CollateResults(IEnumerable<Measurement> measures)
      {
         var size = measures.Select(x => x.N).ToArray();
         var ops = measures.Select(x => x.Operation).ToArray();
         var tags = measures.Select(x => x.Tag).ToArray();
         var time = measures.Select(x => x.Duration).ToArray();
         var type = measures.Select(x => x.Type).ToArray();
         var what = measures.Select(x => x.What).ToArray();
         return this.engine.CreateDataFrame(new IEnumerable[] { size, ops, tags, time, type, what }, new[]{"Size","Operation","Tag","Duration","Type","What"} );
      }

      public string PrintRuntimeOperation(Measurement m)
      {
         return string.Format("{0} {1} {4}; n={2:e01}, deltaT={3} ms", m.Operation, m.Type, m.N, m.Duration, m.What);
      }

      public Measurement MeasureRuntimeOperation(Action<REngine, int, Stopwatch> fun, int n, string type, string operation = "Create", string what = "vector", string tag="")
      {
         var dt = this.MeasureRuntime(fun, n);
         return new Measurement(){ Operation=operation, Type=type, N=n, Duration=dt, What=what, Tag=tag};
      }

      public static void CreateNumericMatrix(REngine engine, int n, Stopwatch s)
      {
         double[,] d = createDoubleMatrix(n);
         s.Start();
         var nvec = engine.CreateNumericMatrix(d);
         s.Stop();
      }

      public static void CreateIntegerMatrix(REngine engine, int n, Stopwatch s)
      {
         int[,] d = createIntegerMatrix(n);
         s.Start();
         var nvec = engine.CreateIntegerMatrix(d);
         s.Stop();
      }

      public static void CreateCharacterMatrix(REngine engine, int n, Stopwatch s)
      {
         string[,] d = createStringMatrix(n);
         s.Start();
         var nvec = engine.CreateCharacterMatrix(d);
         s.Stop();
      }

      public static void CreateLogicalMatrix(REngine engine, int n, Stopwatch s)
      {
         bool[,] d = createLogicalMatrix(n);
         s.Start();
         var nvec = engine.CreateLogicalMatrix(d);
         s.Stop();
      }

      public static void CreateNumericVector(REngine engine, int n, Stopwatch s)
      {
         double[] d = createDoubleArray(n);
         s.Start();
         var nvec = engine.CreateNumericVector(d);
         s.Stop();
      }

      public static void CreateLogicalVector(REngine engine, int n, Stopwatch s)
      {
         int[] d = createIntegerArray(n, 256);
         var b = Array.ConvertAll(d, v => v < 128);
         s.Start();
         var nvec = engine.CreateLogicalVector(b);
         s.Stop();
      }

      public static void CreateIntegerVector(REngine engine, int n, Stopwatch s)
      {
         int[] d = createIntegerArray(n);
         s.Start();
         var nvec = engine.CreateIntegerVector(d);
         s.Stop();
      }

      public static void CreateCharacterVector(REngine engine, int n, Stopwatch s)
      {
         string[] d = createStringArray(n);
         s.Start();
         var nvec = engine.CreateCharacterVector(d);
         s.Stop();
      }

      public static void RtoDotNetNumericVector(REngine engine, int n, Stopwatch s)
      {
         RtoDotNetNumericVector(engine, n, s, e => e.GetSymbol("x").AsNumeric().ToArray(), "x");
      }

      public static void RtoDotNetIntegerVector(REngine engine, int n, Stopwatch s)
      {
         RtoDotNetIntegerVector(engine, n, s, e => e.GetSymbol("x").AsInteger().ToArray(), "x");
      }

      public static void RtoDotNetLogicalVector(REngine engine, int n, Stopwatch s)
      {
         RtoDotNetLogicalVector(engine, n, s, e => e.GetSymbol("x").AsLogical().ToArray(), "x");
      }

      public static void RtoDotNetCharacterVector(REngine engine, int n, Stopwatch s)
      {
         var vStatement = string.Format("x <- rep('abcd',{0})", n.ToString(CultureInfo.InvariantCulture));
         engine.Evaluate(vStatement);
         s.Start();
         var nvec = engine.Evaluate("x").AsCharacter().ToArray();
         s.Stop();
      }

      public static void RtoDotNetNumericMatrix(REngine engine, int n, Stopwatch s)
      {
         RtoDotNetNumericMatrix(engine, n, s, e => e.GetSymbol("x").AsNumericMatrix().ToArray(), "x");
      }

      public static void RtoDotNetIntegerMatrix(REngine engine, int n, Stopwatch s)
      {
         RtoDotNetNumericMatrix(engine, n, s, e => e.GetSymbol("x").AsIntegerMatrix().ToArray(), "x");
      }

      public static void RtoDotNetLogicalMatrix(REngine engine, int n, Stopwatch s)
      {
         RtoDotNetLogicalMatrix(engine, n, s, e => e.GetSymbol("x").AsLogicalMatrix().ToArray(), "x");
      }

      public static void RtoDotNetCharacterMatrix(REngine engine, int n, Stopwatch s)
      {
         var m = floorSqrt(n);
         var vStatement = string.Format("matrix(rep('abcd',{0}*{0}), nrow={0}, ncol={0})", m.ToString(CultureInfo.InvariantCulture));
         setValueAndMeasure(engine, s, e => e.GetSymbol("x").AsCharacterMatrix().ToArray(), "x", vStatement);
      }

      public static void RtoDotNetNumericVector(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun, string symbolName)
      {
         var mode = "numeric";
         measureRtoDotnetVector(engine, n, s, fun, mode, symbolName);
      }

      public static void RtoDotNetLogicalVector(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun, string symbolName)
      {
         var mode = "logical";
         measureRtoDotnetVector(engine, n, s, fun, mode, symbolName);
      }

      public static void RtoDotNetIntegerVector(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun, string symbolName)
      {
         var mode = "integer";
         measureRtoDotnetVector(engine, n, s, fun, mode, symbolName);
      }

      public static void RtoDotNetIntegerMatrix(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun, string symbolName)
      {
         var mode = "integer";
         measureRtoDotnetMatrix(engine, n, s, fun, mode, symbolName);
      }

      public static void RtoDotNetNumericMatrix(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun, string symbolName)
      {
         var mode = "numeric";
         measureRtoDotnetMatrix(engine, n, s, fun, mode, symbolName);
      }

      public static void RtoDotNetLogicalMatrix(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun, string symbolName)
      {
         var mode = "logical";
         measureRtoDotnetMatrix(engine, n, s, fun, mode, symbolName);
      }

      

      private static void measureRtoDotnetVector(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun, string mode, string symbolName)
      {
         var vStatement = string.Format("as.{1}(1:{0})", n.ToString(CultureInfo.InvariantCulture), mode);
         setValueAndMeasure(engine, s, fun, symbolName, vStatement);
      }

      private static void measureRtoDotnetMatrix(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun, string mode, string symbolName)
      {
         var m = floorSqrt(n);
         var vStatement = string.Format("matrix(as.{1}(1:({0}*{0})), nrow={0}, ncol={0})", m.ToString(CultureInfo.InvariantCulture), mode);
         setValueAndMeasure(engine, s, fun, symbolName, vStatement);
      }

      private static void setValueAndMeasure(REngine engine, Stopwatch s, Func<REngine, Array> fun, string symbolName, string vStatement)
      {
         engine.SetSymbol(symbolName, engine.Evaluate(vStatement));
         //engine.Evaluate("cat(ls())");
         measure(engine, s, fun);
      }

      private static void measure(REngine engine, Stopwatch s, Func<REngine, Array> fun)
      {
         s.Start();
         var nvec = fun(engine);
         s.Stop();
      }



      // Not much difference for character as yet, the way they are done.

      private static string[] createStringArray(int n, int strLen = 5)
      {
         StringBuilder s = new StringBuilder();
         Random r = new Random(42);
         for (int i = 0; i < strLen; i++)
            s.Append(Convert.ToChar(r.Next(256)));
         var res = new string[n];
         var str = s.ToString();
         for (int i = 0; i < n; i++)
            res[i] = str;
         return res;
      }

      private static int[] createIntegerArray(int n, int max=10000)
      {
         int[] a = new int[n];
         Random r = new Random(42);
         for (int i = 0; i < n; i++)
            a[i] = r.Next(max);
         return a;
      }

      private static double[] createDoubleArray(int n)
      {
         double[] a = new double[n];
         Random r = new Random(42);
         for (int i = 0; i < n; i++)
            a[i] = r.NextDouble();
         return a;
      }

      private static double[,] createDoubleMatrix(int n)
      {
         int dim = floorSqrt(n);;
         var a = new double[dim, dim];
         Random r = new Random(42);
         for (int i = 0; i < dim; i++)
            for (int j = 0; j < dim; j++)
               a[i, j] = r.NextDouble();
         return a;
      }

      private static string[,] createStringMatrix(int n, int max = 256)
      {
         int dim = floorSqrt(n);
         var a = new string[dim, dim];
         Random r = new Random(42);
         for (int i = 0; i < dim; i++)
            for (int j = 0; j < dim; j++)
               a[i, j] = r.Next(max).ToString();
         return a;
      }

      private static int floorSqrt(int n)
      {
         int dim = (int)Math.Floor(Math.Sqrt(n));
         return dim;
      }

      private static int[,] createIntegerMatrix(int n, int max = 256)
      {
         int dim = floorSqrt(n);;
         var a = new int[dim, dim];
         Random r = new Random(42);
         for (int i = 0; i < dim; i++)
            for (int j = 0; j < dim; j++)
               a[i, j] = r.Next(max);
         return a;
      }

      private static bool[,] createLogicalMatrix(int n, int max = 256)
      {
         int dim = floorSqrt(n);;
         var a = new bool[dim, dim];
         Random r = new Random(42);
         for (int i = 0; i < dim; i++)
            for (int j = 0; j < dim; j++)
               a[i, j] = (r.Next(max)<128);
         return a;
      }


      public string[] Types { get; set; }
      public int[] Sizes { get; set; }
      public string What { get; set; }
      public string Operation { get; set; }
      public string Tag { get; set; }

      public static IDictionary<string, Action<REngine, int, Stopwatch>> GetMatrixCreationFunctions()
      {
         var m = new Dictionary<string, Action<REngine, int, Stopwatch>>();
         m["numeric"] = RuntimeDiagnostics.CreateNumericMatrix;
         m["integer"] = RuntimeDiagnostics.CreateIntegerMatrix;
         m["logical"] = RuntimeDiagnostics.CreateLogicalMatrix;
         m["character"] = RuntimeDiagnostics.CreateCharacterMatrix;
         return m;
      }

      public static IDictionary<string, Action<REngine, int, Stopwatch>> GetVectorCreationFunctions()
      {
         var m = new Dictionary<string, Action<REngine, int, Stopwatch>>();
         m["numeric"] = RuntimeDiagnostics.CreateNumericVector;
         m["integer"] = RuntimeDiagnostics.CreateIntegerVector;
         m["logical"] = RuntimeDiagnostics.CreateLogicalVector;
         m["character"] = RuntimeDiagnostics.CreateCharacterVector;
         return m;
      }

      public static IDictionary<string, Action<REngine, int, Stopwatch>> GetMatrixRetrievalFunctions()
      {
         var m = new Dictionary<string, Action<REngine, int, Stopwatch>>();
         m["numeric"] = RuntimeDiagnostics.RtoDotNetNumericMatrix;
         m["integer"] = RuntimeDiagnostics.RtoDotNetIntegerMatrix;
         m["logical"] = RuntimeDiagnostics.RtoDotNetLogicalMatrix;
         m["character"] = RuntimeDiagnostics.RtoDotNetCharacterMatrix;
         return m;
      }

      public static IDictionary<string, Action<REngine, int, Stopwatch>> GetVectorRetrievalFunctions()
      {
         var m = new Dictionary<string, Action<REngine, int, Stopwatch>>();
         m["numeric"] = RuntimeDiagnostics.RtoDotNetNumericVector;
         m["integer"] = RuntimeDiagnostics.RtoDotNetIntegerVector;
         m["logical"] = RuntimeDiagnostics.RtoDotNetLogicalVector;
         m["character"] = RuntimeDiagnostics.RtoDotNetCharacterVector;
         return m;
      }
   }

   public class Measurement
   {
      public string Operation;

      public string Type;

      public int N;

      public double Duration;

      public string What;
      public string Tag;
   }
}
