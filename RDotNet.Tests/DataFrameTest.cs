using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

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

      [Test]
      public void TestDataFrameInMemoryCreation()
      {
         var engine = this.Engine;
         IEnumerable[] columns = new IEnumerable[3];
         columns[0] = new string[] { "a", "a", "a", "a", "a", "b", "b", "b", "b", "b" };
         columns[1] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
         columns[2] = new double[] { 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 8.1, 9.1, 10.1 };

         var columnNames = new[] { "Category", "No.", "Measure" };
         var df = engine.CreateDataFrame(columns, columnNames: columnNames);
         checkDataFrameContent(df);
         Assert.AreEqual(columnNames, df.ColumnNames);
         df = engine.CreateDataFrame(columns, columnNames: null);
         checkDataFrameContent(df);

         columns[1] = new int[] { 1, 2, 3, 4, 5, 6, 7 };
         Assert.Throws(typeof(EvaluationException), (() => df = engine.CreateDataFrame(columns, columnNames: null)));

      }

      private static void checkDataFrameContent(DataFrame df)
      {
         var cat = df[0].AsFactor().GetFactors();
         Assert.AreEqual("a", cat[0]);
         Assert.AreEqual("a", cat[4]);
         Assert.AreEqual("b", cat[5]);

         var numbers = df[1].AsInteger();
         for (int i = 0; i < 10; i++)
            Assert.AreEqual(i + 1, numbers[i]);

         var measures = df[2].AsNumeric();
         for (int i = 0; i < 10; i++)
            Assert.AreEqual(i + 1.1, measures[i]);
      }
   }
}
