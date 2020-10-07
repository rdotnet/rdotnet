using Xunit;
using RDotNet.NativeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RDotNet
{

    /// <summary> The windows registry.</summary>
    public class MockRegistry : IRegistry
    {
        private MockRegistryKey currentUser;
        private MockRegistryKey localMachine;

        public MockRegistry(MockRegistryKey localMachine, MockRegistryKey currentUser)
        {
            this.localMachine = localMachine;
            this.currentUser = currentUser;
        }

        /// <summary> Gets the current user.</summary>
        ///
        /// <value> The current user.</value>
        public IRegistryKey CurrentUser
        {
            get
            {
                return currentUser;
            }
        }

        /// <summary> Gets the local machine.</summary>
        ///
        /// <value> The local machine.</value>
        public IRegistryKey LocalMachine
        {
            get
            {
                return localMachine;
            }
        }

        public MockRegistry(string localMachineTestReg, string currentUserTestReg = null)
        {
            localMachine = MockRegistryKey.Parse(localMachineTestReg);
            if(string.IsNullOrEmpty(currentUserTestReg))
                currentUser = null; 
            else 
                currentUser = MockRegistryKey.Parse(currentUserTestReg);
        }
    }

    /// <summary> The windows registry key.</summary>
    public class MockRegistryKey : IRegistryKey
    {
        public Dictionary<string, string> keyValues = new Dictionary<string, string>();
        private List<MockRegistryKey> subKeys = new List<MockRegistryKey>();

        private string ShortName;
        //public string /*LongName*/;
        public MockRegistryKey(string fullKey)
        {
            var s = CreateStack(fullKey);
            string k = s.Pop(); // HKEY_LOCAL_MACHINE
            ShortName = k;
            if(s.Count() > 0)
                subKeys.Add(new MockRegistryKey(s));
        }

        public MockRegistryKey(Stack<string> s)
        {
            if (s.Count() == 0) throw new ArgumentException("must be at least one item in the stack of keys");
            string k = s.Pop();
            ShortName = k;
            if (s.Count() > 0)
                subKeys.Add(new MockRegistryKey(s));
        }

        /// <summary>
        /// Get the real key of a registry entry
        /// </summary>
        /// <returns>RegistryKey object</returns>
        public Object GetRealKey()
        {
            return subKeys[0].GetRealKey();
        }

        /// <summary> Gets sub key names.</summary>
        ///
        /// <returns> An array of string.</returns>
        public string[] GetSubKeyNames()
        {
            return subKeys.Select(x => x.ShortName).ToArray();
        }

        /// <summary> Gets a value of a key-value pair.</summary>
        ///
        /// <param name="name"> The name.</param>
        ///
        /// <returns> The value.</returns>
        public object GetValue(string name)
        {
            return keyValues[name];
        }

        /// <summary> Retrieves an array of strings that contains all the value names associated with
        ///                 this key.</summary>
        ///
        /// <returns> An array of string.</returns>
        public string[] GetValueNames()
        {
            return keyValues.Select(x => x.Key).ToArray();
        }

        /// <summary> Opens sub key.</summary>
        ///
        /// <param name="name"> The name.</param>
        ///
        /// <returns> An IRegistryKey.</returns>
        public IRegistryKey OpenSubKey(string name)
        {
            var s = CreateStack(name);
            return this.Find(s);
        }

        internal static MockRegistryKey Parse(string localMachineTestReg)
        {
            //[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64]
            //'InstallPath'='C:\\Program Files\\R\\R-3.3.3'
            //'Current Version'='3.3.3'
            List<MockRegistryKey> topKeys = new List<MockRegistryKey>();
            var lines = localMachineTestReg.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string currentKey = null;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("["))
                {
                    currentKey = line.Replace("[", "").Replace("]", ""); // so left with HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64
                    Populate(currentKey, topKeys);
                }
                else if (currentKey != null)
                {
                    var x = line.Trim();
                    if (x.Length == 0) continue;
                    else
                    {
                        MockRegistryKey k = Find(topKeys, currentKey);
                        //'InstallPath'='C:\\Program Files\\R\\R-3.3.3'
                        var keyVal = x.Replace("'", "").Split('=');
                        k.AddKeyVal(keyVal[0], keyVal[1]);
                    }
                }
            }
            if (topKeys.Count() != 1)
                throw new Exception("");
            return topKeys[0];
        }

        public void AddKeyVal(string key, string val)
        {
            this.keyValues[key] = val;
        }

        private static Stack<string> CreateStack(string fullKey)
        {
            // fullKey is expected to be something like: HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64
            string[] kItems = fullKey.Split('\\');
            Array.Reverse(kItems);
            return new Stack<string>(kItems);
        }

        private static void Populate(string fullKey, List<MockRegistryKey> topKeys)
        {
            // fullKey is expected to be something like: HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64
            Stack<string> s = CreateStack(fullKey);
            if (s.Count() == 0) return;
            string k = s.Pop(); // HKEY_LOCAL_MACHINE
            var existing = topKeys.Where(m => m.ShortName == k);
            MockRegistryKey topKey = null;
            if (existing.Count() > 0)
            {
                topKey = existing.First();
                topKey.AddSubKeys(s);
            }
            else
            {
                topKey = new MockRegistryKey(fullKey);
                topKeys.Add(topKey);
            }
        }

        private void AddSubKeys(Stack<string> s)
        {
            if (s.Count() == 0) return ;
            string k = s.Pop();
            var kk = Find(k);
            if (kk == null)
            {
                if (s.Count() != 0)
                    subKeys.Add(new MockRegistryKey(s));
                else
                    subKeys.Add(new MockRegistryKey(k));
            }
            else
                kk.AddSubKeys(s);
        }

        private MockRegistryKey Find(string shortSubkeyName)
        {
            var v = from x in subKeys where x.ShortName == shortSubkeyName select x;
            if (v.Count() < 1)
                return null;
            if (v.Count() > 1)
                throw new Exception("More than one key found for: " + shortSubkeyName);
            return v.First();
        }

        private static MockRegistryKey Find(List<MockRegistryKey> topKeys, string fullKey)
        {
            Stack<string> s = CreateStack(fullKey);
            if (s.Count() == 0) return null;
            string k = s.Pop(); // HKEY_LOCAL_MACHINE
            var existing = topKeys.Where(m => m.ShortName == k);
            MockRegistryKey topKey = null;
            if (existing.Count() > 0) topKey = existing.First();
            if (s.Count() == 0) return topKey;
            else return topKey.Find(s);
        }

        private MockRegistryKey Find(Stack<string> s)
        {
            if (s.Count() == 0) return null;
            string k = s.Pop();
            var existing = this.subKeys.Where(m => m.ShortName == k);
            MockRegistryKey topKey = null;
            if (existing.Count() > 0) topKey = existing.First();
            if (s.Count() == 0) return topKey;
            else return topKey.Find(s);
        }

    }


    public class REngineInitTest : RDotNetTestFixture
    {
        [Fact(Skip = "Cannot run this in a batch with the new singleton pattern")] // Cannot run this in a batch with the new singleton pattern.
        public void TestInitParams()
        {
            MockDevice device = new MockDevice();
            REngine.SetEnvironmentVariables();
            using (var engine = REngine.GetInstance())
            {
                ulong maxMemSize = 128 * 1024 * 1024;
                StartupParameter parameter = new StartupParameter() {
                    MaxMemorySize = maxMemSize,
                };
                engine.Initialize(parameter: parameter, device: device);
                Assert.Equal(engine.Evaluate("memory.limit()").AsNumeric()[0], 128.0);
            }
        }

        private NativeUtility createTestRegistryUtil(bool realRegistry = true)
        {
            if (realRegistry)
                return new NativeUtility();
            else
            {
                string localMachineTestReg = @"
[HKEY_LOCAL_MACHINE\SOFTWARE\R-core]

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R]
'InstallPath'='C:\\Program Files\\R\\R-3.3.3'
'Current Version'='3.3.3'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\3.2.2.803]
'InstallPath'='C:\\Program Files\\Microsoft\\R Client\\R_SERVER\\'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\3.2.2.803 Microsoft R Client]
'InstallPath'='C:\\Program Files\\Microsoft\\R Client\\R_SERVER\\'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\3.3.3]
'InstallPath'='C:\\Program Files\\R\\R-3.3.3'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\R64]
'InstallPath'='C:\\Program Files\\Microsoft\\R Client\\R_SERVER\\'
'Current Version'='3.2.2.803'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\R64\3.2.2.803]
'InstallPath'='C:\\Program Files\\Microsoft\\R Client\\R_SERVER\\'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64]
'InstallPath'='C:\\Program Files\\R\\R-3.3.3'
'Current Version'='3.3.3'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64\3.2.2.803 Microsoft R Client]
'InstallPath'='C:\\Program Files\\Microsoft\\R Client\\R_SERVER\\'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64\3.3.3]
'InstallPath'='C:\\Program Files\\R\\R-3.3.3'
";

                return new NativeUtility(new MockRegistry(localMachineTestReg));
            }
        }

        // TODO: probably needs adjustments for MacOS and Linux
        [Fact]
        public void TestFindRBinPath()
        {
            string rLibPath = createTestRegistryUtil().FindRPath();
            var files = Directory.GetFiles(rLibPath);
            var fnmatch = files.Where(fn => fn.ToLower() == Path.Combine(rLibPath.ToLower(), NativeUtility.GetRLibraryFileName().ToLower()));
            Assert.Equal(1, fnmatch.Count());
        }

        [Fact]
        public void TestMockWindowsRegistry()
        {

            IRegistryKey rCore;
            // 2020-10 I lost sight of what this test was for. Causes issues on Linux, not sure why and too hard to debug against other priorities.
            if (NativeUtility.IsWin32NT)
            {
                var w = new WindowsRegistry();
                rCore = w.LocalMachine.OpenSubKey(@"SOFTWARE\R-core");


                string localMachineTestReg = @"
    [HKEY_LOCAL_MACHINE\SOFTWARE\R-core]

    [HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R]
    'InstallPath'='C:\Program Files\R\R-3.3.3'
    'Current Version'='3.3.3'
    ";
                var reg = new MockRegistry(localMachineTestReg);
                var lm = reg.LocalMachine;
                //var sk = lm.GetSubKeyNames();
                rCore = lm.OpenSubKey(@"SOFTWARE\R-core");
                var valNames = rCore.GetValueNames();
                Assert.Equal(valNames.Length, 0);

                Assert.Equal(rCore.GetSubKeyNames().Length, 1);
                Assert.Equal(rCore.GetSubKeyNames()[0], "R");
                var R = rCore.OpenSubKey(@"R");
                Assert.Equal(R.GetSubKeyNames().Length, 0);
                Assert.Equal(R.GetValueNames().Length, 2);
                Assert.Equal(R.GetValue("InstallPath"), "C:\\Program Files\\R\\R-3.3.3");
                Assert.Equal(R.GetValue("Current Version"), "3.3.3");

                localMachineTestReg = @"
    [HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\R64]
    'InstallPath'='C:\Program Files\Microsoft\R Client\R_SERVER\'
    'Current Version'='3.2.2.803'

    [HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\R64\3.2.2.803]
    'InstallPath'='C:\Program Files\Microsoft\R Client\R_SERVER\'
    ";

                reg = new MockRegistry(localMachineTestReg);
                lm = reg.LocalMachine;
                rCore = lm.OpenSubKey(@"SOFTWARE\R-core");
                Assert.Equal(rCore.GetValueNames().Length, 0);

                Assert.Equal(rCore.GetSubKeyNames().Length, 1);
                Assert.Equal(rCore.GetSubKeyNames()[0], "R");
                R = rCore.OpenSubKey(@"R");
                Assert.Equal(R.GetSubKeyNames().Length, 1);

                var R64 = lm.OpenSubKey(@"SOFTWARE\R-core\R\R64");

                Assert.Equal(R64.GetSubKeyNames().Length, 1);
                Assert.Equal(R64.GetValueNames().Length, 2);

                Assert.Equal(R64.GetValue("InstallPath"), @"C:\Program Files\Microsoft\R Client\R_SERVER\");
                Assert.Equal(R64.GetValue("Current Version"), "3.2.2.803");
            }
        }

        [Fact]
        public void TestFindRegKey()
        {
            //"HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\3.2.2.803 Microsoft R Client";
            //"HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\R64\3.2.2.803";
            //   C:\Program Files\Microsoft\R Client\R_SERVER\
            //string rLibPath = createTestRegistryUtil().FindRPaths();
            //var files = Directory.GetFiles(rLibPath);
            //var fnmatch = files.Where(fn => fn.ToLower() == Path.Combine(rLibPath.ToLower(), NativeUtility.GetRLibraryFileName().ToLower()));
            //Assert.Equal(1, fnmatch.Count());
        }

        [Fact]
        public void TestFindRHomePath()
        {
            string rHomePath = createTestRegistryUtil().FindRHome();
            var files = Directory.GetDirectories(rHomePath);
            var fnmatch = files.Where(fn => Path.GetFileName(fn) == "library");
            Assert.Equal(1, fnmatch.Count());
            fnmatch = Directory.GetDirectories(fnmatch.First()).Where(fn => Path.GetFileName(fn) == "base");
            Assert.Equal(1, fnmatch.Count());
        }

        [Fact]
        public void TestGetPathInitSearchLog()
        {
            SetUpTest();
            var engine = this.Engine;
            var log = NativeUtility.SetEnvironmentVariablesLog;
            Assert.NotEqual(string.Empty, log);
        }

        [Fact]
        public void TestUsingDefaultRPackages()
        {
            // This test was designed to look at a symptom observed alongside the issue https://github.com/rdotnet/rdotnet/issues/127  
            SetUpTest();
            var engine = this.Engine;
            var se = engine.Evaluate("set.seed");

            if(NativeUtility.GetPlatform() == PlatformID.Win32NT)
            {
                Assert.True(engine.Evaluate("Sys.which('R.dll')").AsCharacter()[0].Length > 0);
                Assert.True(engine.Evaluate("Sys.which('RBLAS.dll')").AsCharacter()[0].Length > 0);
            }

            string[] expected = { "base", "methods", "utils", "grDevices", "graphics", "stats" };
            var loadedDlls = engine.Evaluate("getLoadedDLLs()").AsList();
            string[] dllnames = loadedDlls.Select(x => x.AsCharacter().ToArray()[0]).ToArray();

            Assert.Equal(expected, dllnames);

            se = engine.Evaluate("set.seed(0)");
            se = engine.Evaluate("blah <- rnorm(4)");
        }
    }
}