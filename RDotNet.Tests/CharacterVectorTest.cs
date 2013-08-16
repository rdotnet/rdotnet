using NUnit.Framework;

namespace RDotNet
{
   [TestFixture]
   class CharacterVectorTest
   {
      private const string EngineName = "RDotNetTest";
      private readonly MockDevice device = new MockDevice();

      [TestFixtureSetUp]
      public void SetUpEngine()
      {
         Helper.SetEnvironmentVariables();
         var engine = REngine.CreateInstance(EngineName);
         engine.Initialize(device: device);
      }

      [TestFixtureTearDown]
      public void DisposeEngine()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         if (engine != null)
         {
            engine.Dispose();
         }
      }

      [TearDown]
      public void TearDown()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("rm(list=ls())");
         this.device.Initialize();
      }

      [Test]
      public void TestCharacter()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         var vector = engine.Evaluate("x <- c('foo', NA, 'bar')").AsCharacter();
         Assert.That(vector.Length, Is.EqualTo(3));
         Assert.That(vector[0], Is.EqualTo("foo"));
         Assert.That(vector[1], Is.Null);
         Assert.That(vector[2], Is.EqualTo("bar"));
         vector[0] = null;
         Assert.That(vector[0], Is.Null);
         var logical = engine.Evaluate("is.na(x)").AsLogical();
         Assert.That(logical[0], Is.True);
         Assert.That(logical[1], Is.True);
         Assert.That(logical[2], Is.False);
      }
   }
}
