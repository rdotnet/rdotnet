using NUnit.Framework;

namespace RDotNet
{
   class CharacterVectorTest : RDotNetTestFixture
   {
      [Test]
      public void TestCharacter()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         var vector = engine.Evaluate("x <- c('foo', NA, 'bar')").AsCharacter();
         Assert.That(vector.Length, Is.EqualTo(3));
         Assert.That(vector[0], Is.EqualTo("foo"));
         Assert.That(vector[1], Is.Null);
         Assert.That(vector[2], Is.EqualTo("bar"));
         vector[0] = null;
         Assert.That(vector[0], Is.Null);
         var logical = engine.Evaluate("is.na(x)").AsLogical();
         Assert.That(logical[0], Is.True);
         Assert.That(logical[1], Is.True);
         Assert.That(logical[2], Is.False);
      }
   }
}
