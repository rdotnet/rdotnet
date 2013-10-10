using NUnit.Framework;
using System;

namespace RDotNet
{
   [TestFixture]
   internal class REngineInstanceTest
   {
      [TestFixtureSetUp]
      public void SetUp()
      {
         Helper.SetEnvironmentVariables();
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void TestCreateInstanceWithNull()
      {
         Assert.That(REngine.CreateInstance(null), Throws.TypeOf<ArgumentNullException>());
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestCreateInstanceWithEmpty()
      {
         Assert.That(REngine.CreateInstance(""), Throws.TypeOf<ArgumentException>());
      }

      [Test]
      [ExpectedException(typeof(DllNotFoundException))]
      public void TestCreateInstanceWithWrongDllName()
      {
         Assert.That(REngine.CreateInstance("RDotNetTest", "NotExist.dll"), Throws.TypeOf<DllNotFoundException>());
      }

      [Test]
      public void TestIsRunning()
      {
         Assert.That(REngine.GetInstanceFromID("RDotNetTest"), Is.Null);
         var engine = REngine.CreateInstance("RDotNetTest");
         Assert.That(engine, Is.Not.Null);
         Assert.That(engine.IsRunning, Is.False);
         engine.Initialize();
         Assert.That(engine.IsRunning, Is.True);
         engine.Dispose();
         Assert.That(engine.IsRunning, Is.False);
      }

      [Test]
      public void TestCreateEngineTwice()
      {
         // Investigate:
         // https://rdotnet.codeplex.com/workitem/54
         var engine = REngine.CreateInstance("RDotNetTest");
         engine.Initialize();
         var paths = engine.Evaluate(".libPaths()").AsCharacter().ToArrayFast();
         Console.WriteLine(engine.Evaluate("Sys.getenv('R_HOME')").AsCharacter().ToArrayFast()[0]);
         engine.Dispose();
         Console.WriteLine("Before second creation");
         engine = REngine.CreateInstance("RDotNetTest");
         Console.WriteLine("Before second initialize");
         engine.Initialize();
         Console.WriteLine(engine.Evaluate("Sys.getenv('R_HOME')").AsCharacter().ToArrayFast()[0]);
         paths = engine.Evaluate(".libPaths()").AsCharacter().ToArrayFast();
         try
         {
            engine.Evaluate("library(methods)");
         }
         catch
         {
            engine.Evaluate("traceback()");
            throw;
         }
         finally
         {
            engine.Dispose();
         }
         Assert.That(engine.IsRunning, Is.False);
      }
   }
}