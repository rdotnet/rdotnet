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

      [TestFixtureSetUp]
      protected virtual void SetUpFixture()
      {
         Helper.SetEnvironmentVariables();
         var engine = REngine.CreateInstance(EngineName);
         engine.Initialize(device: device);
      }

      [TestFixtureTearDown]
      protected virtual void TearDownFixture()
      {
         var engine = REngine.GetInstanceFromID(EngineName);
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

   }
}