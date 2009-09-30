using System.Collections.Generic;
namespace Instinct.Time
{
    /// <summary>
    /// Timeline
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public abstract partial class TimeEngine<TListValue, TContext>
    {
        /// <summary>
        /// Adds the specified @object.
        /// </summary>
        /// <param name="object">The @object.</param>
        /// <param name="time">The time.</param>
        public void AddValue(Link value, ulong time)
        {
            //System.Console.WriteLine("Timeline: Add {" + time.ToString() + "}");
            unchecked
            {
                ulong timeslice = (time >> TimePrecision.TimePrecisionBits);
                ulong fractionTime = (time & TimePrecision.TimePrecisionMask);
                if (timeslice < TimeSettings.MaxTimeslices)
                {
                    ////+ time is fractional only
                    ////+ enhance: could check for existance in hash
                    if ((timeslice == 0) && (fractionTime < _maxWorkingFraction))
                    {
                        _isRebuildWorkingFractions = true;
                    }
                    //+ roll timeslice for index
                    timeslice += _timesliceIndex;
                    if (timeslice >= TimeSettings.MaxTimeslices)
                    {
                        timeslice -= TimeSettings.MaxTimeslices;
                    }
                    var fractions = _timeslices[timeslice].Fractions;
                    //+ add to list
                    TimesliceNode node;
                    if (fractions.TryGetValue(fractionTime, out node) == false)
                    {
                        value.NextLink = null;
                        fractions.Add(fractionTime, new TimesliceNode(value));
                    }
                    else
                    {
                        node.Chain.AddFirst(value);
                    }
                    return;
                }
            }
            HibernateValue(value, time);
        }

        /// <summary>
        /// Adds the specified @object.
        /// </summary>
        /// <param name="object">The @object.</param>
        /// <param name="time">The time.</param>
        public void AddValue(TListValue value, ulong time)
        {
            //System.Console.WriteLine("Timeline: Add {" + time.ToString() + "}");
            unchecked
            {
                ulong timeslice = (time >> TimePrecision.TimePrecisionBits);
                ulong fractionTime = (time & TimePrecision.TimePrecisionMask);
                if (timeslice < TimeSettings.MaxTimeslices)
                {
                    ////+ time is fractional only
                    ////+ enhance: could check for existance in hash
                    if ((timeslice == 0) && (fractionTime < _maxWorkingFraction))
                    {
                        _isRebuildWorkingFractions = true;
                    }
                    //+ roll timeslice for index
                    timeslice += _timesliceIndex;
                    if (timeslice >= TimeSettings.MaxTimeslices)
                    {
                        timeslice -= TimeSettings.MaxTimeslices;
                    }
                    var fractions = _timeslices[timeslice].Fractions;
                    //+ add to list
                    TimesliceNode node;
                    if (fractions.TryGetValue(fractionTime, out node) == false)
                    {
                        fractions.Add(fractionTime, new TimesliceNode(value));
                    }
                    else
                    {
                        var list = node.List;
                        if (list == null)
                        {
                            list = node.List = new List<TListValue>(5);
                        }
                        list.Add(value);
                    }
                    return;
                }
            }
            HibernateValue(value, time);
        }

        /// <summary>
        /// Adds the state change.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stateFlag">The state flag.</param>
        internal void AddChange(ThreadContext context, ContextChange change, ulong updateVectors)
        {
            change.UpdateVectors = updateVectors;
            context.ChangeChain.AddFirst(change);
        }
    }
}
