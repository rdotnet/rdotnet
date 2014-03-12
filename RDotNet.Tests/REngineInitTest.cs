using NUnit.Framework;
using System;
using System.Linq;

namespace RDotNet
{
   public class REngineInitTest
   {

      [Test, Ignore] // Cannot run this in a batch with the new singleton pattern.
      public void TestInitParams()
      {
         MockDevice device = new MockDevice();
         string EngineName = "RDotNet";
         REngine.SetEnvironmentVariables();
         using (var engine = REngine.GetInstance())
         {
            ulong maxMemSize = 128 * 1024 * 1024;
            StartupParameter parameter = new StartupParameter()
            {
               MaxMemorySize = maxMemSize,
            };
            engine.Initialize(parameter: parameter, device: device);
            Assert.That(engine.Evaluate("memory.limit()").AsNumeric()[0], Is.LessThan(128)); // returns 122 using command line R.exe
         }
      }
   }
}