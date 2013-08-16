using System.Linq;
using NUnit.Framework;

namespace RDotNet.Tests
{
   [TestFixture]
   class DataFrameRowMappingTest
   {
      private const string EngineName = "RDotNetTest";
      private readonly MockDevice device = new MockDevice();

      [TestFixtureSetUp]
      public void SetUpEngine()
      {
         Helper.SetEnvironmentVariables();
         var engine = REngine.CreateInstance(EngineName);
         engine.Initialize(device: device);
      }

      [TestFixtureTearDown]
      public void DisposeEngine()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         if (engine != null)
         {
            engine.Dispose();
         }
      }

      [TearDown]
      public void TearDown()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("rm(list=ls())");
         this.device.Initialize();
      }

      [Test]
      public void TestGetRow()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
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
         var engine = REngine.GetInstanceFromID(EngineName);
         var iris = engine.Evaluate("iris").AsDataFrame();
         var counts = iris.GetRows<IrisData>().GroupBy(data => data.Species).Select(group => group.Count());
         Assert.That(counts, Is.EquivalentTo(new[] { 50, 50, 50 }));
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
