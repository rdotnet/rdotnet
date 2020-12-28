using Xunit;

namespace RDotNet
{
    public class InternalStringTest : RDotNetTestFixture
    {
        [Fact]
        public void TestCharacter()
        {
            SetUpTest();
            var engine = this.Engine;
            var vector = engine.Evaluate("c('foo', NA, 'bar')").AsCharacter();
            Assert.Equal(vector.Length, (3));
            Assert.Equal(vector[0], ("foo"));
            Assert.Null(vector[1]);
            Assert.Equal(vector[2], ("bar"));
        }
    }
}