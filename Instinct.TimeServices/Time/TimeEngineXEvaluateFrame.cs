using System.Collections.Generic;
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
        private static readonly ReverseComparer s_reverseComparer = new ReverseComparer();

        /// <summary>
        /// Builds the working fractions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="workingFractions">The working fractions.</param>
        /// <param name="workingFractionIndex">Index of the working fraction.</param>
        /// <param name="maxWorkingFraction">The max working fraction.</param>
        /// <returns></returns>
        private static ulong BuildWorkingFractions<T>(SortedDictionary<ulong, T> dictionary, ulong[] workingFractions, ref int workingFractionIndex, ref ulong minWorkingFraction, ref ulong maxWorkingFraction)
        {
            if (dictionary.Count > 0)
            {
                var keyCollection = dictionary.Keys;
                ulong[] fractions = new ulong[keyCollection.Count];
                keyCollection.CopyTo(fractions, 0);
                System.Array.Sort(fractions, s_reverseComparer);
                int fractionsLength = fractions.Length;
                workingFractionIndex = System.Math.Min(fractionsLength, TimeSettings.MaxTimeslices);
                System.Array.Copy(fractions, fractionsLength - workingFractionIndex, workingFractions, 0, workingFractionIndex);
                maxWorkingFraction = workingFractions[0];
                return workingFractions[--workingFractionIndex];
            }
            return ulong.MaxValue;
        }

        /// <summary>
        /// Evaluates the frame.
        /// </summary>
        /// <param name="mSecond">The m second.</param>
        public void EvaluateFrame(long time)
        {
            //System.Console.WriteLine("Timeline: EvaluateFrame {" + time.ToString() + "}");
            unchecked
            {
                _isRebuildWorkingFractions = true;
                ulong fractionTime;
                int fractionTimeIndex = 0;
                bool isFirstLoop = true;
                while (true)
                {
                    if (time <= 0)
                    {
                        //+ return if frame completed
                        return;
                    }
                    var fractions = _timeslices[_timesliceIndex].Fractions;
                    if (fractions.Count == 0)
                    {
                        //+ no fractions available, advance a whole time
                        _isRebuildWorkingFractions = true;
                        time -= (long)TimePrecision.TimeScaler;
                    }
                    else
                    {
                        ulong lastFractionTime = 0;
                        //+ work fractions
                        fractionTime = BuildWorkingFractions(fractions, _workingFractions, ref fractionTimeIndex, ref _minWorkingFraction, ref _maxWorkingFraction);
                        //+ first-time time adjust
                        if (isFirstLoop == true)
                        {
                            isFirstLoop = false;
                            time += (long)fractionTime;
                        }
                        //+ process time-slices
                        while (fractionTime < ulong.MaxValue)
                        {
                            //+ advance time-slice & check for escape
                            time -= (long)(fractionTime - lastFractionTime);
                            if (time <= 0)
                            {
                                return;
                            }
                            //+ next
                            var node = fractions[fractionTime];
                            fractions.Remove(fractionTime);  //_minWorkingFraction = fractionTime; //: fractions.Remove(fractionTime);
                            fractionTime = (fractionTimeIndex > 0 ? _workingFractions[--fractionTimeIndex] : ulong.MaxValue); //: repnz requires one less register
                            //+ evaluate time
                            EvaluateTime(node);
                            _threadPool.Join();
                            //+ apply pending adds and state-changes
                            ApplyPendingAddsAndChanges();
                            //+ build working-fractions
                            if ((_isRebuildWorkingFractions == true) || (_maxWorkingFraction == fractionTime))
                            {
                                _isRebuildWorkingFractions = false;
                                fractionTime = BuildWorkingFractions(fractions, _workingFractions, ref fractionTimeIndex, ref _minWorkingFraction, ref _maxWorkingFraction);
                            }
                            _threadPool.Join();
                        }
                        //+ advance time
                        _isRebuildWorkingFractions = true;
                        time -= (long)(TimePrecision.TimeScaler - lastFractionTime);
                    }
                    //+ next slice
                    _timesliceIndex++;
                    if (_timesliceIndex >= TimeSettings.MaxTimeslices)
                    {
                        _timesliceIndex = 0;
                        DehibernateAnyValues();
                    }
                }
            }
        }

        /// <summary>
        /// Evaluates the time.
        /// </summary>
        /// <param name="node">The node.</param>
        private void EvaluateTime(TimesliceNode node)
        {
            unchecked
            {
                var list = node.List;
                if (list != null)
                {
                    int listIndex;
                    int listCount = list.Count;
                    for (listIndex = 0; listIndex <= (listCount - 5); listIndex += 5)
                    {
                        _threadPool.Add(list.GetRange(listIndex, 5));
                    }
                    if (listIndex != listCount)
                    {
                        _threadPool.Add(list.GetRange(listIndex, listCount - listIndex));
                    }
                }
                var chain = node.Chain;
                if (chain.Count > 0)
                {
                    Link nextItem;
                    //+
                    Link item = chain.Head;
                    int linkCount = chain.Count;
                    int linkIndex;
                    for (linkIndex = 0; linkIndex <= (linkCount - 5); linkIndex += 5)
                    {
                        Link item0 = item; nextItem = item0.NextLink; item0.NextLink = null; //+ paranoia
                        Link item1 = nextItem; nextItem = item1.NextLink; item1.NextLink = null; //+ paranoia
                        Link item2 = nextItem; nextItem = item2.NextLink; item2.NextLink = null; //+ paranoia
                        Link item3 = nextItem; nextItem = item3.NextLink; item3.NextLink = null; //+ paranoia
                        Link item4 = nextItem; nextItem = item4.NextLink; item4.NextLink = null; //+ paranoia
                        item = nextItem;
                        _threadPool.Add(new Link[] { item0, item1, item2, item3, item4 });
                    }
                    if (linkIndex != linkCount)
                    {
                        linkCount -= linkIndex;
                        Link[] links = new Link[linkCount];
                        linkIndex = 0;
                        //for (; itemIndex < itemCount; /*item != null;*/ item = nextItem)
                        //for (; item != null; item = nextItem)
                        for (; linkIndex < linkCount; item = nextItem)
                        {
                            nextItem = item.NextLink; item.NextLink = null; //+ paranoia
                            links[linkIndex++] = item;
                        }
                        _threadPool.Add(links);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the pending adds and changes.
        /// </summary>
        private void ApplyPendingAddsAndChanges()
        {
            //+ pending adds         
            foreach (var threadContext2 in _threadPool.ThreadContexts)
            {
                var threadContext = (ThreadContext)threadContext2;
                threadContext.Commands.Apply();
                LinkChain<ContextChange> changeChain = threadContext.ChangeChain;
                //+ pending changes first allows threads to run
                int changeChainCount = changeChain.Count;
                if (changeChainCount > 10)
                {
                    ApplyContextChangesOverThreadPool(changeChain);
                }
                else if (changeChainCount > 0)
                {
                    ApplyContextChangesInline(changeChain);
                }
            }
        }

        /// <summary>
        /// Applies the context changes inline.
        /// </summary>
        private void ApplyContextChangesInline(LinkChain<ContextChange> changeChain)
        {
            unchecked
            {
                ContextChange nextLink;
                for (ContextChange context = changeChain.Head; context != null; context = nextLink)
                {
                    nextLink = context.NextLink; context.NextLink = null; //+ paranoia
                    context.Execute(null);
                }
                //+ clear
                changeChain.Clear();
            }
        }

        /// <summary>
        /// Applies the context changes over thread pool.
        /// </summary>
        private void ApplyContextChangesOverThreadPool(LinkChain<ContextChange> changeChain)
        {
            unchecked
            {
                ContextChange nextLink;
                ContextChange context = changeChain.Head;
                int contextCount = changeChain.Count;
                int contextIndex;
                for (contextIndex = 0; contextIndex <= (contextCount - 5); contextIndex += 5)
                {
                    ContextChange context0 = context; nextLink = context0.NextLink; context0.NextLink = null; //+ paranoia
                    ContextChange context1 = nextLink; nextLink = context1.NextLink; context1.NextLink = null; //+ paranoia
                    ContextChange context2 = nextLink; nextLink = context2.NextLink; context2.NextLink = null; //+ paranoia
                    ContextChange context3 = nextLink; nextLink = context3.NextLink; context3.NextLink = null; //+ paranoia
                    ContextChange context4 = nextLink; nextLink = context4.NextLink; context4.NextLink = null; //+ paranoia
                    context = nextLink;
                    _threadPool.Add(new ContextChange[] { context0, context1, context2, context3, context4 });
                }
                if (contextIndex != contextCount)
                {
                    contextCount -= contextIndex;
                    ContextChange[] contextArray = new ContextChange[contextCount];
                    contextIndex = 0;
                    //for (; context != null; context = nextLink)
                    for (; contextIndex < contextCount; context = nextLink)
                    {
                        nextLink = context.NextLink; context.NextLink = null; //+ paranoia
                        contextArray[contextIndex++] = context;
                    }
                    _threadPool.Add(contextArray);
                }
                //+ clear
                changeChain.Clear();
            }
        }
    }
}
