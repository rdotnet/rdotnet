using System;
using System.Linq;
using NUnit.Framework;

namespace RDotNet
{
	[TestFixture]
	class REngineTest
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

		[Test]
		public void TestGC()
		{
			var engine = REngine.GetInstanceFromID(EngineName);
			var memoryInitial = engine.Evaluate("memory.size()").AsNumeric().First();
			engine.Evaluate("x <- numeric(5000000)");  // About 38 MB (should be larger than startup memory size)
			GC.Collect();
			engine.ForceGarbageCollection();
			var memoryAfterAlloc = engine.Evaluate("memory.size()").AsNumeric().First();
			Assert.That(memoryAfterAlloc - memoryInitial, Is.GreaterThan(35.0));  // x should not be collected.
			engine.Evaluate("rm(x)");
			GC.Collect();
			engine.ForceGarbageCollection();
			var memoryAfterGC = engine.Evaluate("memory.size()").AsNumeric().First();
			Assert.That(memoryAfterAlloc - memoryAfterGC, Is.GreaterThan(35.0));  // x should be collected.
		}

		[Test]
		public void TestReadConsole()
		{
			var engine = REngine.GetInstanceFromID(EngineName);
			this.device.Input = "Hello, World!";
			Assert.That(engine.Evaluate("readline()").AsCharacter()[0], Is.EqualTo(this.device.Input));
		}

		[Test]
		public void TestWriteConsole()
		{
			var engine = REngine.GetInstanceFromID(EngineName);
			engine.Evaluate("print(NULL)");
			Assert.That(this.device.GetString(), Is.EqualTo("NULL\n"));
		}
	}
}
