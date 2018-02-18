using Xunit;

namespace RDotNet
{
    public class CharacterVectorTest : RDotNetTestFixture
    {
        [Fact]
        public void TestCharacter()
        {
            SetUpTest();
            var engine = this.Engine;
            var vector = engine.Evaluate("x <- c('foo', NA, 'bar')").AsCharacter();
            Assert.Equal(vector.Length, (3));
            Assert.Equal(vector[0], ("foo"));
            Assert.Null(vector[1]);
            Assert.Equal(vector[2], ("bar"));
            vector[0] = null;
            Assert.Null(vector[0]);
            var logical = engine.Evaluate("is.na(x)").AsLogical();
            Assert.Equal(logical[0], true);
            Assert.Equal(logical[1], true);
            Assert.Equal(logical[2], false);
        }

        [Fact]
        public void TestUnicodeCharacter()
        {
            SetUpTest();
            var engine = this.Engine;
            var vector = engine.Evaluate("x <- c('красавица Наталья', 'Un apôtre')").AsCharacter();
            var encoding = engine.Evaluate("Encoding(x)").AsCharacter();
            Assert.Equal(encoding[0], ("UTF-8"));
            Assert.Equal(encoding[1], ("UTF-8"));
            
            Assert.Equal(vector.Length, (2));
            Assert.Equal(vector[0], ("красавица Наталья"));
            Assert.Equal(vector[1], ("Un apôtre"));
        }

        [Fact]
        public void TestDotnetToR()
        {
            SetUpTest();
            var engine = this.Engine;
            var vector = engine.Evaluate("x <- character(100)").AsCharacter();
            Assert.Equal(vector.Length, (100));
            Assert.Equal(vector[0], (""));
            vector[1] = "foo";
            vector[2] = "bar";
            var second = engine.Evaluate("x[2]").AsCharacter().ToArray();
            Assert.Single(second);
            Assert.Equal("foo", second[0]);

            var third = engine.Evaluate("x[3]").AsCharacter().ToArray();
            Assert.Single(third);
            Assert.Equal("bar", third[0]);
        }
    }
}