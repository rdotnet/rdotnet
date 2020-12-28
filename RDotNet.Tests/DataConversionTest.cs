using System.Linq;
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
            SetUpTest();
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
        public void TestCreateIntegerVectorValid_Sequential()
        {
            SetUpTest();
            var engine = this.Engine;

            // Test a short and a long sequence - these will be represented using ALTREP
            engine.Evaluate("x <- 1:100");
            var expected = GenArrayInteger(1, 100);
            var vec = engine.GetSymbol("x").AsInteger();
            if (vec.ToArray()[0] == 0)
            {
                Console.WriteLine("FAIL");
            }
            CheckBothArrayConversions(vec, expected);

            engine.Evaluate("x <- 10000:1000000");
            expected = GenArrayInteger(10000, 1000000);
            vec = engine.GetSymbol("x").AsInteger();
            if (vec.ToArray()[0] == 0)
            {
                Console.WriteLine("FAIL");
            }
            CheckBothArrayConversions(vec, expected);
        }

        [Fact]
        public void TestCreateIntegerVectorValid_Indexed()
        {
            var engine = this.Engine;
            // Test vectors with non-sequential values.  This should not go through ALTREP.
            engine.Evaluate("y <- c(10, 5, 73, 8)");
            var vec = engine.GetSymbol("y").AsInteger();
            CheckBothArrayConversions(vec, new[] { 10, 5, 73, 8 });

            vec = engine.Evaluate("as.integer(c(1,NA,2))").AsInteger();
            CheckBothArrayConversions(vec, new[] { 1, Int32.MinValue, 2 });
        }

        [Fact]
        public void TestCoerceIntegerVectorAsCharacter()
        {
            var engine = this.Engine;
            engine.Evaluate("y <- as.integer(c(10, 5, 73, NA, 8))");
            var vec = engine.GetSymbol("y").AsCharacter();
            CheckBothArrayConversions(vec, new[] {"10", "5", "73", null, "8" });

            engine.Evaluate("x <- 10000:1000000");
            var expected = GenArrayCharacter(10000, 1000000);
            vec = engine.GetSymbol("x").AsCharacter();
            CheckBothArrayConversions(vec, expected);
        }

        [Fact]
        public void TestCoerceLogicalVectorAsCharacter()
        {
            var engine = this.Engine;
            engine.Evaluate("y <- as.logical(c(FALSE, NA, TRUE, NA, FALSE))");
            var vec = engine.GetSymbol("y").AsCharacter();
            CheckBothArrayConversions(vec, new[] { "FALSE", null, "TRUE", null, "FALSE" });
        }

        [Fact]
        public void TestCoerceNumericVectorAsCharacter()
        {
            var engine = this.Engine;
            engine.Evaluate("y <- as.numeric(c(NA, 1.1, 2.2, 3.333, 4.4))");
            var vec = engine.GetSymbol("y").AsCharacter();
            CheckBothArrayConversions(vec, new[] { null, "1.1", "2.2", "3.333", "4.4" });
        }

        [Fact]
        public void TestCreateLogicalVectorValid()
        {
            SetUpTest();
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
            SetUpTest();
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
            SetUpTest();
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
            SetUpTest();
            var engine = this.Engine;
            engine.Evaluate("x <- matrix(1:110 * 1.1, nrow=10, ncol=11)");
            var expected = ToMatrix(ArrayMult(GenArrayDouble(1, 110), 1.1), 10, 11);
            var a = engine.GetSymbol("x").AsNumericMatrix().ToArray();
            CheckArrayEqual(a, expected);
        }

        [Fact]
        public void TestCreateIntegerMatrixValid()
        {
            SetUpTest();
            var engine = this.Engine;
            engine.Evaluate("x <- matrix(as.integer(1:110), nrow=10, ncol=11)");
            var expected = ToMatrix(GenArrayInteger(1, 110), 10, 11);
            var a = engine.GetSymbol("x").AsIntegerMatrix().ToArray();
            CheckArrayEqual(a, expected);
        }

        [Fact]
        public void TestCoerceIntegerMatrixAsCharacterValid()
        {
            var engine = this.Engine;
            engine.Evaluate("x <- matrix(as.integer(1:110), nrow=10, ncol=11)");
            var a = engine.GetSymbol("x").AsCharacterMatrix();
            var expected = ToMatrix(GenArrayCharacter(1, 110), 10, 11);
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 11; col++)
                {
                    Assert.Equal(expected[row,col], a[row, col]);
                }
            }
        }

        [Fact]
        public void TestCreateLogicalMatrixValid()
        {
            SetUpTest();
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
            SetUpTest();
            var engine = this.Engine;
            engine.Evaluate("x <- matrix((1:110 + 1i*(101:210)), nrow=10, ncol=11)");
            var exp_one = Array.ConvertAll(GenArrayInteger(1, 110), val => new Complex(val, val + 100));
            var expected = ToMatrix(exp_one, 10, 11);
            var a = engine.GetSymbol("x").AsComplexMatrix().ToArray();
            CheckArrayEqual(a, expected);
        }

        [Fact]
        public void TestPasteCreationAsCharacter()
        {
            // Created to explicitly test symbolic expression conversion, in response to:
            // https://github.com/jmp75/rdotnet/issues/79#issuecomment-399541873
            SetUpTest();
            var engine = this.Engine;
            const string prefix = "VarTemp_";
            engine.Evaluate(string.Format("VarTemp<-paste(sep='', '{0}',sample(1:10000000, 1, replace=F))", prefix));
            var result = engine.Evaluate("VarTemp").AsCharacter().First();

            // We're testing with a randomly generated string - we don't really care what comes out, it just
            // has to be something longer than the prefix.
            Assert.True(result.StartsWith(prefix) && result.Length > prefix.Length);   
        }
    }
}