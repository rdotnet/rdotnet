using Xunit;
using System.Linq;

namespace RDotNet
{
    public class FactorTest : RDotNetTestFixture
    {
        [Fact]
        public void TestLength()
        {
            var engine = this.Engine;
            var factor = engine.Evaluate("factor(c('A', 'B', 'A', 'C', 'B'))").AsFactor();
            Assert.Equal(factor.Length, (5));
        }

        [Fact]
        public void TestIsOrderedTrue()
        {
            var engine = this.Engine;
            var factor = engine.Evaluate("factor(c('A', 'B', 'A', 'C', 'B'), ordered=TRUE)").AsFactor();
            Assert.Equal(factor.IsOrdered, true);
        }

        [Fact]
        public void TestMissingValues()
        {
            var engine = this.Engine;
            var factor = engine.Evaluate("x <- factor(c('A', 'B', 'A', NA, 'C', 'B'), ordered=TRUE)").AsFactor();
            Assert.Equal(factor.GetFactors(), (new[] { "A", "B", "A", null, "C", "B" }));
            factor = engine.Evaluate(@"
levels(x) <- c('1st', '2nd', '3rd')
x
").AsFactor();
            Assert.Equal(factor.GetFactors(), (new[] { "1st", "2nd", "1st", null, "3rd", "2nd" }));
        }

        [Fact]
        public void TestIsOrderedFalse()
        {
            var engine = this.Engine;
            var factor = engine.Evaluate("factor(c('A', 'B', 'A', 'C', 'B'), ordered=FALSE)").AsFactor();
            Assert.Equal(factor.IsOrdered, false);
        }

        [Fact]
        public void TestGetLevels()
        {
            var engine = this.Engine;
            var factor = engine.Evaluate("x <- factor(c('A', 'B', 'A', 'C', 'B'))").AsFactor();
            Assert.Equal(factor.GetLevels(), (new[] { "A", "B", "C" }));
            factor = engine.Evaluate(@"
levels(x) <- c('1st', '2nd', '3rd')
x
").AsFactor();
            Assert.Equal(factor.GetLevels(), (new[] { "1st", "2nd", "3rd" }));
        }

        [Fact]
        public void TestGetFactors()
        {
            var engine = this.Engine;
            var factor = engine.Evaluate("x <- factor(c('A', 'B', 'A', 'C', 'B'))").AsFactor();
            Assert.Equal(factor.GetFactors(), (new[] { "A", "B", "A", "C", "B" }));
            factor = engine.Evaluate(@"
levels(x) <- c('1st', '2nd', '3rd')
x
").AsFactor();
            Assert.Equal(factor.GetFactors(), (new[] { "1st", "2nd", "1st", "3rd", "2nd" }));
        }

        [Fact]
        public void TestGetFactorsEnum()
        {
            var engine = this.Engine;
            var code = "factor(c(rep('T', 5), rep('C', 5), rep('T', 4), rep('C', 5)), levels=c('T', 'C'), labels=c('Treatment', 'Control'))";
            var factor = engine.Evaluate(code).AsFactor();
            var expected = Enumerable.Repeat(Group.Treatment, 5)
                                     .Concat(Enumerable.Repeat(Group.Control, 5))
                                     .Concat(Enumerable.Repeat(Group.Treatment, 4))
                                     .Concat(Enumerable.Repeat(Group.Control, 5));
            Assert.Equal(factor.GetFactors<Group>(), (expected));
        }

        [Fact]
        public void TestAsCharacterFactors()
        {
            var engine = this.Engine;
            var c = engine.Evaluate("as.factor(rep(letters[1:3], 5))").AsCharacter();
            Assert.Equal("a", c[0]);
            Assert.Equal("b", c[1]);
            Assert.Equal("c", c[2]);
            Assert.Equal("a", c[3]);
        }
    }

    public enum Group
    {
        Treatment = 1,
        Control = 2,
    }
}