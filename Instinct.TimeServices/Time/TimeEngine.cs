using System.Collections.Generic;
using Instinct_.Pattern;
using Instinct_;
using System.Runtime.InteropServices;
namespace Instinct.Time
{
    /// <summary>
    /// Timeline
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public abstract partial class TimeEngine<TListValue, TContext> : Disposeable
        where TListValue : class
        where TContext : new()
    {
        private const ulong MaxTimeslicesTime = (TimeSettings.MaxTimeslices << TimePrecision.TimePrecisionBits);
        //+
        private ulong _timesliceIndex = 0;
        private Timeslice[] _timeslices = new Timeslice[TimeSettings.MaxTimeslices];
        private ulong[] _workingFractions = new ulong[TimeSettings.MaxWorkingFractions];
        private ulong _minWorkingFraction; //: (sorted)dictionary removed are expensive. virtually remove with a window (_minWorkingFraction).
        private ulong _maxWorkingFraction;
        private bool _isRebuildWorkingFractions;

        #region Class Types
        /// <summary>
        /// ContextChange
        /// </summary>
        public class ContextChange : LinkNode<ContextChange>, FrugalThreadPool.IThreadWork
        {
            public ulong Time;
            public TContext This;
            public TContext Next;
            public ulong UpdateVectors;
            private TimeEngine<TListValue, TContext> _parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="TimeEngine&lt;TElement, TContext&gt;.Context&lt;TContext&gt;"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public ContextChange(TimeEngine<TListValue, TContext> parent)
            {
                _parent = parent;
            }

            /// <summary>
            /// Evaluates the time.
            /// </summary>
            public void Execute(object threadContext)
            {
                //_parent.UpdateContext(This, Next, UpdateVectors);
            }
        }

        /// <summary>
        /// Timeslice
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public /*protected*/ struct Timeslice
        {
            /// <summary>
            /// 
            /// </summary>
            public SortedDictionary<ulong, TimesliceNode> Fractions;
        }

        /// <summary>
        /// 
        /// </summary>
        public /*protected*/ class TimesliceNode
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TimeEngine&lt;TValue, TContext&gt;.TimesliceNode"/> class.
            /// </summary>
            public TimesliceNode()
            {
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="TimeEngine&lt;TValue, TContext&gt;.TimesliceNode"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public TimesliceNode(Link value)
            {
                Chain.AddFirst(value);
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="TimeEngine&lt;TValue, TContext&gt;.TimesliceNode"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public TimesliceNode(TListValue value)
            {
                List = new List<TListValue>(5);
                List.Add(value);
            }

            /// <summary>
            /// 
            /// </summary>
            public List<TListValue> List;
            /// <summary>
            /// 
            /// </summary>
            public LinkChain<Link> Chain = new LinkChain<Link>();
        }
        #endregion Class Types

        /// <summary>
        /// Initializes a new instance of the <see cref="Timeline&lt;T&gt;"/> class.
        /// </summary>
        public TimeEngine()
        {
            for (int timesliceIndex = 0; timesliceIndex < _timeslices.Length; timesliceIndex++)
            {
                _timeslices[timesliceIndex].Fractions = new SortedDictionary<ulong, TimesliceNode>();
            }
            for (int segmentIndex = 0; segmentIndex < _hibernateSegments.Length; segmentIndex++)
            {
                _hibernateSegments[segmentIndex].List = new LinkedList<HibernateState>();
                _hibernateSegments[segmentIndex].Chain = new LinkChain<Link>();
            }
            _threadPool = new Instinct_.Pattern.FrugalThreadPool(4, delegate()
            {
                return new ThreadContext(this);
            });
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                _threadPool.Dispose();
            }
        }

        /// <summary>
        /// Gets the timeslice array.
        /// </summary>
        /// <value>The timeslice array.</value>
        public /*protected*/ Timeslice[] Timeslices
        {
            get { return _timeslices; }
        }

        /// <summary>
        /// Gets the hibernate list.
        /// </summary>
        /// <value>The hibernate list.</value>
        public /*protected*/ HibernateSegment[] HibernateSegments
        {
            get { return _hibernateSegments; }
        }

        /// <summary>
        /// Creates the context.
        /// </summary>
        /// <returns></returns>
        protected virtual TContext CreateContext()
        {
            return new TContext();
        }

        /// <summary>
        /// Updates the context.
        /// </summary>
        /// <param name="thisContext">The this context.</param>
        /// <param name="nextContext">The next context.</param>
        /// <param name="updateVectors">The update vectors.</param>
        protected abstract void UpdateContext(TContext thisContext, TContext nextContext, ulong updateVectors);
    }
}
