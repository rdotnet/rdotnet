using NUnit.Framework;
using System.Linq;

namespace RDotNet
{
    internal class DataFrameRowMappingTest : RDotNetTestFixture
    {
        [Test]
        public void TestGetRow()
        {
            var engine = this.Engine;
            var iris = engine.Evaluate("iris").AsDataFrame();
            var row = iris.GetRow<IrisData>(0);
            Assert.That(row.Species, Is.EqualTo(Iris.setosa));
            row = iris.GetRow<IrisData>(50);
            Assert.That(row.Species, Is.EqualTo(Iris.versicolor));
            row = iris.GetRow<IrisData>(100);
            Assert.That(row.Species, Is.EqualTo(Iris.virginica));
        }

        [Test]
        public void TestGetRows()
        {
            var engine = this.Engine;
            var iris = engine.Evaluate("iris").AsDataFrame();
            var counts = iris.GetRows<IrisData>().GroupBy(data => data.Species).Select(group => group.Count());
            Assert.That(counts, Is.EquivalentTo(new[] { 50, 50, 50 }));
        }

        [Test]
        public void TestDataFrameSubsetting()
        {
            var engine = this.Engine;
            dynamic iris = engine.Evaluate("iris").AsDataFrame();
            dynamic iris50 = engine.Evaluate("iris[1:50,]").AsDataFrame();
            Assert.AreEqual(150, iris.RowCount);
            Assert.AreEqual(50, iris50.RowCount);
            var species50 = (DynamicVector)iris50.Species;
            var species = (DynamicVector)iris.Species;
            var sameRef = object.ReferenceEquals(species, species50);
            Assert.AreEqual(150, species.Length);
            Assert.AreEqual(50, species50.Length);
            Assert.AreEqual(iris50["Species"].Length, 50);
        }

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