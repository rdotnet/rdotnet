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
         var engine = this.Engine;
         RuntimeDiagnostics r = new RuntimeDiagnostics(engine);
         int n = (int)1e6;
         var dt = r.MeasureRuntime(RuntimeDiagnostics.CreateNumericVector, n);
         Assert.LessOrEqual(dt, 100, "Creation of a 1 million element numeric vector is less than a tenth of a second");
      }
   }
}
