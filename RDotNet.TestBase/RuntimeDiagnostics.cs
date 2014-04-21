using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;

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
         var dt = this.MeasureRuntime(fun, n);
         return string.Format("{0} {1} {4}; n={2:e01}, deltaT={3} ms", operation, type, n, dt, what);
      }

      public Measurement MeasureRuntimeOperation(Action<REngine, int, Stopwatch> fun, int n, string type, string operation = "Create", string what = "vector")
      {
         var dt = this.MeasureRuntime(fun, n);
         return new Measurement(){ Operation=operation, Type=type, N=n, Time=dt, What=what, Tag=""};
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
         RtoDotNetNumericVector(engine, n, s, e => e.GetSymbol("x").AsNumeric().ToArray());
      }

      public static void RtoDotNetIntegerVector(REngine engine, int n, Stopwatch s)
      {
         RtoDotNetIntegerVector(engine, n, s, e => e.Evaluate("x").AsInteger().ToArray());
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
         RtoDotNetNumericMatrix(engine, n, s, e => e.GetSymbol("x").AsNumericMatrix().ToArray());
      }

      public static void RtoDotNetIntegerMatrix(REngine engine, int n, Stopwatch s)
      {
         RtoDotNetNumericMatrix(engine, n, s, e => e.GetSymbol("x").AsIntegerMatrix().ToArray());
      }

      public static void RtoDotNetIntegerVector(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun)
      {
         var vStatement = string.Format("x <- as.integer(1:{0})", n.ToString(CultureInfo.InvariantCulture));
         engine.Evaluate(vStatement);
         s.Start();
         var nvec = fun(engine);
         s.Stop();
      }

      public static void RtoDotNetIntegerMatrix(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun)
      {
         var m = (int)Math.Floor(Math.Sqrt(n));
         var vStatement = string.Format("matrix(as.integer(1:({0}*{0})), nrow={0}, ncol={0})", m.ToString(CultureInfo.InvariantCulture));
         engine.SetSymbol("x", engine.Evaluate(vStatement));
         //engine.Evaluate("cat(ls())");
         s.Start();
         var nvec = fun(engine);
         s.Stop();
      }

      public static void RtoDotNetNumericMatrix(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun)
      {
         var m = (int)Math.Floor(Math.Sqrt(n));
         var vStatement = string.Format("matrix(as.numeric(1:({0}*{0})), nrow={0}, ncol={0})", m.ToString(CultureInfo.InvariantCulture));
         engine.SetSymbol("x", engine.Evaluate(vStatement));
         //engine.Evaluate("cat(ls())");
         s.Start();
         var nvec = fun(engine);
         s.Stop();
      }

      public static void RtoDotNetNumericVector(REngine engine, int n, Stopwatch s, Func<REngine, Array> fun)
      {
         var vStatement = string.Format("as.numeric(1:{0})", n.ToString(CultureInfo.InvariantCulture));
         engine.SetSymbol("x", engine.Evaluate(vStatement));
         //engine.Evaluate("cat(ls())");
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
         int dim = (int)Math.Floor(Math.Sqrt(n));
         var a = new double[dim, dim];
         Random r = new Random(42);
         for (int i = 0; i < dim; i++)
            for (int j = 0; j < dim; j++)
               a[i, j] = r.NextDouble();
         return a;
      }

      private static int[,] createIntegerMatrix(int n, int max = 256)
      {
         int dim = (int)Math.Floor(Math.Sqrt(n));
         var a = new int[dim, dim];
         Random r = new Random(42);
         for (int i = 0; i < dim; i++)
            for (int j = 0; j < dim; j++)
               a[i, j] = r.Next(max);
         return a;
      }

      private static bool[,] createLogicalMatrix(int n, int max = 256)
      {
         int dim = (int)Math.Floor(Math.Sqrt(n));
         var a = new bool[dim, dim];
         Random r = new Random(42);
         for (int i = 0; i < dim; i++)
            for (int j = 0; j < dim; j++)
               a[i, j] = (r.Next(max)<128);
         return a;
      }
      
   }

   public class Measurement
   {
      public string Operation;

      public string Type;

      public int N;

      public double Time;

      public string What;
      public string Tag;
   }
}
