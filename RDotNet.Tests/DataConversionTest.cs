using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using NUnit.Framework;

namespace RDotNet
{
   public class DataConversionTest : RDotNetTestFixture
   {
      [Test]
      public void TestCreateNumericVectorValid()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("x <- 1:100 * 1.1");
         var expected = ArrayMult(GenArrayDouble(1, 100), 1.1);
         var vec = engine.GetSymbol("x").AsNumeric();
         CheckBothArrayConversions(vec, expected);

         // Test a large data set: I just cannot believe how faster things are...
         engine.Evaluate("x <- 1:1e7 * 1.1");
         var a = engine.GetSymbol("x").AsNumeric().ToArrayFast();
         Assert.AreEqual(a[10000000/2-1], 1.1*1e7/2);
      }

      [Test]
      public void TestCreateIntegerVectorValid()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("x <- 1:100");
         var expected = GenArrayInteger(1, 100);
         var vec = engine.GetSymbol("x").AsInteger();
         CheckBothArrayConversions(vec, expected);
      }

      [Test]
      public void TestCreateLogicalVectorValid()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("x <- rep(c(TRUE,FALSE),50)");
         var expected = Array.ConvertAll(GenArrayInteger(1, 100), val => val % 2 == 1);
         var vec = engine.GetSymbol("x").AsLogical();
         CheckBothArrayConversions(vec, expected);
      }

      [Test]
      public void TestCreateCharacterVectorValid()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("x <- rep(c('a','bb'),50)");
         string[] expected = new string[100];
         for (int i = 0; i < 100; i++)
            expected[i] = (i % 2 == 0) ? "a" : "bb";
         var vec = engine.GetSymbol("x").AsCharacter();
         CheckBothArrayConversions(vec, expected);
      }

      [Test]
      public void TestCreateComplexVectorValid()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("x <- 1:100 + 1i*(101:200)");
         var expected = new Complex[100];
         for (int i = 0; i < 100; i++)
            expected[i] = new Complex(i+1,i+101);
         var vec = engine.GetSymbol("x").AsComplex();
         CheckBothArrayConversions(vec, expected);
      }

      private static void CheckBothArrayConversions<T>(Vector<T> vec, T[] expected)
      {
         var a = vec.ToArrayFast();
         CheckArrayEqual(a, expected);
         a = vec.ToArray();
         CheckArrayEqual(a, expected);
      }

      [Test]
      public void TestCreateNumericMatrixValid()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("x <- matrix(1:110 * 1.1, nrow=10, ncol=11)");
         var expected = ToMatrix(ArrayMult(GenArrayDouble(1, 110), 1.1), 10, 11);
         var a = engine.GetSymbol("x").AsNumericMatrix().ToArrayFast();
         CheckArrayEqual(a, expected);
      }

      [Test]
      public void TestCreateIntegerMatrixValid()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("x <- matrix(as.integer(1:110), nrow=10, ncol=11)");
         var expected = ToMatrix(GenArrayInteger(1, 110), 10, 11);
         var a = engine.GetSymbol("x").AsIntegerMatrix().ToArrayFast();
         CheckArrayEqual(a, expected);
      }

      [Test]
      public void TestCreateLogicalMatrixValid()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("x <- matrix(rep(c(TRUE,FALSE), 55), nrow=10, ncol=11)");
         var exp_one = Array.ConvertAll(GenArrayInteger(1, 110), val => val % 2 == 1);
         var expected = ToMatrix(exp_one, 10, 11);
         var a = engine.GetSymbol("x").AsLogicalMatrix().ToArrayFast();
         CheckArrayEqual(a, expected);
      }

      [Test]
      public void TestCreateComplexMatrixValid()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("x <- matrix((1:110 + 1i*(101:210)), nrow=10, ncol=11)");
         var exp_one = Array.ConvertAll(GenArrayInteger(1, 110), val => new Complex(val, val+100));
         var expected = ToMatrix(exp_one, 10, 11);
         var a = engine.GetSymbol("x").AsComplexMatrix().ToArrayFast();
         CheckArrayEqual(a, expected);
      }
   }
}
