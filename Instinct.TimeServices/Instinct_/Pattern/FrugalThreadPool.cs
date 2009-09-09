using System.Threading;
namespace Instinct_.Pattern
{
    /// <summary>
    /// FrugalThreadPool
    /// </summary>
    /// http://www.albahari.com/threading/part4.aspx
    public class FrugalThreadPool : Pattern.Disposeable
    {
        private Thread[] _threadPool;
        private System.Collections.Generic.Queue<System.Collections.IEnumerable> _workQueue = new System.Collections.Generic.Queue<System.Collections.IEnumerable>();
        private ThreadStatus _threadStatus = ThreadStatus.Idle;
        private int _joiningThreadPoolCount;
        private object _joiningObject = new object();

        #region Class Types
        /// <summary>
        /// IThreadWork
        /// </summary>
        public interface IThreadWork
        {
            /// <summary>
            /// Executes this instance.
            /// </summary>
            void Execute();
        }

        /// <summary>
        /// ThreadStatus
        /// </summary>
        private enum ThreadStatus
        {
            /// <summary>
            /// Idle
            /// </summary>
            Idle = 1,
            /// <summary>
            /// Join
            /// </summary>
            Join = 2,
            /// <summary>
            /// Stop
            /// </summary>
            Stop = 3,
        }
        #endregion Class Types

        /// <summary>
        /// Initializes a new instance of the <see cref="FrugalThreadPool"/> class.
        /// </summary>
        public FrugalThreadPool()
            : this(4)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FrugalThreadPool"/> class.
        /// </summary>
        /// <param name="threadCount">The thread count.</param>
        public FrugalThreadPool(int threadCount)
        {
            _threadPool = new Thread[threadCount];
            for (int threadIndex = 0; threadIndex < _threadPool.Length; threadIndex++)
            {
                Thread thread = _threadPool[threadIndex] = new Thread(new ThreadStart(ThreadWorker)) { Name = "FrugalPool: " + threadIndex.ToString() };
                thread.Start();
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                lock (this)
                {
                    _threadStatus = ThreadStatus.Stop;
                    Monitor.PulseAll(this);
                }
                foreach (Thread thread in _threadPool)
                {
                    thread.Join();
                }
            }
        }

        /// <summary>
        /// Threads the worker.
        /// </summary>
        private void ThreadWorker()
        {
            System.Collections.IEnumerable list;
            while (true)
            {
                lock (this)
                {
                    while (_workQueue.Count == 0)
                    {
                        switch (_threadStatus)
                        {
                            case ThreadStatus.Stop:
                                return;
                            case ThreadStatus.Join:
                                lock (_joiningObject)
                                {
                                    _joiningThreadPoolCount--;
                                    Monitor.Pulse(_joiningObject);
                                }
                                break;
                        }
                        Monitor.Wait(this);
                    }
                    list = _workQueue.Dequeue();
                }
                if (list != null)
                {
                    foreach (IThreadWork @object in list)
                    {
                        @object.Execute();
                    }
                }
            }
        }

        /// <summary>
        /// Adds the specified list.
        /// </summary>
        /// <param name="list">The list.</param>
        public void Add(System.Collections.IEnumerable list)
        {
            if (_threadStatus != ThreadStatus.Idle)
            {
                throw new System.InvalidOperationException();
            }
            lock (this)
            {
                _workQueue.Enqueue(list);
                Monitor.Pulse(this);
            }
        }

        /// <summary>
        /// Joins this instance.
        /// </summary>
        /// <returns></returns>
        public void Join()
        {
            lock (this)
            {
                _threadStatus = ThreadStatus.Join;
                _joiningThreadPoolCount = _threadPool.Length;
                Monitor.PulseAll(this);
            }
            lock (_joiningObject)
            {
                while (_joiningThreadPoolCount > 0)
                {
                    Monitor.Wait(_joiningObject);
                }
                _threadStatus = ThreadStatus.Idle;
            }
        }
    }
}
