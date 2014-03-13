using NUnit.Framework;
using System;
using System.IO;

namespace RDotNet
{
   [TestFixture]
   internal class REngineInstanceTest
   {
      [TestFixtureSetUp]
      public void SetUp()
      {
         REngine.SetEnvironmentVariables();
      }

      [Test, Ignore] // Not useful with new API; rethink.
      [ExpectedException(typeof(ArgumentNullException))]
      public void TestCreateInstanceWithNull()
      {
         Assert.That(TestREngine.CreateTestEngine(null), Throws.TypeOf<ArgumentNullException>());
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestCreateInstanceWithEmpty()
      {
         Assert.That(TestREngine.CreateTestEngine(""), Throws.TypeOf<ArgumentException>());
      }

      [Test]
      [ExpectedException(typeof(DllNotFoundException))]
      public void TestCreateInstanceWithWrongDllName()
      {
         Assert.That(TestREngine.CreateTestEngine("R.NET", "NotExist.dll"), Throws.TypeOf<DllNotFoundException>());
      }

      /// <summary>
      /// A facility to test REngine behavior in unit tests without needing to dispose of the REngine singleton
      /// </summary>
      private class TestREngine : REngine
      {
         public static TestREngine CreateTestEngine(string id, string dll = null)
         {
            dll = ProcessRDllFileName(dll); // as is done in REngine.CreateInstance. not ideal; rethink.
            return new TestREngine(id, dll);
         }
         public TestREngine(string id, string dll)
            : base(id, dll) { }
      }

      [Test, Ignore] // cannot test this easily with new API. Rethink
      public void TestIsRunning()
      {
         var engine = REngine.GetInstance();
         Assert.That(engine, Is.Not.Null);
         Assert.That(engine.IsRunning, Is.False);
         engine.Initialize();
         Assert.That(engine.IsRunning, Is.True);
         engine.Dispose();
         Assert.That(engine.IsRunning, Is.False);
      }

      // Marking this test as ignore, as it is incompatible with trying to get all unit tests 
      // run from NUnit to pass successfully.
      // Keeping the test code as a basis for potential further feasibility investigations
      [Test, Ignore]
      public void TestCreateEngineTwice()
      {
         // Investigate:
         // https://rdotnet.codeplex.com/workitem/54
         var engine = REngine.GetInstance();
         engine.Initialize();
         var paths = engine.Evaluate(".libPaths()").AsCharacter().ToArrayFast();
         Console.WriteLine(engine.Evaluate("Sys.getenv('R_HOME')").AsCharacter().ToArrayFast()[0]);
         // engine.Evaluate("library(rjson)");
         engine.Dispose();
         Console.WriteLine("Before second creation");
         engine = REngine.GetInstance();
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

      [Test, Ignore] // TODO
      public void TestSeveralAppDomains()
      {
         var engine = REngine.GetInstance();
         engine.Initialize();

         // create another AppDomain for loading the plug-ins
         AppDomainSetup setup = new AppDomainSetup();
         setup.ApplicationBase = Path.GetDirectoryName(typeof(REngine).Assembly.Location);

         setup.DisallowApplicationBaseProbing = false;
         setup.DisallowBindingRedirects = false;

         var domain = AppDomain.CreateDomain("Plugin AppDomain", null, setup);

         domain.Load(typeof(REngine).Assembly.EscapedCodeBase);

      }
   }
}