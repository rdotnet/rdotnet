using NUnit.Framework;
using RDotNet.NativeLibrary;
using System;
using System.IO;
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

      // TODO: probably needs adjustments for MacOS and Linux
      [Test]
      public void TestFindRBinPath()
      {
         string rLibPath = NativeUtility.FindRPath();
         var files = Directory.GetFiles(rLibPath);
         var fnmatch = files.Where(fn => fn.ToLower() == Path.Combine(rLibPath.ToLower(), NativeUtility.GetRDllFileName().ToLower()));
         Assert.AreEqual(1, fnmatch.Count());
      }

      [Test]
      public void TestFindRHomePath()
      {
         string rHomePath = NativeUtility.FindRHome();
         var files = Directory.GetFiles(rHomePath);
         var fnmatch = files.Where(fn => Path.GetFileName(fn) == "CHANGES");
         Assert.AreEqual(1, fnmatch.Count());
      }
   }
}