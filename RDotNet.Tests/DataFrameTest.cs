using NUnit.Framework;

namespace RDotNet
{
   class DataFrameTest : RDotNetTestFixture
   {
      [Test]
      public void TestIsDataFrameTrue()
      {
         var engine = this.Engine;
         var iris = engine.Evaluate("data.frame()");
         Assert.That(iris.IsDataFrame(), Is.True);
      }

      [Test]
      public void TestIsDataFrameFalse()
      {
         var engine = this.Engine;
         var iris = engine.Evaluate("list()");
         Assert.That(iris.IsDataFrame(), Is.False);
      }

      [Test]
      public void TestDataFrameFactorColumns()
      {
         var engine = this.Engine;
         engine.Evaluate("data(mpg, package='ggplot2')");
         var mpg = engine.Evaluate("mpg").AsDataFrame();
         var manufacturer = mpg[0].AsFactor().GetFactors();
         Assert.AreEqual("audi", manufacturer[0]);
      }
   }
}
