using NUnit.Framework;
using System;

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
            var engine = this.Engine;
            object expr = engine.Evaluate("function(k) substitute(bar(x) = k)");
            Assert.IsNull(expr);
        }

        [Test, ExpectedExceptionAttribute(typeof(EvaluationException), ExpectedMessage = "Error in fail(\"bailing out\") : the message is bailing out\n")]
        public void TestFailedExpressionEvaluation()
        {
            //> fail <- function(msg) {stop(paste( 'the message is', msg))}
            //> fail('bailing out')
            //Error in fail("bailing out") : the message is bailing out

            ReportFailOnLinux("https://rdotnet.codeplex.com/workitem/146");

            var engine = this.Engine;
            engine.Evaluate("fail <- function(msg) {stop(paste( 'the message is', msg))}");
            object expr = engine.Evaluate("fail('bailing out')");
            Assert.IsNull(expr);
        }

        [Test, ExpectedExceptionAttribute(typeof(EvaluationException), ExpectedMessage = "Error: object 'x' not found")]
        public void TestFailedExpressionUnboundSymbol()
        {
            var engine = this.Engine;
            ReportFailOnLinux("https://rdotnet.codeplex.com/workitem/146");
            var x = engine.GetSymbol("x");
        }

        [Test, ExpectedExceptionAttribute(typeof(EvaluationException), ExpectedMessage = "Error: object 'x' not found\n")]
        public void TestFailedExpressionUnboundSymbolEvaluation()
        {
            ReportFailOnLinux("https://rdotnet.codeplex.com/workitem/146");
            var engine = this.Engine;
            var x = engine.Evaluate("x");
        }

        [Test, ExpectedExceptionAttribute(typeof(EvaluationException), ExpectedMessage = "Error: object 'x' not found\n")]
        public void TestFailedExpressionParsingMissingParenthesis()
        {
            ReportFailOnLinux("https://rdotnet.codeplex.com/workitem/146");
            //> x <- rep(c(TRUE,FALSE), 55
            //+
            //+ x
            //Error: unexpected symbol in:
            //"
            //x"
            //>
            var engine = this.Engine;
            var expr = engine.Evaluate("x <- rep(c(TRUE,FALSE), 55");
            var x = engine.Evaluate("x");
        }
    }
}