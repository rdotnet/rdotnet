using NUnit.Framework;
using RDotNet.Internals;

namespace RDotNet
{
   internal class EnvironmentTest : RDotNetTestFixture
   {
      [Test]
      public void TestCreateEnvironment()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         var newEnv = engine.CreateEnvironment(engine.BaseNamespace);
         Assert.That(newEnv.Type, Is.EqualTo(SymbolicExpressionType.Environment));
         Assert.That(newEnv.Parent.DangerousGetHandle(), Is.EqualTo(engine.BaseNamespace.DangerousGetHandle()));
      }

      [Test]
      public void TestCreateIsolatedEnvironment()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         var newEnv = engine.CreateIsolatedEnvironment();
         Assert.That(newEnv.Type, Is.EqualTo(SymbolicExpressionType.Environment));
         Assert.That(newEnv.Parent, Is.Null);
      }
   }
}