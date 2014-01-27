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

      [Test, ExpectedExceptionAttribute(typeof(EvaluationException), ExpectedMessage = "Error: object 'x' not found")]
      public void TestFailedExpressionUnboundSymbol()
      {
          var engine = REngine.GetInstanceFromID(EngineName);
          var x = engine.GetSymbol("x");
      }

      [Test, ExpectedExceptionAttribute(typeof(EvaluationException), ExpectedMessage = "Error: object 'x' not found\n")]
      public void TestFailedExpressionUnboundSymbolEvaluation()
      {
          var engine = REngine.GetInstanceFromID(EngineName);
          var x = engine.Evaluate("x");
      }

      [Test, ExpectedExceptionAttribute(typeof(EvaluationException), ExpectedMessage = "Error: object 'x' not found\n")]
      public void TestFailedExpressionParsingMissingParenthesis()
      {
          //> x <- rep(c(TRUE,FALSE), 55
          //+ 
          //+ x
          //Error: unexpected symbol in:
          //"
          //x"
          //> 
          var engine = REngine.GetInstanceFromID(EngineName);
          var expr = engine.Evaluate("x <- rep(c(TRUE,FALSE), 55");
          var x = engine.Evaluate("x");
      }
   }
}
