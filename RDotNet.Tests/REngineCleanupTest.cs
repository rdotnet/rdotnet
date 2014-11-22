using NUnit.Framework;
using System;
using System.Linq;

namespace RDotNet
{
   public class REngineCleanupTest : RDotNetTestFixture
   {
      [Test]
      public void TestDefaultClearGlobalEnv()
      {
         var engine = this.Engine;
         engine.ClearGlobalEnvironment();
         var s = engine.Evaluate("ls()").AsCharacter().ToArray();
         Assert.IsTrue(s.Length == 0);
      }
   
      [Test]
      public void TestDetachPackagesDefault()
      {
         var engine = this.Engine;

         var s = engine.Evaluate("search()").AsCharacter().ToArray();
         Assert.IsFalse(s.Contains("package:lattice"));
         Assert.IsFalse(s.Contains("package:knitr"));
         Assert.IsFalse(s.Contains("mpg"));

         engine.ClearGlobalEnvironment();
         engine.Evaluate("library(lattice)");
         engine.Evaluate("library(knitr)");
         engine.Evaluate("data(mpg, package='ggplot2')");
         engine.Evaluate("attach(mpg)");
         s = engine.Evaluate("search()").AsCharacter().ToArray();

         Assert.IsTrue(s.Contains("package:lattice"));
         Assert.IsTrue(s.Contains("package:knitr"));
         Assert.IsTrue(s.Contains("mpg"));

         engine.ClearGlobalEnvironment(detachPackages: true);

         s = engine.Evaluate("search()").AsCharacter().ToArray();
         Assert.IsFalse(s.Contains("package:lattice"));
         Assert.IsFalse(s.Contains("package:knitr"));
         Assert.IsFalse(s.Contains("mpg"));

      }
   }
}