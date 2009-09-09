using System.Collections.Generic;
using Instinct_.Pattern;
namespace Instinct.Time
{
    /// <summary>
    /// Timeline
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public abstract class TimeEngine<TElement, TContext> : Disposeable
        where TElement : FrugalThreadPool.IThreadWork
        where TContext : new()
    {
        private const int TimesliceArraySize = 1000;
        private static readonly ReverseComparer s_reverseComparer = new ReverseComparer();
        private const ulong TimesliceArrayTime = (TimesliceArraySize << TimePrecision.TimePrecisionBits);
        //+
        private ulong _timesliceIndex = 0;
        private LinkedList<Hibernate> _hibernateList = new LinkedList<Hibernate>();
        private TimeLinkBase _hibernateLink;
        private Timeslice[] _timeslices = new Timeslice[TimesliceArraySize];
        private ContextChange _contextChangeLink;
        private int _contextChangeLinkCount = 0;

        #region Class Types
        /// <summary>
        /// Context
        /// </summary>
        public class ContextChange : FrugalThreadPool.IThreadWork
        {
            //private const int ContextArraySize = 100;
            //public TContext[] Contexts = new TContext[ContextArraySize];
            //+
            public ulong Time;
            public TContext This;
            public TContext Next;
            public ContextChange NextContextChange;
            public ulong UpdateVectors;
            private TimeEngine<TElement, TContext> _parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="TimeEngine&lt;TElement, TContext&gt;.Context&lt;TContext&gt;"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public ContextChange(TimeEngine<TElement, TContext> parent)
            {
                _parent = parent;
            }

            /// <summary>
            /// Evaluates the time.
            /// </summary>
            public void Execute()
            {
                _parent.UpdateContext(This, Next, UpdateVectors);
            }
        }

        /// <summary>
        /// TimeLinkHead
        /// </summary>
        public class LinkHead
        {
            public int Count;
            public TimeLinkBase Chain;

            /// <summary>
            /// Initializes a new instance of the <see cref="Head"/> struct.
            /// </summary>
            /// <param name="object">The @object.</param>
            public LinkHead(TimeLinkBase @object)
            {
                Count = 1;
                Chain = @object;
            }
        }

        /// <summary>
        /// Timeslice
        /// </summary>
        public class Timeslice
        {
            public Dictionary<ulong, List<TElement>> List = new Dictionary<ulong, List<TElement>>();
            public Dictionary<ulong, LinkHead> Link = new Dictionary<ulong, LinkHead>();
        }

        /// <summary>
        /// Hibernate
        /// </summary>
        public struct Hibernate
        {
            public ulong Time;
            public TElement Object;
        }

        /// <summary>
        /// ReverseComparer
        /// </summary>
        private class ReverseComparer : IComparer<ulong>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// Value
            /// Condition
            /// Less than zero
            /// <paramref name="x"/> is less than <paramref name="y"/>.
            /// Zero
            /// <paramref name="x"/> equals <paramref name="y"/>.
            /// Greater than zero
            /// <paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            public int Compare(ulong x, ulong y)
            {
                if (x < y)
                {
                    return 1;
                }
                if (x > y)
                {
                    return -1;
                }
                return 0;
            }
        }
        #endregion Class Types

        /// <summary>
        /// Initializes a new instance of the <see cref="Timeline&lt;T&gt;"/> class.
        /// </summary>
        public TimeEngine()
        {
            for (int timesliceIndex = 0; timesliceIndex < _timeslices.Length; timesliceIndex++)
            {
                _timeslices[timesliceIndex] = new Timeslice();
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
                _threadPool.Dispose();
            }
        }

        /// <summary>
        /// Gets the timeslice array.
        /// </summary>
        /// <value>The timeslice array.</value>
        protected Timeslice[] Timeslices
        {
            get { return _timeslices; }
        }

        /// <summary>
        /// Gets the hibernate list.
        /// </summary>
        /// <value>The hibernate list.</value>
        protected LinkedList<Hibernate> HibernateList
        {
            get { return _hibernateList; }
        }

        /// <summary>
        /// Gets the hibernate link.
        /// </summary>
        /// <value>The hibernate link.</value>
        protected TimeLinkBase HibernateLink
        {
            get { return _hibernateLink; }
        }

        #region Add
        private object _addListLock = new object();
        private object _addLinkLock = new object();
        private object _addContextLock = new object();

        /// <summary>
        /// Adds the specified @object.
        /// </summary>
        /// <param name="object">The @object.</param>
        /// <param name="time">The time.</param>
        public void Add(TElement @object, ulong time)
        {
            //System.Console.WriteLine("Timeline: Add {" + time.ToString() + "}");
            ulong fractionTime = (time & TimePrecision.TimePrecisionMask);
            ulong timeslice = (time >> TimePrecision.TimePrecisionBits);
            if (timeslice < TimesliceArraySize)
            {
                //+ time is fractional only
                if ((timeslice == 0) && (fractionTime < _listMaxWorkingFraction)) //+ could check for existance in hash
                {
                    _isRebuildList = true;
                }
                //+ roll timeslice for index
                timeslice += _timesliceIndex;
                if (timeslice >= TimesliceArraySize)
                {
                    timeslice -= TimesliceArraySize;
                }
                //+ add to list
                Dictionary<ulong, List<TElement>> list = _timeslices[timeslice].List;
                lock (_addListLock)
                {
                    List<TElement> value;
                    if (list.TryGetValue(fractionTime, out value) == false)
                    {
                        value = new List<TElement>(5);
                        list.Add(fractionTime, value);
                    }
                    value.Add(@object);
                }
            }
            else
            {
                System.Console.WriteLine("Timeline: Hibernate " + time.ToString());
                //+ add to Hibernate
                lock (_addListLock)
                {
                    _hibernateList.AddFirst(new Hibernate { Time = time, Object = @object });
                }
            }
        }

        /// <summary>
        /// Adds the specified @object.
        /// </summary>
        /// <param name="object">The @object.</param>
        /// <param name="time">The time.</param>
        public void Add(TimeLinkBase @object, ulong time)
        {
            //System.Console.WriteLine("Timeline: Add {" + time.ToString() + "}");
            ulong fractionTime = (time & TimePrecision.TimePrecisionMask);
            ulong timeslice = (time >> TimePrecision.TimePrecisionBits);
            if (timeslice < TimesliceArraySize)
            {
                //+ time is fractional only
                if ((timeslice == 0) && (fractionTime < _linkMaxWorkingFraction)) //+ could check for existance in hash
                {
                    _isRebuildLink = true;
                }
                //+ roll timeslice for index
                timeslice += _timesliceIndex;
                if (timeslice >= TimesliceArraySize)
                {
                    timeslice -= TimesliceArraySize;
                }
                //+ add to link
                Dictionary<ulong, LinkHead> link = _timeslices[timeslice].Link;
                lock (_addLinkLock)
                {
                    LinkHead value;
                    if (link.TryGetValue(fractionTime, out value) == false)
                    {
                        @object.NextTimeLink = null;
                        link.Add(fractionTime, new LinkHead(@object));
                    }
                    else
                    {
                        value.Count++;
                        TimeLinkBase chain = value.Chain;
                        @object.NextTimeLink = chain.NextTimeLink;
                        chain.NextTimeLink = @object;
                    }
                }
            }
            else
            {
                System.Console.WriteLine("Timeline: Hibernate " + time.ToString());
                //+ add to Hibernate
                lock (_addLinkLock)
                {
                    @object.HibernateTime = time;
                    @object.NextTimeLink = _hibernateLink;
                    _hibernateLink = @object;
                }
            }
        }

        /// <summary>
        /// Adds the state change.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stateFlag">The state flag.</param>
        public void AddContextChange(ContextChange change, ulong updateVectors)
        {
            if ((change.NextContextChange != null) || (_contextChangeLink == change))
            {
                throw new System.InvalidOperationException(); //+ paranoia
            }
            lock (_addContextLock)
            {
                _contextChangeLinkCount++;
                change.UpdateVectors = updateVectors;
                change.NextContextChange = _contextChangeLink;
                _contextChangeLink = change;
            }
        }
        #endregion Add

        #region EvaluateFrame
        //+
        private Instinct_.Pattern.FrugalThreadPool _threadPool = new Instinct_.Pattern.FrugalThreadPool(4);
        private ulong[] _listWorkingFractionArray = new ulong[TimePrecision.LinkWorkingFractionSize];
        private ulong[] _linkWorkingFractionArray = new ulong[TimePrecision.ListWorkingFractionSize];
        private ulong _listMaxWorkingFraction;
        private ulong _linkMaxWorkingFraction;
        private bool _isRebuildLink;
        private bool _isRebuildList;

        /// <summary>
        /// Des the key list_.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="hash">The hash.</param>
        /// <param name="workingFractionArray">The working fraction array.</param>
        /// <param name="workingFractionIndex">Index of the working fraction.</param>
        /// <returns></returns>
        private static ulong FillWorkingFractionArray<TValue>(Dictionary<ulong, TValue> hash, ref ulong[] workingFractionArray, int workingFractionLength, ref int workingFractionIndex, ref ulong maxWorkingFraction)
        {
            if (hash.Count > 0)
            {
                List<ulong> fractionList = new List<ulong>(hash.Keys);
                fractionList.Sort(s_reverseComparer);
                ulong[] fractionArray = fractionList.ToArray();
                int fractionArrayLength = fractionArray.Length;
                workingFractionIndex = System.Math.Min(fractionArrayLength, workingFractionLength);
                System.Array.Copy(fractionArray, fractionArrayLength - workingFractionIndex, workingFractionArray, 0, workingFractionIndex);
                maxWorkingFraction = workingFractionArray[0];
                return workingFractionArray[--workingFractionIndex];
            }
            else
            {
                return ulong.MaxValue;
            }
        }

        /// <summary>
        /// Evaluates the frame.
        /// </summary>
        /// <param name="mSecond">The m second.</param>
        public void EvaluateFrame(ulong time)
        {
            //System.Console.WriteLine("Timeline: EvaluateFrame {" + time.ToString() + "}");
            _isRebuildList = true;
            _isRebuildLink = true;
            ulong listFractionTime;
            ulong linkFractionTime;
            int listFractionTimeIndex = 0;
            int linkFractionTimeIndex = 0;
            bool isFirstLoop = true;
            while (true)
            {
                //+ check for escape
                if (time <= 0)
                {
                    return;
                }
                //+ new timelice
                Timeslice timeslice = _timeslices[_timesliceIndex];
                Dictionary<ulong, List<TElement>> list = timeslice.List;
                Dictionary<ulong, LinkHead> link = timeslice.Link;
                listFractionTime = FillWorkingFractionArray<List<TElement>>(list, ref _listWorkingFractionArray, TimePrecision.ListWorkingFractionSize, ref listFractionTimeIndex, ref _listMaxWorkingFraction);
                linkFractionTime = FillWorkingFractionArray<LinkHead>(link, ref _linkWorkingFractionArray, TimePrecision.LinkWorkingFractionSize, ref linkFractionTimeIndex, ref _linkMaxWorkingFraction);
                //+ process time-slices
                ulong fractionTime = System.Math.Min(listFractionTime, linkFractionTime);
                if (isFirstLoop == true)
                {
                    isFirstLoop = false;
                    time += fractionTime;
                }
                ulong lastFractionTime = 0;
                while (fractionTime < ulong.MaxValue)
                {
                    //+ advance time-slice + check for escape
                    time -= (fractionTime - lastFractionTime);
                    if (time <= 0)
                    {
                        return;
                    }
                    //+ evaluate time
                    #region Evalutate Time
                    if (listFractionTime == fractionTime)
                    {
                        List<TElement> objectList = list[listFractionTime];
                        int objectIndex;
                        int objectListCount = objectList.Count;
                        for (objectIndex = 0; objectIndex <= (objectListCount - 5); objectIndex += 5)
                        {
                            _threadPool.Add(objectList.GetRange(objectIndex, 5));
                        }
                        if (objectIndex != objectListCount)
                        {
                            _threadPool.Add(objectList.GetRange(objectIndex, objectListCount - objectIndex));
                        }
                        //+ next
                        list.Remove(listFractionTime);
                        listFractionTime = (listFractionTimeIndex > 0 ? _listWorkingFractionArray[--listFractionTimeIndex] : ulong.MaxValue);
                    }
                    if (linkFractionTime == fractionTime)
                    {
                        TimeLinkBase nextItem;
                        LinkHead head = link[linkFractionTime];
                        TimeLinkBase item = head.Chain;
                        int itemCount = head.Count;
                        int itemIndex;
                        for (itemIndex = 0; itemIndex <= (itemCount - 5); itemIndex += 5)
                        {
                            TimeLinkBase item0 = item; nextItem = item0.NextTimeLink; item0.NextTimeLink = null; //+ paranoia
                            TimeLinkBase item1 = nextItem; nextItem = item1.NextTimeLink; item1.NextTimeLink = null; //+ paranoia
                            TimeLinkBase item2 = nextItem; nextItem = item2.NextTimeLink; item2.NextTimeLink = null; //+ paranoia
                            TimeLinkBase item3 = nextItem; nextItem = item3.NextTimeLink; item3.NextTimeLink = null; //+ paranoia
                            TimeLinkBase item4 = nextItem; nextItem = item4.NextTimeLink; item4.NextTimeLink = null; //+ paranoia
                            item = nextItem;
                            _threadPool.Add(new TimeLinkBase[] { item0, item1, item2, item3, item4 });
                        }
                        if (itemIndex != itemCount)
                        {
                            itemCount -= itemIndex;
                            TimeLinkBase[] itemArray = new TimeLinkBase[itemCount];
                            itemIndex = 0;
                            //for (; itemIndex < itemCount; /*item != null;*/ item = nextItem)
                            //for (; item != null; item = nextItem)
                            for (; itemIndex < itemCount; item = nextItem)
                            {
                                nextItem = item.NextTimeLink; item.NextTimeLink = null; //+ paranoia
                                itemArray[itemIndex++] = item;
                            }
                            _threadPool.Add(itemArray);
                        }
                        //+ next
                        link.Remove(linkFractionTime);
                        linkFractionTime = (linkFractionTimeIndex > 0 ? _linkWorkingFractionArray[--linkFractionTimeIndex] : ulong.MaxValue);
                    }
                    #endregion Evalutate Time
                    _threadPool.Join();
                    //System.Console.WriteLine("Timeslice: " + time.ToString());
                    //+ process state-change
                    #region Process State-Change
                    ContextChange nextContext;
                    //for (Context context = m_contextChangeLink; context != null; context = nextContext)
                    //{
                    //    nextContext = context.NextContext;
                    //    context.NextContext = null; //+ paranoia
                    //    context.Execute();
                    //}
                    ContextChange context = _contextChangeLink;
                    int contextCount = _contextChangeLinkCount;
                    int contextIndex;
                    for (contextIndex = 0; contextIndex <= (contextCount - 5); contextIndex += 5)
                    {
                        ContextChange context0 = context; nextContext = context0.NextContextChange; context0.NextContextChange = null; //+ paranoia
                        ContextChange context1 = nextContext; nextContext = context1.NextContextChange; context1.NextContextChange = null; //+ paranoia
                        ContextChange context2 = nextContext; nextContext = context2.NextContextChange; context2.NextContextChange = null; //+ paranoia
                        ContextChange context3 = nextContext; nextContext = context3.NextContextChange; context3.NextContextChange = null; //+ paranoia
                        ContextChange context4 = nextContext; nextContext = context4.NextContextChange; context4.NextContextChange = null; //+ paranoia
                        context = nextContext;
                        _threadPool.Add(new ContextChange[] { context0, context1, context2, context3, context4 });
                    }
                    if (contextIndex != contextCount)
                    {
                        contextCount -= contextIndex;
                        ContextChange[] contextArray = new ContextChange[contextCount];
                        contextIndex = 0;
                        //for (; contextIndex < itemCount; /*context != null;*/ context = nextContext)
                        //for (; context != null; context = nextContext)
                        for (; contextIndex < contextCount; context = nextContext)
                        {
                            nextContext = context.NextContextChange; context.NextContextChange = null; //+ paranoia
                            contextArray[contextIndex++] = context;
                        }
                        _threadPool.Add(contextArray);
                    }
                    //+ clear
                    _contextChangeLink = null;
                    _contextChangeLinkCount = 0;
                    #endregion Process State-Change
                    //+
                    if ((_isRebuildList == true) || (_listMaxWorkingFraction == fractionTime))
                    {
                        _isRebuildList = false;
                        listFractionTime = FillWorkingFractionArray<List<TElement>>(list, ref _listWorkingFractionArray, TimePrecision.ListWorkingFractionSize, ref listFractionTimeIndex, ref _listMaxWorkingFraction);
                    }
                    if ((_isRebuildLink == true) || (_linkMaxWorkingFraction == fractionTime))
                    {
                        _isRebuildLink = false;
                        linkFractionTime = FillWorkingFractionArray<LinkHead>(link, ref _linkWorkingFractionArray, TimePrecision.LinkWorkingFractionSize, ref linkFractionTimeIndex, ref _linkMaxWorkingFraction);
                    }
                    lastFractionTime = fractionTime;
                    fractionTime = System.Math.Min(listFractionTime, linkFractionTime);
                    _threadPool.Join();
                }
                //+ advance time
                _isRebuildList = true;
                _isRebuildLink = true;
                time -= (TimePrecision.TimeScaler - lastFractionTime);
                _timesliceIndex++;
                if (_timesliceIndex >= TimesliceArraySize)
                {
                    _timesliceIndex = 0;
                    //System.Console.WriteLine("Timeline: ccycle");
                    //+ dehibernate
                    #region Dehibernate
                    for (LinkedListNode<Hibernate> hibernateNode = _hibernateList.First; hibernateNode != null; hibernateNode = hibernateNode.Next)
                    {
                        Hibernate hibernate = hibernateNode.Value;
                        ulong newTime = (hibernate.Time -= TimesliceArrayTime);
                        if (newTime < TimesliceArrayTime)
                        {
                            if (newTime < 0)
                            {
                                throw new System.InvalidOperationException(); //+ paranoia
                            }
                            //+ remove node
                            _hibernateList.Remove(hibernateNode);
                            //+ add to timeline
                            System.Console.WriteLine("Timeline: Dehibernate {" + newTime.ToString() + "}");
                            Add(hibernate.Object, newTime);
                        }
                        else
                        {
                            hibernateNode.Value = hibernate;
                        }
                    }
                    TimeLinkBase lastObject = _hibernateLink;
                    for (TimeLinkBase @object = _hibernateLink; @object != null; @object = @object.NextTimeLink)
                    {
                        ulong newTime = (@object.HibernateTime -= TimesliceArrayTime);
                        if (newTime < TimesliceArrayTime)
                        {
                            if (newTime < 0)
                            {
                                throw new System.InvalidOperationException(); //+ paranoia
                            }
                            //+ remove node
                            //lastObject = (m_hibernateLink != @object ? (lastObject.NextTimeLink = @object.NextTimeLink) : (m_hibernateLink = @object.NextTimeLink));
                            lastObject = (lastObject.NextTimeLink = @object.NextTimeLink);
                            @object.NextTimeLink = null; //+ paranoia
                            //+ add to timeline
                            System.Console.WriteLine("Timeline: Dehibernate {" + newTime.ToString() + "}");
                            Add(@object, newTime);
                        }
                        else
                        {
                            lastObject = @object;
                        }
                    }
                }
                    #endregion Dehibernate
            }
        }
        #endregion EvaluateFrame

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
        public abstract void UpdateContext(TContext thisContext, TContext nextContext, ulong updateVectors);
    }
}
