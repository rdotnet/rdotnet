using NUnit.Framework;
using System.Linq;

namespace RDotNet
{
   internal class FactorTest : RDotNetTestFixture
   {
      [Test]
      public void TestLength()
      {
         var engine = this.Engine;
         var factor = engine.Evaluate("factor(c('A', 'B', 'A', 'C', 'B'))").AsFactor();
         Assert.That(factor.Length, Is.EqualTo(5));
      }

      [Test]
      public void TestIsOrderedTrue()
      {
         var engine = this.Engine;
         var factor = engine.Evaluate("factor(c('A', 'B', 'A', 'C', 'B'), ordered=TRUE)").AsFactor();
         Assert.That(factor.IsOrdered, Is.True);
      }

      [Test]
      public void TestMissingValues()
      {
         var engine = this.Engine;
         var factor = engine.Evaluate("x <- factor(c('A', 'B', 'A', NA, 'C', 'B'), ordered=TRUE)").AsFactor();
         Assert.That(factor.GetFactors(), Is.EquivalentTo(new[] { "A", "B", "A", null, "C", "B" }));
         factor = engine.Evaluate(@"
levels(x) <- c('1st', '2nd', '3rd')
x
").AsFactor();
         Assert.That(factor.GetFactors(), Is.EquivalentTo(new[] { "1st", "2nd", "1st", null, "3rd", "2nd" }));
      }

      [Test]
      public void TestIsOrderedFalse()
      {
         var engine = this.Engine;
         var factor = engine.Evaluate("factor(c('A', 'B', 'A', 'C', 'B'), ordered=FALSE)").AsFactor();
         Assert.That(factor.IsOrdered, Is.False);
      }

      [Test]
      public void TestGetLevels()
      {
         var engine = this.Engine;
         var factor = engine.Evaluate("x <- factor(c('A', 'B', 'A', 'C', 'B'))").AsFactor();
         Assert.That(factor.GetLevels(), Is.EquivalentTo(new[] { "A", "B", "C" }));
         factor = engine.Evaluate(@"
levels(x) <- c('1st', '2nd', '3rd')
x
").AsFactor();
         Assert.That(factor.GetLevels(), Is.EquivalentTo(new[] { "1st", "2nd", "3rd" }));
      }

      [Test]
      public void TestGetFactors()
      {
         var engine = this.Engine;
         var factor = engine.Evaluate("x <- factor(c('A', 'B', 'A', 'C', 'B'))").AsFactor();
         Assert.That(factor.GetFactors(), Is.EquivalentTo(new[] { "A", "B", "A", "C", "B" }));
         factor = engine.Evaluate(@"
levels(x) <- c('1st', '2nd', '3rd')
x
").AsFactor();
         Assert.That(factor.GetFactors(), Is.EquivalentTo(new[] { "1st", "2nd", "1st", "3rd", "2nd" }));
      }

      [Test]
      public void TestGetFactorsEnum()
      {
         var engine = this.Engine;
         var code = "factor(c(rep('T', 5), rep('C', 5), rep('T', 4), rep('C', 5)), levels=c('T', 'C'), labels=c('Treatment', 'Control'))";
         var factor = engine.Evaluate(code).AsFactor();
         var expected = Enumerable.Repeat(Group.Treatment, 5)
                                  .Concat(Enumerable.Repeat(Group.Control, 5))
                                  .Concat(Enumerable.Repeat(Group.Treatment, 4))
                                  .Concat(Enumerable.Repeat(Group.Control, 5));
         Assert.That(factor.GetFactors<Group>(), Is.EquivalentTo(expected));
      }
   }

   public enum Group
   {
      Treatment = 1,
      Control = 2,
   }
}