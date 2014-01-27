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

      [Test, ExpectedExceptionAttribute(typeof(ParseException), ExpectedMessage = "Status Error for function(k) substitute(bar(x) = k)\n : unexpected '='")]
      public void TestFailedExpressionParsing()
      {
         // https://rdotnet.codeplex.com/workitem/77 
         var engine = REngine.GetInstanceFromID(EngineName);
         object expr = engine.Evaluate("function(k) substitute(bar(x) = k)");
         Assert.IsNull(expr);
      }

      [Test, ExpectedExceptionAttribute(typeof(EvaluationException), ExpectedMessage = "Error in fail(\"bailing out\") : the message is bailing out\n")]
      public void TestFailedExpressionEvaluation()
      {
         //> fail <- function(msg) {stop(paste( 'the message is', msg))} 
         //> fail('bailing out')
         //Error in fail("bailing out") : the message is bailing out
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("fail <- function(msg) {stop(paste( 'the message is', msg))}");
         object expr = engine.Evaluate("fail('bailing out')");
         Assert.IsNull(expr);
      }
   }
}
