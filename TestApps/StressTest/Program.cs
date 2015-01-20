using RDotNet;
using RDotNet.Devices;
using System;
using System.Threading.Tasks;

namespace StressTest
{
    internal class Program
    {
        private static readonly ICharacterDevice device = new ConsoleDevice();

        private static void Main(string[] args)
        {
            REngine.SetEnvironmentVariables();
            using (var engine = REngine.GetInstance(device: device))
            {
                engine.EnableLock = true;
                var r = new RuntimeDiagnostics(engine);
                int sizeEach = 20;
                int n = 1000;
                NumericVector[] nVecs = r.CreateNumericVectors(n, sizeEach);
                nVecs = r.Lapply(nVecs, "function(x) {x * 2}");

                doMultiThreadingOperation(r, sizeEach, n, 50, false);
                doTestObjectFinalization(r, sizeEach, n);
                doTestObjectFinalization(r, sizeEach, n, disposeSexp: true);
                nVecs = null;
                engine.Dispose(); // deliberate test
            }
        }

        private static void doMultiThreadingOperation(RuntimeDiagnostics r, int sizeEach, int n, int numOps, bool concurrent = false)
        {
            var pVecs = new NumericVector[numOps][];
            Parallel.For(0, numOps, i => CreateVector(r, sizeEach, n, pVecs, i, !concurrent));
        }

        private static Object lockedObj = new Object();

        private static void CreateVector(RuntimeDiagnostics r, int sizeEach, int n, NumericVector[][] pVecs, int i, bool doLock)
        {
            if (doLock)
                lock (lockedObj) { CreateVector(r, sizeEach, n, pVecs, i, false); }
            else
                pVecs[i] = r.CreateNumericVectors(n, sizeEach);
        }

        private static void doTestObjectFinalization(RuntimeDiagnostics r, int sizeEach, int n, bool disposeSexp = false)
        {
            NumericVector[] v = r.CreateNumericVectors(n, sizeEach);
            checkSizes(v, sizeEach);
            var v2 = r.Lapply(v, "function(x) {x * x}");
            checkSizes(v2, sizeEach);
            v = null;
            for (int i = 0; i < 5; i++)
            {
                GC.Collect();
                v = r.CreateNumericVectors(n, sizeEach);
                checkSizes(v, sizeEach);
                if (disposeSexp)
                    for (int j = 0; j < v.Length; j++)
                    {
                        v[j].Dispose();
                    }
            }
        }

        private static void checkSizes(NumericVector[] v, int sizeEach)
        {
            for (int i = 0; i < v.Length; i++)
            {
                if (v[i].Length != sizeEach)
                    throw new Exception(string.Format("Unexpected length {0} at index {1}", v[i].Length, i));
            }
        }
    }
}