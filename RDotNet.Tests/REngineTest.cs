using NUnit.Framework;
using System;
using System.Linq;

namespace RDotNet
{
   internal class REngineTest : RDotNetTestFixture
   {
      [Test]
      public void TestSetCommandLineArguments()
      {
         var engine = this.Engine;
         engine.SetCommandLineArguments(new[] { "Hello", "World" });
         Assert.That(engine.Evaluate("commandArgs()").AsCharacter(), Is.EquivalentTo(new[] { REngine.EngineName, "Hello", "World" }));
      }

      [Test]
      public void TestDefaultCommandLineArgs()
      {
         var engine = this.Engine;
         var cmdArgs = engine.Evaluate("commandArgs()").AsCharacter();
      }

      [Test]
      public void TestGlobalEnvironment()
      {
         var engine = this.Engine;
         Assert.That(engine.GlobalEnvironment.DangerousGetHandle(), Is.EqualTo(engine.Evaluate(".GlobalEnv").DangerousGetHandle()));
      }

      [Test]
      public void TestBaseNamespace()
      {
         var engine = this.Engine;
         Assert.That(engine.BaseNamespace.DangerousGetHandle(), Is.EqualTo(engine.Evaluate(".BaseNamespaceEnv").DangerousGetHandle()));
      }

      [Test]
      public void TestNilValue()
      {
         var engine = this.Engine;
         Assert.That(engine.NilValue.DangerousGetHandle(), Is.EqualTo(engine.Evaluate("NULL").DangerousGetHandle()));
      }

      [Test]
      public void TestRGarbageCollectNumericVectors()
      {
         /*
         gc()
         gc()
         memory.size()
         ## [1] 24.7
         x <- numeric(5e6)
         object.size(x)
         40000040 bytes
         memory.size()
         ## [1] 64.11
         gc()
         memory.size()
         ## [1] 62.89
         rm(x)
         gc()
         memory.size()
         ## [1] 24.7
         */
         var statementCreateX = "x <- numeric(5e6)";
         // For some reasons the delta is not 40MB spot on. Use 35 MB as a threshold
         double expectedMinMegaBytesDifference = 35.0;

         Func<SymbolicExpression, NumericVector> coercionFun = SymbolicExpressionExtension.AsNumeric;
         CheckProperMemoryReclaimR(statementCreateX, expectedMinMegaBytesDifference, coercionFun);
      }

      [Test]
      public void TestRGarbageCollectCharacterVectors()
      {
         /*
         rm(list=ls())
         gc()
         gc()
         memory.size()
         ## [1] 10.86 from scratch, [1] 18.62 subsequent
         #x <- rep('abcd', 1e6)
         ltrs <- letters[1:26] ; x <- expand.grid(ltrs,ltrs,ltrs,ltrs,ltrs[1:5]) ; rm(ltrs) ; x <- rep(paste0(x[,1], x[,2], x[,3], x[,4], x[,5]), 1)
         object.size(x)
         ## 82255704  bytes
         memory.size()
         ## [1] 235.95
         gc()
         gc()
         memory.size()
         ## [1] 97.11
         rm(x)
         gc()
         gc()
         memory.size()
         ## [1] 18.62
         */
         var statementCreateX = "ltrs <- letters[1:26] ; x <- expand.grid(ltrs,ltrs,ltrs,ltrs,ltrs[1:5]) ; rm(ltrs) ; x <- rep(paste0(x[,1], x[,2], x[,3], x[,4], x[,5]), 1)";
         // For some reasons the delta is not 40MB spot on. Use 35 MB as a threshold
         double expectedMinMegaBytesDifference = 80.0;

         Func<SymbolicExpression, CharacterVector> coercionFun = SymbolicExpressionExtension.AsCharacter;
         CheckProperMemoryReclaimR(statementCreateX, expectedMinMegaBytesDifference, coercionFun);
      }

      private void CheckProperMemoryReclaimR<T>(string statementCreateX, double expectedMinMegaBytesDifference, Func<SymbolicExpression, T> coercionFun) where T : SymbolicExpression
      {
         var engine = this.Engine;
         engine.Evaluate("if (exists('x')) {rm(x)}");
         var memoryInitial = GetBaselineRengineMemory(engine);
         engine.Evaluate(statementCreateX);
         T sexp = coercionFun(engine.GetSymbol("x"));
         var memoryAfterAlloc = GetBaselineRengineMemory(engine);
         Assert.That(memoryAfterAlloc - memoryInitial, Is.GreaterThan(expectedMinMegaBytesDifference));
         engine.Evaluate("rm(x)");
         sexp = null;
         var memoryAfterGC = GetBaselineRengineMemory(engine);
         Assert.That(memoryAfterAlloc - memoryAfterGC, Is.GreaterThan(expectedMinMegaBytesDifference));  // x should be collected.
      }

      private static double GetBaselineDotnetMemory(REngine engine)
      {
         GarbageCollectRandClr(engine);
         return GC.GetTotalMemory(false);
      }

      private static double GetBaselineRengineMemory(REngine engine)
      {
         GarbageCollectRandClr(engine);
         return GetRMemorySize(engine);
      }

      [Test]
      public void TestCharacterVectorToStringMemReclaim()
      {
         var statementCreateX = "x <- format(1:1000000)";
         double expectedMinBytesDifference = 2.5e7;  // (20 bytes + 2*6) * 1e6 = 3.2e7 bytes ~ 32 MB
         
         var engine = this.Engine;
         var memoryInitial = GetBaselineDotnetMemory(engine);
         engine.Evaluate(statementCreateX);
         var strArray = engine.GetSymbol("x").AsCharacter().ToArray();
         engine.Evaluate("rm(x)");
         var memoryAfterAlloc = GetBaselineDotnetMemory(engine);
         Assert.That(memoryAfterAlloc - memoryInitial, Is.GreaterThan(expectedMinBytesDifference));
         strArray = null;
         var memoryAfterGC = GetBaselineDotnetMemory(engine);
         Assert.That(memoryAfterAlloc - memoryAfterGC, Is.GreaterThan(expectedMinBytesDifference));  // x should be collected.
      }

      [Test]
      public void TestParseCodeLine()
      {
         var engine = this.Engine;
         engine.Evaluate("cat('hello')");
         Assert.That(Device.GetString(), Is.EqualTo("hello"));
      }

      [Test]
      public void TestParseCodeBlock()
      {
         var engine = this.Engine;
         engine.Evaluate("for(i in 1:3){\ncat(i)\ncat(i)\n}");
         Assert.That(Device.GetString(), Is.EqualTo("112233"));
      }

      [Test]
      public void TestReadConsole()
      {
         var engine = this.Engine;
         Device.Input = "Hello, World!";
         Assert.That(engine.Evaluate("readline()").AsCharacter()[0], Is.EqualTo(Device.Input));
      }

      [Test]
      public void TestWriteConsole()
      {
         var engine = this.Engine;
         engine.Evaluate("print(NULL)");
         Assert.That(Device.GetString(), Is.EqualTo("NULL\n"));
      }

      [Test]
      public void TestCallingTwice()
      {
         var engine = this.Engine;
         engine.Evaluate("a <- 1");
         engine.Evaluate("a <- a+1");
         NumericVector v1 = engine.GetSymbol("a").AsNumeric();
         Assert.AreEqual(2.0, v1[0]);
         engine.Evaluate("a <- a+1");
         NumericVector v2 = engine.GetSymbol("a").AsNumeric();
         Assert.AreEqual(3.0, v2[0]);
      }
   }
}