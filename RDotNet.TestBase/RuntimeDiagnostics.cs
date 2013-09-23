using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace RDotNet
{
    public class RuntimeDiagnostics
    {
        private REngine engine;
        public RuntimeDiagnostics(REngine engine)
        {
            this.engine = engine;
        }

        public long MeasureRuntime(Action<REngine, int, Stopwatch> fun, int n)
        {
            var s = new Stopwatch();
            fun(engine, n, s);
            return s.ElapsedMilliseconds;
        }

        public static void CreateNumericVector(REngine engine, int n, Stopwatch s)
        {
            double[] d = createDoubleArray(n);
            s.Start();
            var nvec = engine.CreateNumericVector(d);
            s.Stop();
        }

        private static double[] createDoubleArray(int n)
        {
            double[] a = new double[n];
            Random r = new Random(42);
            for (int i = 0; i < n; i++)
                a[i] = r.NextDouble();
            return a;
        }
    }
}
