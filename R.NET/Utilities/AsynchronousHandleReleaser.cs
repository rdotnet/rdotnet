using System;
using System.Collections.Concurrent;
using System.Threading;
using RDotNet.Internals;

namespace RDotNet.Utilities
{
    /// <summary>
    /// Release handle from SymbolicExpression in a dedicated thread
    /// other than the GC thread
    /// </summary>
    public class AsynchronousHandleReleaser
    {
        private readonly BlockingCollection<IntPtr> m_queue = new BlockingCollection<IntPtr>(new ConcurrentQueue<IntPtr>());
        private readonly REngine m_engine;
        private bool m_stop;
        private TimeSpan m_timeout = TimeSpan.FromSeconds(2);
        private Thread m_thread;

        /// <summary>
        /// We need to have REngine reference to retrieve R.dll handles
        /// </summary>
        /// <param name="engine"></param>
        public AsynchronousHandleReleaser(REngine engine)
        {
            m_engine = engine;
        }

        /// <summary>
        /// Number of elements to release
        /// </summary>
        public int Count { get => m_queue.Count; }

        /// <summary>
        /// SymbolicExpression handler to release
        /// </summary>
        /// <param name="ptr"></param>
        public void AddHandler(IntPtr ptr)
        {
            if (m_stop)
            {
                ReleaseRHandle(ptr);
            } else
            {
                m_queue.Add(ptr);
            }
        }

        /// <summary>
        /// Launch thread
        /// </summary>
        public void Init()
        {
            m_thread = new Thread(Run);
            m_thread.IsBackground = true;
            m_thread.Start();
        }

        /// <summary>
        /// Stop thread
        /// </summary>
        public void Stop()
        {
            m_stop = true;
            m_queue.Add(IntPtr.Zero);
        }

        private void Run()
        {
            while (!m_stop)
            {
                if (m_queue.TryTake(out IntPtr handle, m_timeout))
                {
                    lock (SymbolicExpression.GetLockObject())
                    {
                        ReleaseRHandle(handle);
                    }
                }
            }

            PurgeRefQueue();
        }

        private void PurgeRefQueue()
        {
            lock (SymbolicExpression.GetLockObject())
            {
                while (m_queue.TryTake(out IntPtr handle))
                {
                    ReleaseRHandle(handle);
                }
            }
        }

        private void ReleaseRHandle(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                m_engine.GetFunction<R_ReleaseObject>()(handle);
            }
        }
    }
}
