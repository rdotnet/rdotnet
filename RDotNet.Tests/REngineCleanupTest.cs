using NUnit.Framework;
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
            Assert.IsFalse(s.Contains("package:Matrix"));
            Assert.IsFalse(s.Contains("package:MASS"));
            Assert.IsFalse(s.Contains("biopsy"));

            engine.ClearGlobalEnvironment();
            engine.Evaluate("library(lattice)");
            engine.Evaluate("library(Matrix)");
            engine.Evaluate("library(MASS)");
            engine.Evaluate("data(biopsy, package='MASS')");
            engine.Evaluate("attach(biopsy)");
            s = engine.Evaluate("search()").AsCharacter().ToArray();

            Assert.IsTrue(s.Contains("package:lattice"));
            Assert.IsTrue(s.Contains("package:Matrix"));
            Assert.IsTrue(s.Contains("package:MASS"));
            Assert.IsTrue(s.Contains("biopsy"));

            engine.ClearGlobalEnvironment(detachPackages: true);

            s = engine.Evaluate("search()").AsCharacter().ToArray();
            Assert.IsFalse(s.Contains("package:lattice"));
            Assert.IsFalse(s.Contains("package:Matrix"));
            Assert.IsFalse(s.Contains("package:MASS"));
            Assert.IsFalse(s.Contains("biopsy"));
        }
    }
}