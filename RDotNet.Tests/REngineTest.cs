using Xunit;
using System;
using System.Threading;

namespace RDotNet
{
    public class REngineTest : RDotNetTestFixture
    {
        [Fact]
        public void TestCStackCheckDisabled()
        {
            SetUpTest();
            var engine = this.Engine;
            var cStackLimit = engine.GetInt32("R_CStackLimit");
            Assert.Equal(-1, cStackLimit);
        }

        [Fact]
        public void TestSetCommandLineArguments()
        {
            SetUpTest();
            var engine = this.Engine;
            engine.SetCommandLineArguments(new[] { "Hello", "World" });
            Assert.Equal(engine.Evaluate("commandArgs()").AsCharacter(), (new[] { REngine.EngineName, "Hello", "World" }));
        }

        [Fact]
        public void TestDefaultCommandLineArgs()
        {
            SetUpTest();
            var engine = this.Engine;
            var cmdArgs = engine.Evaluate("commandArgs()").AsCharacter();
        }

        [Fact]
        public void TestGlobalEnvironment()
        {
            SetUpTest();
            var engine = this.Engine;
            Assert.Equal(engine.GlobalEnvironment.DangerousGetHandle(), (engine.Evaluate(".GlobalEnv").DangerousGetHandle()));
        }

        [Fact]
        public void TestBaseNamespace()
        {
            SetUpTest();
            var engine = this.Engine;
            Assert.Equal(engine.BaseNamespace.DangerousGetHandle(), (engine.Evaluate(".BaseNamespaceEnv").DangerousGetHandle()));
        }

        [Fact]
        public void TestNilValue()
        {
            SetUpTest();
            var engine = this.Engine;
            Assert.Equal(engine.NilValue.DangerousGetHandle(), (engine.Evaluate("NULL").DangerousGetHandle()));
        }

        // Note: unfortunately the results of this test remain unpredictable from run to run, if run from NUnit.
        [Fact(Skip = "This test still seems to not be repeatable in behavior nor suceeding in a unit test context. Baffled.")]
        public void TestRGarbageCollectNumericVectors()
        {
            /*
            gc()
            gc()
            memory.size()
            ## [1] 24.7
            x <- numeric(5e6)
            object.size(x)
            40000040 bytes
            memory.size()
            ## [1] 64.11
            gc()
            memory.size()
            ## [1] 62.89
            rm(x)
            gc()
            memory.size()
            ## [1] 24.7
            */
            var statementCreateX = "x <- numeric(5e6)";
            // For some reasons the delta is not 40MB spot on. Use 35 MB as a threshold
            double expectedMinMegaBytesDifference = 35.0;

            Func<SymbolicExpression, NumericVector> coercionFun = SymbolicExpressionExtension.AsNumeric;
            CheckProperMemoryReclaimR(statementCreateX, expectedMinMegaBytesDifference, coercionFun);
        }

        // Note: unfortunately the results of this test remain unpredictable from run to run, if run from NUnit.
        [Fact(Skip = "This test still seems to not be repeatable in behavior nor suceeding in a unit test context. Baffled.")]
        public void TestRGarbageCollectCharacterVectors()
        {
            /*
            rm(list=ls())
            gc()
            gc()
            memory.size()
            ## [1] 10.86 from scratch, [1] 18.62 subsequent
            #x <- rep('abcd', 1e6)
            ltrs <- letters[1:26] ; x <- expand.grid(ltrs,ltrs,ltrs,ltrs,ltrs[1:5]) ; rm(ltrs) ; x <- rep(paste0(x[,1], x[,2], x[,3], x[,4], x[,5]), 1)
            object.size(x)
            ## 82255704  bytes
            memory.size()
            ## [1] 235.95
            gc()
            gc()
            memory.size()
            ## [1] 97.11
            rm(x)
            gc()
            gc()
            memory.size()
            ## [1] 18.62
            */
            var statementCreateX = "ltrs <- letters[1:26] ; x <- expand.grid(ltrs,ltrs,ltrs,ltrs,ltrs[1:5]) ; rm(ltrs) ; x <- rep(paste0(x[,1], x[,2], x[,3], x[,4], x[,5]), 1)";
            double expectedMinMegaBytesDifference = 75.0;

            Func<SymbolicExpression, CharacterVector> coercionFun = SymbolicExpressionExtension.AsCharacter;
            CheckProperMemoryReclaimR(statementCreateX, expectedMinMegaBytesDifference, coercionFun);
        }

        private void CheckProperMemoryReclaimR<T>(string statementCreateX, double expectedMinMegaBytesDifference, Func<SymbolicExpression, T> coercionFun) where T : SymbolicExpression
        {
            var engine = this.Engine;
            engine.Evaluate("if (exists('x')) {rm(x)}");
            Thread.Sleep(100);
            var memoryInitial = GetBaselineRengineMemory(engine);
            engine.Evaluate(statementCreateX);
            T sexp = coercionFun(engine.GetSymbol("x"));
            var memoryAfterAlloc = GetBaselineRengineMemory(engine);
            Assert.True(memoryAfterAlloc - memoryInitial > (expectedMinMegaBytesDifference));
            engine.Evaluate("rm(x)");
            // We still have a reference from .NET, the variable sexp. Should not have been collected yet.
            var memoryAfterRemoveRvar = GetBaselineRengineMemory(engine);
            Assert.True(memoryAfterRemoveRvar - memoryInitial > (expectedMinMegaBytesDifference));
            sexp = null;
            Thread.Sleep(100);
            var memoryAfterGC = GetBaselineRengineMemory(engine);
            Assert.True(memoryAfterAlloc - memoryAfterGC > (expectedMinMegaBytesDifference));  // x should be collected.
        }

        private static double GetBaselineDotnetMemory(REngine engine)
        {
            GarbageCollectRandClr(engine);
            return GC.GetTotalMemory(false);
        }

        private static double GetBaselineRengineMemory(REngine engine)
        {
            GarbageCollectRandClr(engine);
            var tmp = GetRMemorySize(engine);
            Thread.Sleep(100);
            GarbageCollectRandClr(engine);
            return GetRMemorySize(engine);
        }

        [Fact(Skip = "This test still seems to not be repeatable in behavior nor suceeding in a unit test context. Baffled.")]
        public void TestCharacterVectorToStringMemReclaim()
        {
            var statementCreateX = "x <- format(1:1000000)";
            double expectedMinBytesDifference = 2.5e7;  // (20 bytes + 2*6) * 1e6 = 3.2e7 bytes ~ 32 MB

            var engine = this.Engine;
            var memoryInitial = GetBaselineDotnetMemory(engine);
            engine.Evaluate(statementCreateX);
            var strArray = engine.GetSymbol("x").AsCharacter().ToArray();
            engine.Evaluate("rm(x)");
            var memoryAfterAlloc = GetBaselineDotnetMemory(engine);
            Assert.True(memoryAfterAlloc - memoryInitial > (expectedMinBytesDifference));
            strArray = null;
            var memoryAfterGC = GetBaselineDotnetMemory(engine);
            Assert.True(memoryAfterAlloc - memoryAfterGC > (expectedMinBytesDifference));  // x should be collected.
        }

        [Fact]
        public void TestParseCodeLine()
        {
            SetUpTest();
            var engine = this.Engine;
            engine.Evaluate("cat('hello')");
            Assert.Equal(Device.GetString(), ("hello"));
        }

        [Fact]
        public void TestParseCodeBlock()
        {
            SetUpTest();
            var engine = this.Engine;
            engine.Evaluate("for(i in 1:3){\ncat(i)\ncat(i)\n}");
            Assert.Equal(Device.GetString(), ("112233"));
        }

        [Fact]
        public void TestParseCodeBlockMultiLine()
        {
            SetUpTest();
            // Tests suggested by the following issue, but not dealing with it per se.
            // https://rdotnet.codeplex.com/workitem/165
            var engine = this.Engine;
            engine.Evaluate(@"for(i in 1:3){
cat(i)
cat(i)
}");
            Assert.Equal(Device.GetString(), ("112233"));

            Device.Initialize();
            engine.Evaluate(@"for(i in 1:3){
cat(i); cat(i)
}");
            Assert.Equal(Device.GetString(), ("112233"));
        }

        [Fact]
        public void TestParseComments()
        {
            SetUpTest();
            // See
            // https://rdotnet.codeplex.com/workitem/165
            var engine = this.Engine;
            engine.Evaluate(@"for(i in 1:3){
cat(i) ; # cat(i) ; cat(i)
}");
            Assert.Equal(Device.GetString(), ("123"));

            Device.Initialize();
            engine.Evaluate(@"cat('Hi') ; # cat(' there') ; cat(' How\'s it going?')");
            Assert.Equal(Device.GetString(), ("Hi"));

            Device.Initialize();
            engine.Evaluate("cat(\"Hello!\\n\"); #cat(\"Glad to see you today.\\n\"); cat(\"Goodbye.\\n\")");
            Assert.Equal(Device.GetString(), ("Hello!\n"));

            Device.Initialize();
            engine.Evaluate("cat(\"Hello!\\n\"); #cat(\"Glad to see you today.\\n\");\n cat(\"Goodbye.\\n\")");
            Assert.Equal(Device.GetString(), ("Hello!\nGoodbye.\n"));

            Device.Initialize();
            engine.Evaluate("cat('Hello!\\n'); #cat('Glad to see you today.\\n');\n cat('Goodbye.\\n')");
            Assert.Equal(Device.GetString(), ("Hello!\nGoodbye.\n"));

            Device.Initialize();
            engine.Evaluate(@"f <- function() {
cat('function f ') # some comments
cat(paste('a','b',  # some more comments there
sep=''))
}");
            engine.Evaluate(@"f()");
            Assert.Equal(Device.GetString(), ("function f ab"));
        }

        [Fact]
        public void TestParseLineWithStringWithHash()
        {
            SetUpTest();
            //https://github.com/jmp75/rdotnet/issues/14
            var engine = this.Engine;

            Device.Initialize();
            engine.Evaluate(@"cat('This is a string with a (#) character') # ; blah ; cat(' this is removed')");
            Assert.Equal(Device.GetString(), ("This is a string with a (#) character"));

            Device.Initialize();
            engine.Evaluate("cat('This is a string with a (#) character') ; # cat(' this is removed') \n cat(' but it did not remove this line')");
            Assert.Equal(Device.GetString(), ("This is a string with a (#) character but it did not remove this line"));

            Device.Initialize();
            engine.Evaluate("cat('This is a string with a (#) character') ; # cat(' this is removed') \n\r cat(' but it did not remove this line')");
            Assert.Equal(Device.GetString(), ("This is a string with a (#) character but it did not remove this line"));

            Device.Initialize();
            engine.Evaluate(@"cat('This is a string with a \'#\' character') # ; cat(' this is removed')");
            Assert.Equal(Device.GetString(), ("This is a string with a '#' character"));

            Device.Initialize();
            engine.Evaluate("cat('This is a string with a \\\"#\\\" character') # ; cat(' this is removed')");
            Assert.Equal(Device.GetString(), ("This is a string with a \"#\" character"));

            Device.Initialize();
            engine.Evaluate("cat(\"This is a string with a \\\"#\\\" character\") # ; cat(' this is removed')");
            Assert.Equal(Device.GetString(), ("This is a string with a \"#\" character"));

            Device.Initialize();
            engine.Evaluate("cat('first statement') ; cat(\" then this is a string with a \\\"#\\\" character\") # ; cat(' this is removed')");
            Assert.Equal(Device.GetString(), ("first statement then this is a string with a \"#\" character"));

            Device.Initialize();
            engine.Evaluate("cat('first statement') ; cat(\" then this is a string with a \\\"#\\\" character\") # ; cat(' this is removed')");
            Assert.Equal(Device.GetString(), ("first statement then this is a string with a \"#\" character"));

            Device.Initialize();
            engine.Evaluate(@"cat('single quote delimiter (\') with # and "" and \' ') # ; cat(' this # is removed')");
            Assert.Equal(Device.GetString(), (@"single quote delimiter (') with # and "" and ' "));

            Device.Initialize();
            engine.Evaluate("cat(\"double quote delimiter (\\\") with # and \\\" and \' \") # ; cat(' this # is removed')");
            Assert.Equal(Device.GetString(), (@"double quote delimiter ("") with # and "" and ' "));

            Device.Initialize();
            engine.Evaluate("cat('### Some markdown with multiple hashtags') # ; cat(' this # is removed')");
            Assert.Equal(Device.GetString(), (@"### Some markdown with multiple hashtags"));

            Device.Initialize();
            engine.Evaluate(@"cat(""#\' Some Roxygen"") # ; cat(' this # is removed')");
            Assert.Equal(Device.GetString(), (@"#' Some Roxygen"));

            Device.Initialize();
            engine.Evaluate("cat('#\\\' Some Roxygen') # ; cat(' this # is removed')");
            Assert.Equal(Device.GetString(), (@"#' Some Roxygen"));

            /* 
             * TODO:
            Device.Initialize();
            engine.Evaluate(@"cat('Some
multiline with # kept 
string') # ; cat(' this # is removed')");
            Assert.Equal(Device.GetString(), ("Some\nmultiline with # kept \nstring"));
            */
        }

        [Fact]
        public void TestProcessingMultipleHashes()
        {
            SetUpTest();
            // TODO?
            // paste('this contains ### characters', " this too ###", 'Oh, and this # one too') # but "this" 'rest' is commented

            //            var blah = @"blah = 'blah
            //\'blah\'
            //blah";
            //            blah = @"blah = 'blah\n\'blah\'\nblah";

        }

        [Fact]
        public void TestReadConsole()
        {
            SetUpTest();
            var engine = this.Engine;
            string additionalMsg = "https://rdotnet.codeplex.com/workitem/146";
            ReportFailOnLinux(additionalMsg);
            Device.Input = "Hello, World!";
            Assert.Equal(engine.Evaluate("readline()").AsCharacter()[0], (Device.Input));
        }

        [Fact]
        public void TestWriteConsole()
        {
            SetUpTest();
            var engine = this.Engine;
            engine.Evaluate("print(NULL)");
            Assert.Equal(Device.GetString(), ("NULL\n"));
        }

        [Fact]
        public void TestCallingTwice()
        {
            SetUpTest();
            var engine = this.Engine;
            engine.Evaluate("a <- 1");
            engine.Evaluate("a <- a+1");
            NumericVector v1 = engine.GetSymbol("a").AsNumeric();
            Assert.Equal(2.0, v1[0]);
            engine.Evaluate("a <- a+1");
            NumericVector v2 = engine.GetSymbol("a").AsNumeric();
            Assert.Equal(3.0, v2[0]);
        }
    }
}