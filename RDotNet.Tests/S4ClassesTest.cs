using Xunit;

namespace RDotNet
{
    public class S4ClassesTest : RDotNetTestFixture
    {
        [Fact]
        public void TestSlots()
        {
            var engine = this.Engine;

            engine.Evaluate("track <- setClass('track', slots = c(x='numeric', y='numeric'))");
            var t1 = engine.Evaluate("track(x = 1:10, y = 1:10 + rnorm(10))").AsS4();

            Assert.Equal(true, t1.HasSlot("x"));
            Assert.Equal(false, t1.HasSlot("X"));
            Assert.Equal(new[] { "x", "y" }, t1.SlotNames);

            double[] x = t1["x"].AsNumeric().ToArray();
            var expx = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Assert.Equal(expx, x);
            double[] y = t1["y"].AsNumeric().ToArray();
            y[2] = 0.1;
            t1["y"] = engine.CreateNumericVector(y);
            Assert.Equal(y, t1["y"].AsNumeric().ToArray());
        }

        [Fact]
        public void TestGetSlotTypes()
        {
            var engine = this.Engine;
            engine.Evaluate("setClass('testclass', representation(foo='character', bar='integer'))");
            var obj = engine.Evaluate("new('testclass', foo='s4', bar=1:4)").AsS4();
            var actual = obj.GetSlotTypes();
            Assert.Equal(actual.Count, (2));
            Assert.True(actual.ContainsKey("foo"));
            Assert.Equal(actual["foo"], ("character"));
            Assert.True(actual.ContainsKey("bar"));
            Assert.Equal(actual["bar"], ("integer"));
        }
    }
}