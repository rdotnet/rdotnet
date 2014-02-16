using System;
using NUnit.Framework;

namespace RDotNet
{
   [TestFixture]
   public class RDotNetTestFixture
   {
      private readonly MockDevice device = new MockDevice();

      protected string EngineName { get { return "RDotNetTest"; } }

      protected MockDevice Device { get { return this.device; } }

      private static REngine engine = null;

      private readonly bool initializeOnceOnly = false;

      [TestFixtureSetUp]
      protected virtual void SetUpFixture()
      {
         if (initializeOnceOnly && engine != null)
            return;
         Helper.SetEnvironmentVariables();
         engine = REngine.CreateInstance(EngineName);
         engine.Initialize(device: device);
      }

      [TestFixtureTearDown]
      protected virtual void TearDownFixture()
      {
         if (initializeOnceOnly && engine != null)
            engine.ClearGlobalEnvironment();
         else
         //var engine = REngine.GetInstanceFromID(EngineName);
            if (engine != null)
            {
               engine.Dispose();
            }
      }

      [SetUp]
      protected virtual void SetUpTest()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
         engine.Evaluate("rm(list=ls())");
         this.device.Initialize();
      }

      [TearDown]
      protected virtual void TearDownTest()
      {
      }

      protected double[] GenArrayDouble(int from, int to)
      {
         return Array.ConvertAll(GenArrayInteger(from, to), input => (double)input);
      }

      protected static T[,] ToMatrix<T>(T[] d, int nrow, int ncol)
      {
         var res = new T[nrow,ncol];
         for (int i = 0; i < nrow; i++)
            for (int j = 0; j < ncol; j++)
               res[i, j] = d[nrow * j + i];
         return res;
      }

      protected double[] ArrayMult(double[] a, double mult)
      {
         return Array.ConvertAll(a, input => input*mult);
      }

      protected int[] GenArrayInteger(int from, int to)
      {
         Assert.Greater(to, from);
         var res = new int[to - from + 1];
         for (int i = 0; i < (to - from + 1); i++)
            res[i] = i + from;
         return res;
      }

      // I thought NUnit was dealing with array equivalence. Cannot see here, so emulate.
      protected static void CheckArrayEqual<T>(T[] a, T[] expected)
      {
         Assert.AreEqual(expected.Length, a.Length);
         for (int i = 0; i < a.Length; i++)
            Assert.AreEqual(expected[i], a[i]); //, 1e-9);
      }

      // I thought NUnit was dealing with array equivalence. Cannot see here, so emulate.
      protected static void CheckArrayEqual<T>(T[,] a, T[,] expected)
      {
         Assert.AreEqual(expected.Length, a.Length);
         for (int i = 0; i < 2; i++)
            Assert.AreEqual(expected.GetLength(i), a.GetLength(i));
         for (int i = 0; i < a.GetLength(0); i++)
            for (int j = 0; j < a.GetLength(1); j++)
               Assert.AreEqual(expected[i,j], a[i,j]); //, 1e-9);
      }

   }
}