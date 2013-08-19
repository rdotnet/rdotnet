using NUnit.Framework;

namespace RDotNet
{
   [TestFixture]
   public class RDotNetTestFixture
   {
      private readonly MockDevice device = new MockDevice();

      protected string EngineName { get { return "RDotNetTest"; } }

      protected MockDevice Device { get { return this.device; } }

      [TestFixtureSetUp]
      protected virtual void SetUpFixture()
      {
         Helper.SetEnvironmentVariables();
         var engine = REngine.CreateInstance(EngineName);
         engine.Initialize(device: device);
      }

      [TestFixtureTearDown]
      protected virtual void TearDownFixture()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         if (engine != null)
         {
            engine.Dispose();
         }
      }

      [SetUp]
      protected virtual void SetUpTest()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("rm(list=ls())");
         this.device.Initialize();
      }

      [TearDown]
      protected virtual void TearDownTest()
      {
      }
   }
}