using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace RDotNet
{
    public class PerformanceTest : RDotNetTestFixture
    {
        [Test]
        public void TestCreateNumericVector()
        {
            double[] d = createDoubleArray(1000000);
            var engine = REngine.GetInstanceFromID(EngineName);
            var s = new Stopwatch();
            s.Start();
            var nvec = engine.CreateNumericVector(d);
            s.Stop();
            var dt = s.ElapsedMilliseconds;
        }

        private double[] createDoubleArray(int n)
        {
            double[] a = new double[n];
            Random r = new Random(42);
            for (int i = 0; i < n; i++)
                a[i] = r.NextDouble();
            return a;
        }
    }
}
