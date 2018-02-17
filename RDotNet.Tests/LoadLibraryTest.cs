using DynamicInterop;
using Xunit;
using RDotNet.NativeLibrary;
using System;
using System.IO;

namespace RDotNet
{
    [Collection("R.NET unit tests")]
    public class LoadLibraryTest
    {
        [Fact]
        public void TestLoadUnmanagedDllNullRef()
        {
            Assert.Throws<ArgumentNullException>(
                () => {
                    var lib = new MockLoadLib(null);
                });
        }

        [Fact]
        public void TestLoadUnmanagedDllEmptyString()
        {
            Assert.Throws<ArgumentException>(
                () => {
                    var lib = new MockLoadLib("");
                }
                //,
                //  "The name of the library to load is an empty string"
                );
        }

        [Fact]
        public void TestLoadUnmanagedDllWrongShortName()
        {
            // Note: this does not pass as of 2017-08. 
            // Kept as is until dynamic-interop changes the exception type to ArgumentException.
            Assert.Throws<ArgumentException>(
                () => {
                    var lib = new MockLoadLib("SomeVeryUnlikelyName.dll");
                });
        }

        [Fact]
        public void TestLoadWindows()
        {
            // Test, if possible, loading 32 bits R.dll from a 64 bits process.
            // I do not really like tests to be platform-dependent, but this is pragmatically unavoidable.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;
            string rDllPath = "";
            try
            {
                rDllPath = new NativeUtility().FindRPathFromRegistry();
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
                Assert.Throws<Exception>(() => { var lib = new MockLoadLib(rDllFullFilePath); });
            }
        }

        private class MockLoadLib : UnmanagedDll
        {
            public MockLoadLib(string dll)
                : base(dll)
            {
                // nothing here.
            }
        }
    }
}