using Xunit;
using System.Linq;

namespace RDotNet
{
    public class DataFrameRowMappingTest : RDotNetTestFixture
    {
        [Fact]
        public void TestGetRow()
        {
            SetUpTest();
            var engine = this.Engine;
            var iris = engine.Evaluate("iris").AsDataFrame();
            var row = iris.GetRow<IrisData>(0);
            Assert.Equal(row.Species, (Iris.setosa));
            row = iris.GetRow<IrisData>(50);
            Assert.Equal(row.Species, (Iris.versicolor));
            row = iris.GetRow<IrisData>(100);
            Assert.Equal(row.Species, (Iris.virginica));
        }

        [Fact]
        public void TestGetRows()
        {
            SetUpTest();
            var engine = this.Engine;
            var iris = engine.Evaluate("iris").AsDataFrame();
            var counts = iris.GetRows<IrisData>().GroupBy(data => data.Species).Select(group => group.Count());
            Assert.Equal(counts, (new[] { 50, 50, 50 }));
        }

        // Was not behaving properly. Disabling the support for "dynamic" until investigation. Too dangerous.
        //[Fact]
        //public void TestDataFrameSubsetting()
        //{
        //    SetUpTest();
        //    var engine = this.Engine;
        //    dynamic iris = engine.Evaluate("iris").AsDataFrame();
        //    engine.Evaluate("iris50 <- iris[1:50,]");
        //    dynamic iris50 = engine.Evaluate("iris50").AsDataFrame();
        //    Assert.Equal(150, iris.RowCount);
        //    Assert.Equal(50, iris50.RowCount);
        //    var species = iris.Species;
        //    var species50 = iris50.Species;
        //    var sameRef = object.ReferenceEquals(species, species50);
        //    Assert.Equal(150, species.Length);
        //    Assert.Equal(50, species50.Length);
        //    Assert.Equal(iris50["Species"].Length, 50);
        //}

        private static dynamic GetSpecies(dynamic iris)
        {
            return iris.Species;
        }
    }

    public enum Iris
    {
        setosa = 1,
        versicolor = 2,
        virginica = 3,
    }

    [DataFrameRow]
    public class IrisData
    {
        [DataFrameColumn("Sepal.Length")]
        public double SepalLength { get; set; }

        [DataFrameColumn("Sepal.Width")]
        public double SepalWidth { get; set; }

        [DataFrameColumn("Petal.Length")]
        public double PetalLength { get; set; }

        [DataFrameColumn("Petal.Width")]
        public double PetalWidth { get; set; }

        [DataFrameColumn("Species")]
        public Iris Species { get; set; }
    }
}