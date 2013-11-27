using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace RDotNet
{
   public class ExceptionHandlingTest : RDotNetTestFixture
   {
      [Test, ExpectedException(typeof(NullReferenceException))]
      public void TestCharacter()
      {
         // Check that https://rdotnet.codeplex.com/workitem/70 does not occur; in particular worth testing on CentOS according to issue reporter.
         string v = null;
         var t = v.ToString();
      }
   }
}
