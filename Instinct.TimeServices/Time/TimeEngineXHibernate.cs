using System.Collections.Generic;
using System.Runtime.InteropServices;
using Instinct_;
namespace Instinct.Time
{
    /// <summary>
    /// Timeline
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public abstract partial class TimeEngine<TListValue, TContext>
    {
        private HibernateSegment[] _hibernateSegments = new HibernateSegment[TimeSettings.MaxHibernateSegments];

        #region Class Types
        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public /*protected*/ struct HibernateSegment
        {
            /// <summary>
            /// 
            /// </summary>
            public LinkedList<HibernateState> List;
            /// <summary>
            /// 
            /// </summary>
            public LinkChain<Link> Chain;
        }

        /// <summary>
        /// HibernateState
        /// </summary>
        public /*protected*/ class HibernateState
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong Time;
            /// <summary>
            /// 
            /// </summary>
            public TListValue Object;
        }
        #endregion Class Types

        /// <summary>
        /// Hibernates the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="time">The time.</param>
        private void HibernateValue(Link value, ulong time)
        {
            //System.Console.WriteLine("Timeline: Hibernate " + time.ToString());
            value.HibernateTime = time;
            _hibernateSegments[0].Chain.AddFirst(value);
        }

        /// <summary>
        /// Hibernates the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="time">The time.</param>
        private void HibernateValue(TListValue value, ulong time)
        {
            //System.Console.WriteLine("Timeline: Hibernate " + time.ToString());
            _hibernateSegments[0].List.AddFirst(new HibernateState { Time = time, Object = value });
        }

        /// <summary>
        /// Dehibernates any values.
        /// </summary>
        private void DehibernateAnyValues()
        {
            var list = _hibernateSegments[0].List;
            if (list.Count > 0)
            {
                for (var node = list.First; node != null; node = node.Next)
                {
                    var hibernateState = node.Value;
                    if (hibernateState.Time < MaxTimeslicesTime)
                    {
                        throw new System.InvalidOperationException(); //+ paranoia
                    }
                    ulong newTime = (hibernateState.Time -= MaxTimeslicesTime);
                    if (newTime < MaxTimeslicesTime)
                    {
                        //System.Console.WriteLine("Timeline: Dehibernate {" + newTime.ToString() + "}");
                        //+ remove node
                        list.Remove(node);
                        //+ add to timeline
                        AddValue(hibernateState.Object, newTime);
                    }
                }
            }
            var chain = _hibernateSegments[0].Chain;
            if (chain.Count > 0)
            {
                Link lastItem = chain.Head;
                for (Link nextItem = lastItem; nextItem != null; nextItem = nextItem.NextLink)
                {
                    if (nextItem.HibernateTime < MaxTimeslicesTime)
                    {
                        throw new System.InvalidOperationException(); //+ paranoia
                    }
                    ulong newTime = (nextItem.HibernateTime -= MaxTimeslicesTime);
                    if (newTime < MaxTimeslicesTime)
                    {
                        //System.Console.WriteLine("Timeline: Dehibernate {" + newTime.ToString() + "}");   
                        //+ remove node
                        //lastObject = (m_hibernateLink != @object ? (lastObject.NextTimeLink = @object.NextTimeLink) : (m_hibernateLink = @object.NextTimeLink));
                        lastItem = (lastItem.NextLink = nextItem.NextLink); nextItem.NextLink = null; //+ paranoia
                        //+ add to timeline
                        AddValue(nextItem, newTime);
                    }
                    else
                    {
                        lastItem = nextItem;
                    }
                }
            }
        }
    }
}
