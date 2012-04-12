using System;
using System.IO;
using System.Windows.Forms;
using NUnit.Framework;

namespace RDotNet.Graphics.Tests
{
	[TestFixture]
	internal class GraphicsDeviceTest
	{
		#region Setup/Teardown

		[TearDown]
		public void TearDown()
		{
			var engine = REngine.GetInstanceFromID(EngineName);
			engine.Evaluate("rm(list=ls())");
		}

		#endregion

		private const string EngineName = "RDotNetTest";

		private static readonly GraphForm form;

		static GraphicsDeviceTest()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			form = new GraphForm();
		}

		[TestFixtureSetUp]
		public void SetUpEngine()
		{
			Helper.SetEnvironmentVariables();
			var engine = REngine.CreateInstance(EngineName);
			engine.Initialize();
			engine.Install(form.graphPanel);
			form.Engine = engine;
		}

		[TestFixtureTearDown]
		public void DisposeEngine()
		{
			var engine = REngine.GetInstanceFromID(EngineName);
			if (engine != null)
			{
				engine.Dispose();
			}
			form.Dispose();
		}

		[Test]
		public void Test()
		{
			var path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".png");
			form.Code = "plot(1:5)";
			form.TempImagePath = path;
			try
			{
				GC.Collect();
				var result = form.ShowDialog();
				Assert.That(result, Is.EqualTo(DialogResult.OK));
			}
			finally 
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
		}
	}
}
