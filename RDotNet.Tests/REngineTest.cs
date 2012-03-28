using NUnit.Framework;

namespace RDotNet.Tests
{
	[TestFixture]
	class REngineTest
	{
		private const string EngineName = "RDotNetTest";

		[TestFixtureSetUp]
		public void SetUpEngine()
		{
			Helper.SetEnvironmentVariables();
			var engine = REngine.CreateInstance(EngineName);
			engine.Initialize();
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
		}

		[Test]
		public void TestSetCommandLineArguments()
		{
			var engine = REngine.GetInstanceFromID(EngineName);
			engine.SetCommandLineArguments(new[] { "Hello", "World" });
			Assert.That(engine.Evaluate("commandArgs()").AsCharacter(), Is.EquivalentTo(new[] { EngineName, "Hello", "World" }));
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
	}
}
