using System.Threading;
using Xunit;
using RDotNet;
using RDotNet.Devices;
using System.Text;
using System.Diagnostics;

namespace SymbolicExpressionTest
{
    public class SymbolicExpressionReleaseTest
    {

        /// <summary>
        /// Generate a lot of R objects
        /// 
        /// R 4.1.2 => System.AccessViolationException / Stuck
        /// R 4.1.2 + SymbolicExprssion.Dispose => System.AccessViolationException / Stuck
        /// R 4.0.2 => System.AccessViolationException 
        /// R 4.0.2 + SymbolicExpression.Dispose => Sometimes stucked
        /// 
        /// Time: one minute
        /// 
        /// </summary>
        [Fact]
        public void CreateStressTest()
        {
            // Generate multiple R commands, not mandatory
            string cmd = BuildRCommand(2);
            using (var engine = BuildREngine())
            {
                engine.EnableLock = true;
                for (var i = 0; i < 500000; i++)
                {
                    SymbolicExpression e = engine.Evaluate(cmd);
                    e.Dispose();
                }
            }
        }

        /// <summary>
        /// Create multiple Rengine
        /// 
        /// </summary>
        [Fact]
        public void DisposeTest()
        {

            Process.GetCurrentProcess().Refresh(); 
            ProcessThreadCollection threadsAtStartup = Process.GetCurrentProcess().Threads;
            ProcessThreadCollection threadsRun = null;
            ProcessThreadCollection threadsAfterDispose = null;

            string cmd = BuildRCommand(100);
            REngine engine = null;
            using (engine = BuildREngine())
            {
                Process.GetCurrentProcess().Refresh();
                threadsRun = Process.GetCurrentProcess().Threads;
                SymbolicExpression e = engine.Evaluate(cmd);
            }

            Thread.Sleep(1000);
            Assert.StrictEqual(0, engine.ReleasableHandleCount);
            Process.GetCurrentProcess().Refresh();
            threadsAfterDispose = Process.GetCurrentProcess().Threads;

            Assert.True(threadsRun.Count > threadsAtStartup.Count);
            Assert.True(threadsRun.Count > threadsAfterDispose.Count);
        }

        private static string BuildRCommand(int n)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                sb.Append($"output{i} = mat.or.vec(100, 100); ");
            }

            return sb.ToString();
        }

        private static REngine BuildREngine()
        {
            ICharacterDevice device = new ConsoleDevice();
            //string RHome = "c:\\Progra~1\\R\\R-4.0.2";
            //REngine.SetEnvironmentVariables(RHome + "\\bin\\x64", RHome);
            
            REngine.SetEnvironmentVariables();

            return REngine.GetInstance(device: device);
        }
    }
}
