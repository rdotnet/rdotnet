using Xunit;
using System;

namespace RDotNet
{
    public class ExceptionHandlingTest : RDotNetTestFixture
    {
        [Fact]
        public void TestCharacter()
        {
            SetUpTest();
            // Check that https://rdotnet.codeplex.com/workitem/70 does not occur; in particular worth testing on CentOS according to issue reporter.
            string v = null;
            Assert.Throws(typeof(NullReferenceException), () => { var t = v.ToString(); });            
        }

        [Fact]
        public void TestFailedExpressionParsing()
        {
            SetUpTest();
            // https://rdotnet.codeplex.com/workitem/77
            var engine = this.Engine;
            object expr = null;
            AssertThrows<ParseException>(
                () => {
                    expr = engine.Evaluate("function(k) substitute(bar(x) = k)");
                },
                "Status Error for function(k) substitute(bar(x) = k)\n : unexpected '='"
                );
            Assert.Null(expr);
        }

        [Fact]
        public void TestFailedExpressionEvaluation()
        {
            SetUpTest();
            //> fail <- function(msg) {stop(paste( 'the message is', msg))}
            //> fail('bailing out')
            //Error in fail("bailing out") : the message is bailing out

            ReportFailOnLinux("https://rdotnet.codeplex.com/workitem/146");

            var engine = this.Engine;
            engine.Evaluate("fail <- function(msg) {stop(paste( 'the message is', msg))}");
            object expr = null;
            AssertThrows<EvaluationException>(
                () => {
                    expr = engine.Evaluate("fail('bailing out')");
                },
                "Error in fail(\"bailing out\") : the message is bailing out\n"
                );
            Assert.Null(expr);
        }

        [Fact]
        public void TestFailedExpressionUnboundSymbol()
        {
            SetUpTest();
            var engine = this.Engine;
            ReportFailOnLinux("https://rdotnet.codeplex.com/workitem/146");
            AssertThrows<EvaluationException>(
                () => {
                    var x = engine.GetSymbol("x");
                },
                "Error: object 'x' not found"
                );
        }

        [Fact]
        public void TestFailedExpressionUnboundSymbolEvaluation()
        {
            SetUpTest();
            ReportFailOnLinux("https://rdotnet.codeplex.com/workitem/146");
            var engine = this.Engine;
            AssertThrows<EvaluationException>(
                () => {
                    var x = engine.Evaluate("x");
                },
                "Error: object 'x' not found\n"
                );
        }

        [Fact]
        public void TestFailedExpressionParsingMissingParenthesis()
        {
            SetUpTest();
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
            AssertThrows<EvaluationException>(
                () => {
                    var x = engine.Evaluate("x");
                },
                "Error: object 'x' not found\n"
                );
        }
    }
}