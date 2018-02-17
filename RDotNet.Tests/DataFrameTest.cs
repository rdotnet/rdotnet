using Xunit;
using System.Collections;

namespace RDotNet
{
    public class DataFrameTest : RDotNetTestFixture
    {
        [Fact]
        public void TestIsDataFrameTrue()
        {
            var engine = this.Engine;
            var iris = engine.Evaluate("data.frame()");
            Assert.Equal(iris.IsDataFrame(), true);
        }

        [Fact]
        public void TestIsDataFrameFalse()
        {
            var engine = this.Engine;
            var iris = engine.Evaluate("list()");
            Assert.Equal(iris.IsDataFrame(), false);
        }

        [Fact]
        public void TestDataFrameFactorColumns()
        {
            var engine = this.Engine;
            var biopsy = getBiopsyDataFrame(engine);
            //$ class: Factor w/ 2 levels "benign","malignant": 1 1 1 1 1 2 1 1 1 1 ...
            var factor = biopsy[10].AsFactor();
            var classFact = factor.GetFactors();
            Assert.Equal(699, classFact.Length);
            Assert.Equal("benign", classFact[0]);
            Assert.Equal("malignant", classFact[5]);
            var levels = factor.GetLevels();
            Assert.Equal(2, levels.Length);
            Assert.Equal("benign", levels[0]);
            Assert.Equal("malignant", levels[1]);

            // Check that we get the following:
//> head(as.character(biopsy$class))
            //[1] "benign"    "benign"    "benign"    "benign"    "benign"    "malignant"
            //   NOT:
            //[1] "1"    "1"    "1"    "1"    "1"    "2" 
            checkClassString(factor.AsCharacter().ToArray());
            checkClassString(biopsy[10].AsCharacter().ToArray());

        }

        private static void checkClassString(string[] values)
        {
            Assert.Equal(699, values.Length);
            for (int i = 0; i < 5; i++)
                Assert.Equal("benign", values[i]);
            Assert.Equal("malignant", values[5]);
        }

        private static DataFrame getBiopsyDataFrame(REngine engine)
        {
            engine.Evaluate("data(biopsy, package='MASS')");
            var biopsy = engine.Evaluate("biopsy").AsDataFrame();
            return biopsy;
        }

        //[Fact]
        public void TestDataFrameInMemoryCreationTwice()
        {
            // https://rdotnet.codeplex.com/workitem/146
            TestDataFrameInMemoryCreation();
            TestDataFrameInMemoryCreation();
        }

        [Fact]
        public void TestDataFrameInMemoryCreation()
        {
            var engine = this.Engine;
            IEnumerable[] columns;
            string[] columnNames;
            DataFrame df;
            createTestDataFrame(engine, out columns, out columnNames, out df);
            checkDataFrameContent(df);
            Assert.Equal(columnNames, df.ColumnNames);
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

        [Fact]
        public void TestDataElementTwoDimIndex()
        {
            var engine = this.Engine;
            var biopsy = getBiopsyDataFrame(engine);

            /*

             * > head(biopsy)
       ID V1 V2 V3 V4 V5 V6 V7 V8 V9     class
1 1000025  5  1  1  1  2  1  3  1  1    benign
2 1002945  5  4  4  5  7 10  3  2  1    benign
3 1015425  3  1  1  1  2  2  3  1  1    benign
4 1016277  6  8  8  1  3  4  3  7  1    benign
5 1017023  4  1  1  3  2  1  3  1  1    benign
6 1017122  8 10 10  8  7 10  9  7  1 malignant

             * > str((biopsy))
'data.frame':   699 obs. of  11 variables:
 $ ID   : chr  "1000025" "1002945" "1015425" "1016277" ...
 $ V1   : int  5 5 3 6 4 8 1 2 2 4 ...
 $ V2   : int  1 4 1 8 1 10 1 1 1 2 ...
 $ V3   : int  1 4 1 8 1 10 1 2 1 1 ...
 $ V4   : int  1 5 1 1 3 8 1 1 1 1 ...
 $ V5   : int  2 7 2 3 2 7 2 2 2 2 ...
 $ V6   : int  1 10 2 4 1 10 10 1 1 1 ...
 $ V7   : int  3 3 3 3 3 9 3 3 1 2 ...
 $ V8   : int  1 2 1 7 1 7 1 1 1 1 ...
 $ V9   : int  1 1 1 1 1 1 1 1 5 1 ...
 $ class: Factor w/ 2 levels "benign","malignant": 1 1 1 1 1 2 1 1 1 1 ...

            */

            var obj = biopsy[0, 10];
            Assert.Equal(typeof(string), obj.GetType());
            var s = (string)obj;
            Assert.Equal("benign", s);
            Assert.Equal("benign", biopsy[0, "class"]);
            // While R does 'something' where there is no names defined, the behavior seems inconsistent between rownames in data frames
            // and named vectors. Better not support this for the time being.
            //         Assert.Equal("audi", mpg["1", "manufacturer"]);
            Assert.Equal("malignant", biopsy[5, 10]);
            Assert.Equal("malignant", biopsy[5, "class"]);
            Assert.Equal(4, biopsy[1, 3]);
            Assert.Equal(4, biopsy[1, "V3"]);

            IEnumerable[] columns;
            string[] columnNames;
            DataFrame df;
            createTestDataFrame(engine, out columns, out columnNames, out df);

            Assert.Equal("a", df[0, 0]);
            df[0, 0] = "b";
            Assert.Equal("b", df[0, 0]);
            df[0, 0] = "c";
            Assert.Equal(null, df[0, 0]);
            Assert.Equal("b", df[5, 0]);
            df[5, 0] = null;
            Assert.Equal(null, df[5, 0]);
        }

        private static void checkDataFrameContent(DataFrame df)
        {
            var cat = df[0].AsFactor().GetFactors();
            Assert.Equal("a", cat[0]);
            Assert.Equal("a", cat[4]);
            Assert.Equal("b", cat[5]);

            var numbers = df[1].AsInteger();
            for (int i = 0; i < 10; i++)
                Assert.Equal(i + 1, numbers[i]);

            var measures = df[2].AsNumeric();
            for (int i = 0; i < 10; i++)
                Assert.Equal(i + 1.1, measures[i]);
        }
    }
}