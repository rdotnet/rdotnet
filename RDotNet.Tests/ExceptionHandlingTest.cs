using NUnit.Framework;
using System;

namespace RDotNet
{
    public class ExceptionHandlingTest : RDotNetTestFixture
    {
        [Test]
        public void TestCharacter()
        {
            // Check that https://rdotnet.codeplex.com/workitem/70 does not occur; in particular worth testing on CentOS according to issue reporter.
            string v = null;
            Assert.Throws(typeof(NullReferenceException), () => { var t = v.ToString(); });            
        }

        [Test]
        public void TestFailedExpressionParsing()
        {
            // https://rdotnet.codeplex.com/workitem/77
            var engine = this.Engine;
            object expr = null;
            Assert.Throws<ParseException>(
                () => {
                    expr = engine.Evaluate("function(k) substitute(bar(x) = k)");
                },
                "Status Error for function(k) substitute(bar(x) = k)\n : unexpected '='"
                );
            Assert.IsNull(expr);
        }

        [Test]
        public void TestFailedExpressionEvaluation()
        {
            //> fail <- function(msg) {stop(paste( 'the message is', msg))}
            //> fail('bailing out')
            //Error in fail("bailing out") : the message is bailing out

            ReportFailOnLinux("https://rdotnet.codeplex.com/workitem/146");

            var engine = this.Engine;
            engine.Evaluate("fail <- function(msg) {stop(paste( 'the message is', msg))}");
            object expr = null;
            Assert.Throws<EvaluationException>(
                () => {
                    expr = engine.Evaluate("fail('bailing out')");
                },
                "Error in fail(\"bailing out\") : the message is bailing out\n"
                );
            Assert.IsNull(expr);
        }

        [Test]
        public void TestFailedExpressionUnboundSymbol()
        {
            var engine = this.Engine;
            ReportFailOnLinux("https://rdotnet.codeplex.com/workitem/146");
            Assert.Throws<EvaluationException>(
                () => {
                    var x = engine.GetSymbol("x");
                },
                "Error: object 'x' not found"
                );
        }

        [Test]
        public void TestFailedExpressionUnboundSymbolEvaluation()
        {
            ReportFailOnLinux("https://rdotnet.codeplex.com/workitem/146");
            var engine = this.Engine;
            Assert.Throws<EvaluationException>(
                () => {
                    var x = engine.Evaluate("x");
                },
                "Error: object 'x' not found\n"
                );
        }

        [Test]
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
            Assert.Throws<EvaluationException>(
                () => {
                    var x = engine.Evaluate("x");
                },
                "Error: object 'x' not found\n"
                );
        }
    }
}