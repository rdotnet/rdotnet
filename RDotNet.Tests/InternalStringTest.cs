using NUnit.Framework;

namespace RDotNet
{
   internal class InternalStringTest : RDotNetTestFixture
   {
      [Test]
      public void TestCharacter()
      {
         var engine = this.Engine;
         var vector = engine.Evaluate("c('foo', NA, 'bar')").AsCharacter();
         Assert.That(vector.Length, Is.EqualTo(3));
         Assert.That(vector[0], Is.EqualTo("foo"));
         Assert.That(vector[1], Is.Null);
         Assert.That(vector[2], Is.EqualTo("bar"));
      }
   }
}