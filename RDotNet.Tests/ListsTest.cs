using NUnit.Framework;

namespace RDotNet
{
   public class ListsTest : RDotNetTestFixture
   {
      [Test]
      public void TestIsList()
      {
         //https://rdotnet.codeplex.com/workitem/81         
         var engine = REngine.GetInstanceFromID(EngineName);
         var pairList = engine.Evaluate("pairlist(a=5)");
         var aList = engine.Evaluate("list(a=5)");
         bool b = aList.AsList().IsList();
         Assert.AreEqual(true, pairList.IsList());
         Assert.AreEqual(true, aList.IsList());
      }

      [Test]
      public void TestListSubsetting()
      {
         //https://rdotnet.codeplex.com/workitem/81         
         var engine = REngine.GetInstanceFromID(EngineName);
         var numlist = engine.Evaluate("c(1.5, 2.5)").AsList();
         var numListString = numlist.ToString();
         var element = numlist[1];
      }
   }
}