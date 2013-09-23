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
         var engine = REngine.GetInstanceFromID(EngineName);
         RuntimeDiagnostics r = new RuntimeDiagnostics(engine);
         int n = (int)1e6;
         var dt = r.MeasureRuntime(RuntimeDiagnostics.CreateNumericVector, n);
         Assert.LessOrEqual(dt, 100, "Creation of a 1 million element numeric vector is less than a tenth of a second");
      }

      // These tests should more else where where more is tested on numerical accuracy.
      [Test]
      public void TestCreateNumericVectorValid()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("x <- 1:100 * 1.1");
         var expected = ArrayMult(GenArrayDouble(1, 100), 1.1);
         var a = engine.GetSymbol("x").AsNumeric().ToArrayFast();
         CheckArrayEqual(a, expected);
         a = engine.GetSymbol("x").AsNumeric().ToArray();
         CheckArrayEqual(a, expected);

         // Test a large data set: I just cannot believe how faster things are...
         engine.Evaluate("x <- 1:1e7 * 1.1");
         a = engine.GetSymbol("x").AsNumeric().ToArrayFast();
         Assert.AreEqual(a[10000000/2-1], 1.1*1e7/2);
      }

      [Test]
      public void TestCreateIntegerVectorValid()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("x <- 1:100");
         var expected = GenArrayInteger(1, 100);
         var a = engine.GetSymbol("x").AsInteger().ToArrayFast();
         CheckArrayEqual(a, expected);
         a = engine.GetSymbol("x").AsInteger().ToArray();
         CheckArrayEqual(a, expected);
      }
   }
}
