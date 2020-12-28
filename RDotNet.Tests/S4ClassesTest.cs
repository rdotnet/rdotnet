using Xunit;

namespace RDotNet
{
    public class S4ClassesTest : RDotNetTestFixture
    {
        [Fact]
        public void TestSlots()
        {
            SetUpTest();
            var engine = this.Engine;

            engine.Evaluate("track <- setClass('track', slots = c(x='numeric', y='numeric'))");
            var t1 = engine.Evaluate("track(x = 1:10, y = 1:10 + rnorm(10))").AsS4();

            Assert.True(t1.HasSlot("x"));
            Assert.False(t1.HasSlot("X"));
            Assert.Equal(new[] { "x", "y" }, t1.SlotNames);

            Assert.Equal(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                t1["x"].AsInteger().ToArray());
            Assert.Equal(
                new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                t1["x"].AsNumeric().ToArray());

            double[] y = t1["y"].AsNumeric().ToArray();
            y[2] = 0.1;
            t1["y"] = engine.CreateNumericVector(y);
            Assert.Equal(y, t1["y"].AsNumeric().ToArray());
        }

        [Fact]
        public void TestGetSlotTypes()
        {
            SetUpTest();
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