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
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.SetCommandLineArguments(new[] { "Hello", "World" });
         Assert.That(engine.Evaluate("commandArgs()").AsCharacter(), Is.EquivalentTo(new[] { EngineName, "Hello", "World" }));
      }

      [Test]
      public void TestDefaultCommandLineArgs()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         var cmdArgs = engine.Evaluate("commandArgs()").AsCharacter();
      }

      [Test]
      public void TestGlobalEnvironment()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         Assert.That(engine.GlobalEnvironment.DangerousGetHandle(), Is.EqualTo(engine.Evaluate(".GlobalEnv").DangerousGetHandle()));
      }

      [Test]
      public void TestBaseNamespace()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         Assert.That(engine.BaseNamespace.DangerousGetHandle(), Is.EqualTo(engine.Evaluate(".BaseNamespaceEnv").DangerousGetHandle()));
      }

      [Test]
      public void TestNilValue()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         Assert.That(engine.NilValue.DangerousGetHandle(), Is.EqualTo(engine.Evaluate("NULL").DangerousGetHandle()));
      }

      [Test]
      public void TestGC()
      {
         //> gc()
         //> gc()
         //> memory.size()
         //[1] 24.7
         //> x <- numeric(5e6)
         //> object.size(x)
         //40000040 bytes
         //> memory.size()
         //[1] 64.11
         //> gc()
         //> memory.size()
         //[1] 62.89
         //> rm(x)
         //> gc()
         //> memory.size()
         //[1] 24.7

         var engine = REngine.GetInstanceFromID(EngineName);
         GC.Collect();
         // it seems important to call gc() twice to get a proper baseline.
         engine.ForceGarbageCollection();
         engine.ForceGarbageCollection();
         var memoryInitial = engine.Evaluate("memory.size()").AsNumeric().First();
         engine.Evaluate("x <- numeric(5e6)"); 
         GC.Collect();
         engine.ForceGarbageCollection();
         engine.ForceGarbageCollection();
         var memoryAfterAlloc = engine.Evaluate("memory.size()").AsNumeric().First();
         // For some reasons the delta is not 40MB spot on. Use 35 MB as a threshold
         Assert.That(memoryAfterAlloc - memoryInitial, Is.GreaterThan(35.0)); 
         engine.Evaluate("rm(x)");
         GC.Collect();
         engine.ForceGarbageCollection();
         var memoryAfterGC = engine.Evaluate("memory.size()").AsNumeric().First();
         Assert.That(memoryAfterAlloc - memoryAfterGC, Is.GreaterThan(35.0));  // x should be collected.
      }

      [Test]
      public void TestParseCodeLine()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("cat('hello')");
         Assert.That(Device.GetString(), Is.EqualTo("hello"));
      }

      [Test]
      public void TestParseCodeBlock()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("for(i in 1:3){\ncat(i)\ncat(i)\n}");
         Assert.That(Device.GetString(), Is.EqualTo("112233"));
      }

      [Test]
      public void TestReadConsole()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         Device.Input = "Hello, World!";
         Assert.That(engine.Evaluate("readline()").AsCharacter()[0], Is.EqualTo(Device.Input));
      }

      [Test]
      public void TestWriteConsole()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("print(NULL)");
         Assert.That(Device.GetString(), Is.EqualTo("NULL\n"));
      }
   }
}