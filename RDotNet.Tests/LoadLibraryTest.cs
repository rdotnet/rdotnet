using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RDotNet.NativeLibrary;

namespace RDotNet
{
   [TestFixture]
   public class LoadLibraryTest
   {
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void TestLoadUnmanagedDllNullRef()
      {
         var lib = new MockLoadLib(null);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestLoadUnmanagedDllEmptyString()
      {
         var lib = new MockLoadLib("");
      }

      [Test]
      [ExpectedException(typeof(DllNotFoundException))]
      public void TestLoadUnmanagedDllWrongShortName()
      {
         var lib = new MockLoadLib("SomeVeryUnlikelyName.dll");
      }

      [Test]
      public void TestLoadWindows()
      {
         // Test, if possible, loading 32 bits R.dll from a 64 bits process.
         // I do not really like tests to be platform-dependent, but this is pragmatically unavoidable.
         if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            return;
         string rDllPath = "";
         try
         {
            rDllPath = NativeUtility.FindRPathFromRegistry();
         }
         catch (ApplicationException) // registry keys not found - bail out
         {
            return;
         }
         if (Environment.Is64BitProcess)
         {
            if (!rDllPath.Contains("x64")) // Odd, but not what we are testing here.
               return;
            rDllPath = rDllPath.Replace("x64", "i386");
            var rDllFullFilePath = Path.Combine(rDllPath, NativeUtility.GetRLibraryFileName());
            if (!File.Exists(rDllFullFilePath))
               return;
            Assert.Throws(typeof(Exception), () => { var lib = new MockLoadLib(rDllFullFilePath); });
         }
      }

      private class MockLoadLib : UnmanagedDll
      {
         public MockLoadLib(string dll) : base(dll)
         {
            // nothing here.
         }
      }
   }
}
