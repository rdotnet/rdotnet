using Xunit;
using RDotNet.Internals;
using System;

namespace RDotNet
{
    public class EnvironmentTest : RDotNetTestFixture
    {
        [Fact]
        public void TestCreateEnvironment()
        {
            var engine = this.Engine;
            var newEnv = engine.CreateEnvironment(engine.BaseNamespace);
            Assert.Equal(newEnv.Type, (SymbolicExpressionType.Environment));
            Assert.Equal(newEnv.Parent.DangerousGetHandle(), (engine.BaseNamespace.DangerousGetHandle()));
        }

        [Fact]
        public void TestCreateIsolatedEnvironment()
        {
            var engine = this.Engine;
            var newEnv = engine.CreateIsolatedEnvironment();
            Assert.Equal(newEnv.Type, (SymbolicExpressionType.Environment));
            Assert.Null(newEnv.Parent);
        }

        [Fact]
        public void TestSetSymbols()
        {
            var engine = this.Engine;
            engine.Evaluate("rm(list=ls())");
            var env = engine.Evaluate("list2env(list(a = 1, b = 2:4, p = pi, ff = gl(3, 4, labels = LETTERS[1:3])))").AsEnvironment();
            Assert.Equal(0, engine.GlobalEnvironment.GetSymbolNames().Length);
            Assert.Equal(4, env.GetSymbolNames().Length);
            env.SetSymbol("b", engine.CreateIntegerVector(new int[] { 6, 7 }));
            env.SetSymbol("c", engine.CreateNumericVector(new double[] { Math.PI, Math.E }));
            Assert.Equal(0, engine.GlobalEnvironment.GetSymbolNames().Length);
            Assert.Equal(5, env.GetSymbolNames().Length);
            Assert.Equal(2, env.GetSymbol("b").AsInteger().Length);
            Assert.Equal(2, env.GetSymbol("c").AsNumeric().Length);

            //Defining symbols in the global environment does not affect the environment.
            engine.SetSymbol("b", engine.CreateIntegerVector(new int[] { 11 }));
            Assert.Equal(1, engine.GetSymbol("b").AsInteger().Length);
            Assert.Equal(1, engine.GlobalEnvironment.GetSymbol("b").AsInteger().Length);
            Assert.Equal(2, env.GetSymbol("b").AsInteger().Length);
            Assert.Equal(1, engine.GlobalEnvironment.GetSymbolNames().Length);

            engine.SetSymbol("d", engine.CreateIntegerVector(new int[] { 1, 2, 3, 4 }));
            Assert.Equal(4, engine.GetSymbol("d").AsInteger().Length);
            Assert.Equal(4, engine.GlobalEnvironment.GetSymbol("d").AsInteger().Length);
            Assert.Equal(2, engine.GlobalEnvironment.GetSymbolNames().Length);
            Assert.Equal(5, env.GetSymbolNames().Length);
        }
    }
}