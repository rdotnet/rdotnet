using Xunit;
using System;
using System.Numerics;

namespace RDotNet
{
    /// <summary>
    /// A suite of tests for checking that data is converted as expected from R to .NET
    /// </summary>
    public class DataConversionTest : RDotNetTestFixture
    {
        [Fact]
        public void TestCreateNumericVectorValid()
        {
            var engine = this.Engine;
            engine.Evaluate("x <- 1:100 * 1.1");
            var expected = ArrayMult(GenArrayDouble(1, 100), 1.1);
            var vec = engine.GetSymbol("x").AsNumeric();
            CheckBothArrayConversions(vec, expected);

            vec = engine.Evaluate("c(1.1,NA,2.2)").AsNumeric();
            CheckBothArrayConversions(vec, new[] { 1.1, double.NaN, 2.2 });

            // Test a large data set: I just cannot believe how faster things are...
            engine.Evaluate("x <- 1:1e7 * 1.1");
            var a = engine.GetSymbol("x").AsNumeric().ToArray();
            Assert.Equal(a[10000000 / 2 - 1], 1.1 * 1e7 / 2);
        }

        [Fact]
        public void TestCreateIntegerVectorValid()
        {
            var engine = this.Engine;
            engine.Evaluate("x <- 1:100");
            var expected = GenArrayInteger(1, 100);
            var vec = engine.GetSymbol("x").AsInteger();
            CheckBothArrayConversions(vec, expected);
            vec = engine.Evaluate("as.integer(c(1,NA,2))").AsInteger();
            CheckBothArrayConversions(vec, new[] { 1, Int32.MinValue, 2 });
        }

        [Fact]
        public void TestCreateLogicalVectorValid()
        {
            var engine = this.Engine;
            engine.Evaluate("x <- rep(c(TRUE,FALSE),50)");
            var expected = Array.ConvertAll(GenArrayInteger(1, 100), val => val % 2 == 1);
            var vec = engine.GetSymbol("x").AsLogical();
            CheckBothArrayConversions(vec, expected);
            vec = engine.Evaluate("c(TRUE,NA,FALSE)").AsLogical();
            CheckBothArrayConversions(vec, new[] { true, true, false });
        }

        [Fact]
        public void TestCreateCharacterVectorValid()
        {
            var engine = this.Engine;
            engine.Evaluate("x <- rep(c('a','bb'),50)");
            string[] expected = new string[100];
            for (int i = 0; i < 100; i++)
                expected[i] = (i % 2 == 0) ? "a" : "bb";
            var vec = engine.GetSymbol("x").AsCharacter();
            CheckBothArrayConversions(vec, expected);
            // NA members is already tested in another test class
        }

        [Fact]
        public void TestCreateComplexVectorValid()
        {
            var engine = this.Engine;
            engine.Evaluate("x <- 1:100 + 1i*(101:200)");
            var expected = new Complex[100];
            for (int i = 0; i < 100; i++)
                expected[i] = new Complex(i + 1, i + 101);
            var vec = engine.GetSymbol("x").AsComplex();
            CheckBothArrayConversions(vec, expected);
            vec = engine.Evaluate("c(1+2i,NA,3+4i)").AsComplex();
            CheckBothArrayConversions(vec, new[] { new Complex(1, 2), new Complex(double.NaN, double.NaN), new Complex(3, 4) });
        }

        private static void CheckBothArrayConversions<T>(Vector<T> vec, T[] expected)
        {
            var a = vec.ToArray();
            CheckArrayEqual(a, expected);
        }

        [Fact]
        public void TestCreateNumericMatrixValid()
        {
            var engine = this.Engine;
            engine.Evaluate("x <- matrix(1:110 * 1.1, nrow=10, ncol=11)");
            var expected = ToMatrix(ArrayMult(GenArrayDouble(1, 110), 1.1), 10, 11);
            var a = engine.GetSymbol("x").AsNumericMatrix().ToArray();
            CheckArrayEqual(a, expected);
        }

        [Fact]
        public void TestCreateIntegerMatrixValid()
        {
            var engine = this.Engine;
            engine.Evaluate("x <- matrix(as.integer(1:110), nrow=10, ncol=11)");
            var expected = ToMatrix(GenArrayInteger(1, 110), 10, 11);
            var a = engine.GetSymbol("x").AsIntegerMatrix().ToArray();
            CheckArrayEqual(a, expected);
        }

        [Fact]
        public void TestCreateLogicalMatrixValid()
        {
            var engine = this.Engine;
            engine.Evaluate("x <- matrix(rep(c(TRUE,FALSE), 55), nrow=10, ncol=11)");
            var exp_one = Array.ConvertAll(GenArrayInteger(1, 110), val => val % 2 == 1);
            var expected = ToMatrix(exp_one, 10, 11);
            var a = engine.GetSymbol("x").AsLogicalMatrix().ToArray();
            CheckArrayEqual(a, expected);
        }

        [Fact]
        public void TestCreateComplexMatrixValid()
        {
            var engine = this.Engine;
            engine.Evaluate("x <- matrix((1:110 + 1i*(101:210)), nrow=10, ncol=11)");
            var exp_one = Array.ConvertAll(GenArrayInteger(1, 110), val => new Complex(val, val + 100));
            var expected = ToMatrix(exp_one, 10, 11);
            var a = engine.GetSymbol("x").AsComplexMatrix().ToArray();
            CheckArrayEqual(a, expected);
        }
    }
}