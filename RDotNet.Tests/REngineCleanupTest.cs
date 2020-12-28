using Xunit;
using System.Linq;

namespace RDotNet
{
    public class REngineCleanupTest : RDotNetTestFixture
    {
        [Fact]
        public void TestDefaultClearGlobalEnv()
        {
            SetUpTest();
            var engine = this.Engine;
            engine.ClearGlobalEnvironment();
            var s = engine.Evaluate("ls()").AsCharacter().ToArray();
            Assert.True(s.Length == 0);
        }

        [Fact]
        public void TestDetachPackagesDefault()
        {
            SetUpTest();
            var engine = this.Engine;

            var s = engine.Evaluate("search()").AsCharacter().ToArray();
            Assert.False(s.Contains("package:lattice"));
            Assert.False(s.Contains("package:Matrix"));
            Assert.False(s.Contains("package:MASS"));
            Assert.False(s.Contains("biopsy"));

            engine.ClearGlobalEnvironment();
            engine.Evaluate("library(lattice)");
            engine.Evaluate("library(Matrix)");
            engine.Evaluate("library(MASS)");
            engine.Evaluate("data(biopsy, package='MASS')");
            engine.Evaluate("attach(biopsy)");
            s = engine.Evaluate("search()").AsCharacter().ToArray();

            Assert.True(s.Contains("package:lattice"));
            Assert.True(s.Contains("package:Matrix"));
            Assert.True(s.Contains("package:MASS"));
            Assert.True(s.Contains("biopsy"));

            engine.ClearGlobalEnvironment(detachPackages: true);

            s = engine.Evaluate("search()").AsCharacter().ToArray();
            Assert.False(s.Contains("package:lattice"));
            Assert.False(s.Contains("package:Matrix"));
            Assert.False(s.Contains("package:MASS"));
            Assert.False(s.Contains("biopsy"));
        }
    }
}