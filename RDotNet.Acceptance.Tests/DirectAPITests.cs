using System;
using System.Collections.Generic;
using System.Numerics;
using RDotNet.R.Adapter;
using RDotNet.Server;
using FluentAssertions;
using NUnit.Framework;
using CoreRManagementService = RDotNet.Server.Services.RManagementService;
using CoreRLanguageService = RDotNet.Server.Services.RLanguageService;

namespace RDotNet.Acceptance.Tests
{
    [TestFixture]
    [Category("Acceptance")]
    public class DirectAPITests
    {
        private CoreRManagementService _ms;
        private CoreRLanguageService _ls;

        [Test]
        public void CreateStrings()
        {
            var c = _ls.MakeChar("charTest");
            c.Type.Should().Be(SymbolicExpressionType.InternalCharacterString);

            var converted = _ls.GetInternalCharacterValue(c);
            converted.Should().Be("charTest");
        }

        [Test]
        public void CreateCharacterVectors()
        {
            var s = _ls.MakeString("stringTest");
            s.Type.Should().Be(SymbolicExpressionType.CharacterVector);

            var converted = _ls.GetCharacterVectorValueAt(s, 0);
            converted.Should().Be("stringTest");

            var vector = _ls.AllocateVector(SymbolicExpressionType.CharacterVector, 4);
            _ls.InitializeCharacterVector(vector, new List<string> {"A", "B", "C", "D"});
            var values = _ls.GetCharacterVectorValues(vector);
            values.Should().ContainInOrder(new[] {"A", "B", "C", "D"});

            _ls.SetCharacterVectorValueAt(vector, 0, "StringA");
            _ls.SetCharacterVectorValueAt(vector, 1, "StringB");
            _ls.SetCharacterVectorValueAt(vector, 2, null);
            _ls.SetCharacterVectorValueAt(vector, 3, "StringC");

            values = _ls.GetCharacterVectorValues(vector);
            values.Should().ContainInOrder(new[] {"StringA", "StringB", null, "StringC"});

            var result = _ls.GetCharacterVectorValueAt(vector, 0);
            result.Should().Be("StringA");

            result = _ls.GetCharacterVectorValueAt(vector, 1);
            result.Should().Be("StringB");

            result = _ls.GetCharacterVectorValueAt(vector, 2);
            result.Should().BeNull();

            result = _ls.GetCharacterVectorValueAt(vector, 3);
            result.Should().Be("StringC");

            _ms.ForceGarbageCollection();
            //NAString, SetValue, etc..
        }

        [Test]
        public void CreateIntegerVectors()
        {
            var vector = _ls.AllocateVector(SymbolicExpressionType.IntegerVector, 4);
            _ls.InitializeIntegerVector(vector, new List<int> {1, 2, 3, 4});
            var values = _ls.GetIntegerVectorValues(vector);
            values.Should().ContainInOrder(new[] {1, 2, 3, 4});

            _ls.SetIntegerVectorValueAt(vector, 0, 71);
            _ls.SetIntegerVectorValueAt(vector, 1, 2);
            _ls.SetIntegerVectorValueAt(vector, 2, 113);

            values = _ls.GetIntegerVectorValues(vector);
            values.Should().ContainInOrder(new[] {71, 2, 113, 4});

            var result = _ls.GetIntegerVectorValueAt(vector, 0);
            result.Should().Be(71);

            result = _ls.GetIntegerVectorValueAt(vector, 1);
            result.Should().Be(2);

            result = _ls.GetIntegerVectorValueAt(vector, 2);
            result.Should().Be(113);

            result = _ls.GetIntegerVectorValueAt(vector, 3);
            result.Should().Be(4);

            _ls.SetIntegerVectorValueAt(vector, 3, -18);
            result = _ls.GetIntegerVectorValueAt(vector, 3);
            result.Should().Be(-18);

            _ms.ForceGarbageCollection();
        }

        [Test]
        public void CreateNumericVectors()
        {
            var vector = _ls.AllocateVector(SymbolicExpressionType.NumericVector, 4);
            _ls.InitializeNumericVector(vector, new List<double> {0.1, 2.0, 3.1, 4.2});

            var values = _ls.GetNumericVectorValues(vector);
            values.Should().ContainInOrder(new[] {0.1, 2.0, 3.1, 4.2});

            _ls.SetNumericVectorValueAt(vector, 0, 0.314);
            _ls.SetNumericVectorValueAt(vector, 1, 0.836);
            _ls.SetNumericVectorValueAt(vector, 2, 5.09);

            values = _ls.GetNumericVectorValues(vector);
            values.Should().ContainInOrder(new[] {0.314, 0.836, 5.09, 4.2});

            var result = _ls.GetNumericVectorValueAt(vector, 0);
            result.Should().Be(0.314);

            result = _ls.GetNumericVectorValueAt(vector, 1);
            result.Should().Be(0.836);

            result = _ls.GetNumericVectorValueAt(vector, 2);
            result.Should().Be(5.09);

            result = _ls.GetNumericVectorValueAt(vector, 3);
            result.Should().Be(4.2);

            _ls.SetNumericVectorValueAt(vector, 3, -1.3333);
            result = _ls.GetNumericVectorValueAt(vector, 3);
            result.Should().Be(-1.3333);

            _ms.ForceGarbageCollection();
        }

        [Test]
        public void CreateComplexVectors()
        {
            var vector = _ls.AllocateVector(SymbolicExpressionType.ComplexVector, 3);
            _ls.InitializeComplexVector(vector, new List<Complex> {new Complex(0, 5), new Complex(1, 4), new Complex(2, 3)});

            var values = _ls.GetComplexVectorValues(vector);
            values.Should().ContainInOrder(new[] {new Complex(0, 5), new Complex(1, 4), new Complex(2, 3)});

            _ls.SetComplexVectorValueAt(vector, 0, new Complex(3, 14));
            _ls.SetComplexVectorValueAt(vector, 1, new Complex(0, 0));
            _ls.SetComplexVectorValueAt(vector, 2, new Complex(5.09, 0));

            values = _ls.GetComplexVectorValues(vector);
            values.Should().ContainInOrder(new[] {new Complex(3, 14), new Complex(0, 0), new Complex(5.09, 0)});

            var result = _ls.GetComplexVectorValueAt(vector, 0);
            result.Should().Be(new Complex(3, 14));

            result = _ls.GetComplexVectorValueAt(vector, 1);
            result.Should().Be(new Complex(0, 0));

            result = _ls.GetComplexVectorValueAt(vector, 2);
            result.Should().Be(new Complex(5.09, 0));


            _ls.SetComplexVectorValueAt(vector, 1, new Complex(1, 0));
            result = _ls.GetComplexVectorValueAt(vector, 1);
            result.Should().Be(new Complex(1, 0));

            _ms.ForceGarbageCollection();
        }

        [Test]
        public void CreateRawVectors()
        {
            var vector = _ls.AllocateVector(SymbolicExpressionType.RawVector, 8);
            _ls.InitializeRawVector(vector, new List<byte> {0xD, 0xE, 0xA, 0xD, 0xB, 0xE, 0xE, 0xF});

            var values = _ls.GetRawVectorValues(vector);
            values.Should().ContainInOrder(new byte[] { 0xD, 0xE, 0xA, 0xD, 0xB, 0xE, 0xE, 0xF });

            _ls.SetRawVectorValueAt(vector, 0, 0xB);
            _ls.SetRawVectorValueAt(vector, 1, 0xA);
            _ls.SetRawVectorValueAt(vector, 6, 0xD);

            values = _ls.GetRawVectorValues(vector);
            values[0].Should().Be(0xB);
            values[1].Should().Be(0xA);
            values[6].Should().Be(0xD);

            var result = _ls.GetRawVectorValueAt(vector, 0);
            result.Should().Be(0xB);

            result = _ls.GetRawVectorValueAt(vector, 1);
            result.Should().Be(0xA);

            result = _ls.GetRawVectorValueAt(vector, 6);
            result.Should().Be(0xD);

            _ls.SetRawVectorValueAt(vector, 1, 0xF);
            result = _ls.GetRawVectorValueAt(vector, 1);
            result.Should().Be(0xF);

            _ms.ForceGarbageCollection();
        }

        [Test]
        public void CreateLogicalVectors()
        {
            var vector = _ls.AllocateVector(SymbolicExpressionType.LogicalVector, 3);
            _ls.InitializeLogicalVector(vector, new List<bool> {true, false, true});

            var values = _ls.GetLogicalVectorValues(vector);
            values.Should().ContainInOrder(new[] {true, false, true});

            _ls.SetLogicalVectorValueAt(vector, 0, false);
            _ls.SetLogicalVectorValueAt(vector, 1, true);
            _ls.SetLogicalVectorValueAt(vector, 2, false);

            values = _ls.GetLogicalVectorValues(vector);
            values.Should().ContainInOrder(new[] {false, true, false});

            var result = _ls.GetLogicalVectorValueAt(vector, 0);
            result.Should().BeFalse();

            result = _ls.GetLogicalVectorValueAt(vector, 1);
            result.Should().BeTrue();

            result = _ls.GetLogicalVectorValueAt(vector, 2);
            result.Should().BeFalse();

            _ls.SetLogicalVectorValueAt(vector, 1, false);
            result = _ls.GetLogicalVectorValueAt(vector, 1);
            result.Should().BeFalse();

            _ms.ForceGarbageCollection();
        }

        [Test]
        public void CreateExpressionVectors()
        {
            var vector = _ls.AllocateVector(SymbolicExpressionType.ExpressionVector, 3);
            _ls.InitializeExpressionVector(vector, new List<SymbolicExpressionContext> {_ls.GetNilValue(), _ls.GetNilValue(), _ls.GetNilValue()});

            var values = _ls.GetExpressionVectorValues(vector);
            var nil = _ls.GetNilValue();
            values.Should().ContainInOrder(new object[] {nil, nil, nil});

            var exp1 = _ls.AllocateSEXP(SymbolicExpressionType.Closure);
            var exp2 = _ls.AllocateSEXP(SymbolicExpressionType.Environment);
            var exp3 = _ls.AllocateSEXP(SymbolicExpressionType.BuiltinFunction);
            _ls.SetExpressionVectorValueAt(vector, 0, exp1);
            _ls.SetExpressionVectorValueAt(vector, 1, exp2);
            _ls.SetExpressionVectorValueAt(vector, 2, exp3);

            values = _ls.GetExpressionVectorValues(vector);
            values.Should().BeEquivalentTo(new object[] {exp1, exp2, exp3});

            var result = _ls.GetExpressionVectorValueAt(vector, 0);
            result.Should().Be(exp1);

            result = _ls.GetExpressionVectorValueAt(vector, 1);
            result.Should().Be(exp2);

            result = _ls.GetExpressionVectorValueAt(vector, 2);
            result.Should().Be(exp3);

            _ls.SetExpressionVectorValueAt(vector, 1, _ls.GetNilValue());
            result = _ls.GetExpressionVectorValueAt(vector, 1);
            result.Should().Be(nil);

            _ms.ForceGarbageCollection();
        }

        [Test]
        public void AllocateAndSniffTypes()
        {
            var allocated = _ls.AllocateSEXP(SymbolicExpressionType.BuiltinFunction);
            _ls.IsFunction(allocated).Should().BeTrue();

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.CharacterVector);
            _ls.IsVector(allocated).Should().BeTrue();

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.Closure);
            _ls.IsFunction(allocated).Should().BeTrue();

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.ComplexVector);
            _ls.IsVector(allocated).Should().BeTrue();

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.Environment);
            _ls.IsEnvironment(allocated).Should().BeTrue();

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.ExpressionVector);
            _ls.IsVector(allocated).Should().BeTrue();

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.IntegerVector);
            _ls.IsVector(allocated).Should().BeTrue();

            allocated = _ls.MakeString("Howdy howdy howdy");
            _ls.IsString(allocated).Should().BeTrue();

            allocated = _ls.MakeChar("Taco taco taco");
            allocated.Type.Should().Be(SymbolicExpressionType.InternalCharacterString);

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.InternalCharacterString);
            allocated.Type.Should().Be(SymbolicExpressionType.InternalCharacterString);

//            allocated = ls.AllocateSEXP(SymbolicExpressionType.List);
//            Assert.That(ls.IsList(allocated), Is.True);???!

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.LogicalVector);
            allocated.Type.Should().Be(SymbolicExpressionType.LogicalVector);

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.Null);
            _ls.IsNull(allocated).Should().BeTrue();

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.NumericVector);
            _ls.IsVector(allocated).Should().BeTrue();

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.Pairlist);
            _ls.IsPairList(allocated).Should().BeTrue();

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.Promise);
            _ls.IsPromise(allocated).Should().BeTrue();

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.RawVector);
            _ls.IsVector(allocated).Should().BeTrue();

//            allocated = ls.AllocateSEXP(SymbolicExpressionType.S4);
//            Assert.That(ls.IsS4(allocated), Is.True);???!

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.SpecialFunction);
            _ls.IsFunction(allocated).Should().BeTrue();

            allocated = _ls.AllocateSEXP(SymbolicExpressionType.Symbol);
            _ls.IsSymbol(allocated).Should().BeTrue();

            _ms.ForceGarbageCollection();
        }

        [Test]
        public void EvaluateExpressions()
        {
            const string msg = "sessionInfo()\n";
            var s = _ls.MakeString(msg);
            var ge = _ls.GetGlobalEnvironment();
            _ls.Protect(s);
            _ls.Protect(ge);

            ParseStatus status;
            var vector = _ls.ParseVector(s, -1, out status);
            status.Should().Be(ParseStatus.Ok);
            _ls.Protect(vector);

            var expression = _ls.GetExpressionVectorValueAt(vector, 0);

            bool errorOccurred;
            var evaluated = _ls.TryEvaluate(expression, ge, out errorOccurred);
            errorOccurred.Should().BeFalse();
            _ls.Protect(evaluated);

            var messages = _ls.CoerceVector(evaluated, SymbolicExpressionType.CharacterVector);
            _ls.Protect(messages);

            var message = _ls.GetCharacterVectorValueAt(messages, 0);

            _ls.Unprotect(5);
            message.Should().Contain("R version 3.1.0");
        }

        [Test]
        public void CreateCharacterMatrix()
        {
            var matrix = _ls.AllocateMatrix(SymbolicExpressionType.CharacterVector, 3, 2);
            _ls.InitializeCharacterMatrix(matrix, new List<List<string>> {new List<string> {"A", "B", "C"}, new List<string> {"D", null, "FFFFF"}});
            var values = _ls.GetCharacterMatrixValues(matrix);
            values[0][0].Should().Be("A");
            values[0][1].Should().Be("B");
            values[0][2].Should().Be("C");
            values[1][0].Should().Be("D");
            values[1][1].Should().BeNull();
            values[1][2].Should().Be("FFFFF");

            _ls.SetCharacterMatrixValueAt(matrix, 0, 0, "StringA");
            _ls.SetCharacterMatrixValueAt(matrix, 1, 1, "StringB");
            _ls.SetCharacterMatrixValueAt(matrix, 2, 1, null);

            values = _ls.GetCharacterMatrixValues(matrix);
            values[0][0].Should().Be("StringA");
            values[1][1].Should().Be("StringB");
            values[1][2].Should().BeNull();

            var result = _ls.GetCharacterMatrixValueAt(matrix, 0, 0);
            result.Should().Be("StringA");

            result = _ls.GetCharacterMatrixValueAt(matrix, 1, 1);
            result.Should().Be("StringB");

            result = _ls.GetCharacterMatrixValueAt(matrix, 2, 1);
            result.Should().BeNull();
            _ms.ForceGarbageCollection();
            //NAString, SetValue, etc..
        }

        [Test]
        public void CreateIntegerMatrix()
        {
            var matrix = _ls.AllocateMatrix(SymbolicExpressionType.IntegerVector, 4, 4);
            _ls.InitializeIntegerMatrix(matrix,
                new List<List<int>>
                {
                    new List<int> {1, 0, 0, 0},
                    new List<int> {0, 2, 0, 0},
                    new List<int> {0, 0, 3, 0},
                    new List<int> {0, 0, 0, 4}
                });
            var values = _ls.GetIntegerMatrixValues(matrix);
            values[0][0].Should().Be(1);
            values[1][1].Should().Be(2);
            values[2][2].Should().Be(3);
            values[3][3].Should().Be(4);

            _ls.SetIntegerMatrixValueAt(matrix, 0, 0, 71);
            _ls.SetIntegerMatrixValueAt(matrix, 1, 1, 2);
            _ls.SetIntegerMatrixValueAt(matrix, 2, 2, 113);

            values = _ls.GetIntegerMatrixValues(matrix);
            values[0][0].Should().Be(71);
            values[1][1].Should().Be(2);
            values[2][2].Should().Be(113);
            values[3][3].Should().Be(4);

            var result = _ls.GetIntegerMatrixValueAt(matrix, 0, 0);
            result.Should().Be(71);

            result = _ls.GetIntegerMatrixValueAt(matrix, 1, 1);
            result.Should().Be(2);

            result = _ls.GetIntegerMatrixValueAt(matrix, 2, 2);
            result.Should().Be(113);

            result = _ls.GetIntegerMatrixValueAt(matrix, 3, 3);
            result.Should().Be(4);

            _ls.SetIntegerMatrixValueAt(matrix, 3, 3, -18);
            result = _ls.GetIntegerMatrixValueAt(matrix, 3, 3);
            result.Should().Be(-18);

            _ms.ForceGarbageCollection();
        }

        [Test]
        public void CreateNumericMatrix()
        {
            var matrix = _ls.AllocateMatrix(SymbolicExpressionType.NumericVector, 4, 4);
            _ls.InitializeNumericMatrix(matrix,
                new List<List<double>>
                {
                    new List<double> {0.1, 0, 0, 0},
                    new List<double> {0, 2.0, 0, 0},
                    new List<double> {0, 0, 3.1, 0},
                    new List<double> {0, 0, 0, 4.2}
                });

            var values = _ls.GetNumericMatrixValues(matrix);
            values[0][0].Should().Be(0.1);
            values[1][1].Should().Be(2.0);
            values[2][2].Should().Be(3.1);
            values[3][3].Should().Be(4.2);

            _ls.SetNumericMatrixValueAt(matrix, 0, 0, 0.314);
            _ls.SetNumericMatrixValueAt(matrix, 1, 1, 0.836);
            _ls.SetNumericMatrixValueAt(matrix, 2, 2, 5.09);

            values = _ls.GetNumericMatrixValues(matrix);
            values[0][0].Should().Be(0.314);
            values[1][1].Should().Be(0.836);
            values[2][2].Should().Be(5.09);
            values[3][3].Should().Be(4.2);

            var result = _ls.GetNumericMatrixValueAt(matrix, 0, 0);
            result.Should().Be(0.314);

            result = _ls.GetNumericMatrixValueAt(matrix, 1, 1);
            result.Should().Be(0.836);

            result = _ls.GetNumericMatrixValueAt(matrix, 2, 2);
            result.Should().Be(5.09);

            result = _ls.GetNumericMatrixValueAt(matrix, 3, 3);
            result.Should().Be(4.2);

            _ls.SetNumericMatrixValueAt(matrix, 3, 3, -1.3333);
            result = _ls.GetNumericMatrixValueAt(matrix, 3, 3);
            result.Should().Be(-1.3333);

            _ms.ForceGarbageCollection();
        }

        [Test]
        public void CreateComplexMatrix()
        {
            var matrix = _ls.AllocateMatrix(SymbolicExpressionType.ComplexVector, 3, 3);
            _ls.InitializeComplexMatrix(matrix,
                new List<List<Complex>>
                {
                    new List<Complex> {new Complex(0, 5), new Complex(0, 0), new Complex(0, 0)},
                    new List<Complex> {new Complex(0, 0), new Complex(1, 4), new Complex(0, 0)},
                    new List<Complex> {new Complex(0, 0), new Complex(0, 0), new Complex(2, 3)}
                });

            var values = _ls.GetComplexMatrixValues(matrix);
            values[0][0].Should().Be(new Complex(0, 5));
            values[1][1].Should().Be(new Complex(1, 4));
            values[2][2].Should().Be(new Complex(2, 3));

            _ls.SetComplexMatrixValueAt(matrix, 0, 0, new Complex(3, 14));
            _ls.SetComplexMatrixValueAt(matrix, 0, 1, new Complex(0, 1));
            _ls.SetComplexMatrixValueAt(matrix, 0, 2, new Complex(5.09, 0));

            values = _ls.GetComplexMatrixValues(matrix);
            values[0][0].Should().Be(new Complex(3, 14));
            values[1][0].Should().Be(new Complex(0, 1));
            values[2][0].Should().Be(new Complex(5.09, 0));

            var result = _ls.GetComplexMatrixValueAt(matrix, 0, 0);
            result.Should().Be(new Complex(3, 14));

            result = _ls.GetComplexMatrixValueAt(matrix, 0, 1);
            result.Should().Be(new Complex(0, 1));

            result = _ls.GetComplexMatrixValueAt(matrix, 0, 2);
            result.Should().Be(new Complex(5.09, 0));

            _ls.SetComplexMatrixValueAt(matrix, 1, 0, new Complex(1, 0));
            result = _ls.GetComplexMatrixValueAt(matrix, 1, 0);
            result.Should().Be(new Complex(1, 0));

            _ms.ForceGarbageCollection();
        }

        [Test]
        public void CreateRawMatrix()
        {
            var vector = _ls.AllocateMatrix(SymbolicExpressionType.RawVector, 4, 4);
            _ls.InitializeRawMatrix(vector,
                new List<List<byte>>
                {
                    new List<byte> {0xD, 0, 0, 0},
                    new List<byte> {0, 0xE, 0, 0},
                    new List<byte> {0, 0, 0xA, 0},
                    new List<byte> {0, 0, 0, 0xD}
                });

            var values = _ls.GetRawMatrixValues(vector);
            values[0][0].Should().Be(0xD);
            values[1][1].Should().Be(0xE);
            values[2][2].Should().Be(0xA);
            values[3][3].Should().Be(0xD);

            _ls.SetRawMatrixValueAt(vector, 0, 0, 0xB);
            _ls.SetRawMatrixValueAt(vector, 1, 1, 0xA);
            _ls.SetRawMatrixValueAt(vector, 3, 2, 0xD);

            values = _ls.GetRawMatrixValues(vector);
            values[0][0].Should().Be(0xB);
            values[1][1].Should().Be(0xA);
            values[3][2].Should().Be(0xD);

            var result = _ls.GetRawMatrixValueAt(vector, 0, 0);
            result.Should().Be(0xB);

            result = _ls.GetRawMatrixValueAt(vector, 1, 1);
            result.Should().Be(0xA);

            result = _ls.GetRawMatrixValueAt(vector, 3, 2);
            result.Should().Be(0xD);

            _ls.SetRawMatrixValueAt(vector, 1, 2, 0xF);
            result = _ls.GetRawMatrixValueAt(vector, 1, 2);
            result.Should().Be(0xF);

            _ms.ForceGarbageCollection();
        }

        [Test]
        public void CreateLogicalMatrix()
        {
            var vector = _ls.AllocateMatrix(SymbolicExpressionType.LogicalVector, 3, 3);
            _ls.InitializeLogicalMatrix(vector,
                new List<List<bool>>
                {
                    new List<bool> {true, false, false},
                    new List<bool> {false, true, false},
                    new List<bool> {false, false, true}
                });

            var values = _ls.GetLogicalMatrixValues(vector);
            values[0][0].Should().BeTrue();
            values[1][1].Should().BeTrue();
            values[2][2].Should().BeTrue();

            _ls.SetLogicalMatrixValueAt(vector, 0, 0, false);
            _ls.SetLogicalMatrixValueAt(vector, 0, 1, false);
            _ls.SetLogicalMatrixValueAt(vector, 0, 2, false);

            values = _ls.GetLogicalMatrixValues(vector);
            values[0][0].Should().BeFalse();
            values[0][1].Should().BeFalse();
            values[0][2].Should().BeFalse();

            var result = _ls.GetLogicalMatrixValueAt(vector, 0, 0);
            result.Should().BeFalse();

            result = _ls.GetLogicalMatrixValueAt(vector, 0, 1);
            result.Should().BeFalse();

            result = _ls.GetLogicalMatrixValueAt(vector, 0, 2);
            result.Should().BeFalse();

            _ls.SetLogicalMatrixValueAt(vector, 0, 1, true);
            result = _ls.GetLogicalMatrixValueAt(vector, 0, 1);
            result.Should().BeTrue();

            _ms.ForceGarbageCollection();
        }

        public void SetFixture(REngineFixture data)
        {
            _ms = data.ManagementService;
            _ls = data.LanguageService;
        }

        public class REngineFixture : IDisposable
        {
            private static readonly RInstanceManager InstanceManager = new RInstanceManager();
            public CoreRManagementService ManagementService { get; private set; }
            public CoreRLanguageService LanguageService { get; private set; }

            public REngineFixture()
            {
                ManagementService = new CoreRManagementService(InstanceManager);
                LanguageService = new CoreRLanguageService(InstanceManager);

                ManagementService.Start(StartupParameter.Default);

                var alive = ManagementService.IsAlive();
                alive.Should().BeTrue();
            }

            public void Dispose()
            {
                ManagementService.ForceGarbageCollection();
                ManagementService.Terminate();
            }
        }
    }
}
