using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using RDotNet.NativeLibrary;
using System;

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
         // TODO: move to use a package installed by default with R
         var mpg = getMpgDataFrame(engine);
         var manufacturer = mpg[0].AsFactor().GetFactors();
         Assert.AreEqual("audi", manufacturer[0]);
         manufacturer = mpg[0].AsCharacter().ToArray();
         Assert.AreEqual("audi", manufacturer[0]);
      }

      private static DataFrame getMpgDataFrame(REngine engine)
      {
         engine.Evaluate("data(mpg, package='ggplot2')");
         var mpg = engine.Evaluate("mpg").AsDataFrame();
         return mpg;
      }

      //[Test]
      public void TestDataFrameInMemoryCreationTwice()
      {
         // https://rdotnet.codeplex.com/workitem/146
         TestDataFrameInMemoryCreation();
         TestDataFrameInMemoryCreation();
      }
         
      [Test]
      public void TestDataFrameInMemoryCreation()
      {
         var engine = this.Engine;
         IEnumerable[] columns;
         string[] columnNames;
         DataFrame df;
         createTestDataFrame(engine, out columns, out columnNames, out df);
         checkDataFrameContent(df);
         Assert.AreEqual(columnNames, df.ColumnNames);
         df = engine.CreateDataFrame(columns, columnNames: null);
         checkDataFrameContent(df);

         string additionalMsg = "https://rdotnet.codeplex.com/workitem/146";
         ReportFailOnLinux(additionalMsg);

         columns[1] = new int[] { 1, 2, 3, 4, 5, 6, 7 };
         // NOTE: on at least one machine, this fails at the first test run with an OutOfMemoryException
         // It is unclear at what level this occurs; if I follow the instructions at http://stackoverflow.com/questions/36014/why-is-net-exception-not-caught-by-try-catch-block
         // and disable "Just my code" the stack trace shows a TargetInvocationException in a NUnit only call stack



         Assert.Throws(typeof(EvaluationException), (() => df = engine.CreateDataFrame(columns, columnNames: null)));

//         try
//         {
//            df = engine.CreateDataFrame(columns, columnNames: null);
//         }
//         catch (Exception ex)
//         {
//            Assert.True(ex is EvaluationException);
//         }

      }

      private static void createTestDataFrame(REngine engine, out IEnumerable[] columns, out string[] columnNames, out DataFrame df)
      {
         columns = createTestDfColumns();
         columnNames = new[] { "Category", "No.", "Measure" };
         df = engine.CreateDataFrame(columns, columnNames: columnNames);
      }

      private static IEnumerable[] createTestDfColumns()
      {
         IEnumerable[] columns = new IEnumerable[3];
         columns[0] = new string[] { "a", "a", "a", "a", "a", "b", "b", "b", "b", "b" };
         columns[1] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
         columns[2] = new double[] { 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 8.1, 9.1, 10.1 };
         return columns;
      }

      [Test]
      public void TestDataElementTwoDimIndex()
      {
         var engine = this.Engine;
         var mpg = getMpgDataFrame(engine);

         /*
           manufacturer model displ year cyl      trans drv cty hwy fl   class
1         audi    a4   1.8 1999   4   auto(l5)   f  18  29  p compact
2         audi    a4   1.8 1999   4 manual(m5)   f  21  29  p compact
3         audi    a4   2.0 2008   4 manual(m6)   f  20  31  p compact
4         audi    a4   2.0 2008   4   auto(av)   f  21  30  p compact
5         audi    a4   2.8 1999   6   auto(l5)   f  16  26  p compact
6         audi    a4   2.8 1999   6 manual(m5)   f  18  26  p compact
          * > str(mpg)
'data.frame':   234 obs. of  11 variables:
 $ manufacturer: Factor w/ 15 levels "audi","chevrolet",..: 1 1 1 1 1 1 1 1 1 1 ...
 $ model       : Factor w/ 38 levels "4runner 4wd",..: 2 2 2 2 2 2 2 3 3 3 ...
 $ displ       : num  1.8 1.8 2 2 2.8 2.8 3.1 1.8 1.8 2 ...
 $ year        : int  1999 1999 2008 2008 1999 1999 2008 1999 1999 2008 ...
 $ cyl         : int  4 4 4 4 6 6 6 4 4 4 ...
 $ trans       : Factor w/ 10 levels "auto(av)","auto(l3)",..: 4 9 10 1 4 9 1 9 4 10 ...
 $ drv         : Factor w/ 3 levels "4","f","r": 2 2 2 2 2 2 2 1 1 1 ...
 $ cty         : int  18 21 20 21 16 18 18 18 16 20 ...
 $ hwy         : int  29 29 31 30 26 26 27 26 25 28 ...
 $ fl          : Factor w/ 5 levels "c","d","e","p",..: 4 4 4 4 4 4 4 4 4 4 ...
 $ class       : Factor w/ 7 levels "2seater","compact",..: 2 2 2 2 2 2 2 2 2 2 ...

         */

         var obj = mpg[0,0];
         Assert.AreEqual(typeof(string), obj.GetType());
         var s = (string)obj;
         Assert.AreEqual("audi", s);
         Assert.AreEqual("audi", mpg[0, "manufacturer"]);
         // While R does 'something' where there is no names defined, the behavior seems inconsistent between rownames in data frames
         // and named vectors. Better not support this for the time being.
//         Assert.AreEqual("audi", mpg["1", "manufacturer"]);
         Assert.AreEqual("auto(av)", mpg[3, 5]);
         Assert.AreEqual("auto(av)", mpg[3, "trans"]);
//         Assert.AreEqual("auto(av)", mpg["4", "trans"]);
         Assert.AreEqual(2008, mpg[2, 3]);
         Assert.AreEqual(2008, mpg[2, "year"]);
//         Assert.AreEqual(2008, mpg["3", "year"]);

         IEnumerable[] columns;
         string[] columnNames;
         DataFrame df;
         createTestDataFrame(engine, out columns, out columnNames, out df);

         Assert.AreEqual("a", df[0, 0]);
         df[0, 0] = "b";
         Assert.AreEqual("b", df[0, 0]);
         df[0, 0] = "c";
         Assert.AreEqual(null, df[0, 0]);
         Assert.AreEqual("b", df[5, 0]);
         df[5, 0] = null;
         Assert.AreEqual(null, df[5, 0]);
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
