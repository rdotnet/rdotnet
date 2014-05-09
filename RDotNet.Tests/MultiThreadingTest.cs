using NUnit.Framework;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace RDotNet
{
   public class MultiThreadingTest : RDotNetTestFixture
   {
      [Test]
      public void TestNonConcurrentMultiThreading()
      {
         // This tests checks that calling R 
         // The key was to set R_CStackLimit to -1 in the engine initialization, but to do so towards the end, otherwise
         // at least on Windows, it is overriden by the rest of the initialization procedure.
         // While multiple issues are releated, see for instance issue 115.

         var engine = this.Engine;
         var r = new RuntimeDiagnostics(engine);
         int sizeEach = 20;
         int n = 100;
         int nThreads = 10;
         // with concurrent:true, very likely to bomb.
         doMultiThreadingOperation(r, sizeEach, n, nThreads, concurrent: false);
      }

      private void doMultiThreadingOperation(RuntimeDiagnostics r, int sizeEach, int n, int numOps, bool concurrent = false)
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
   }
}