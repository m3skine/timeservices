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
        private Instinct_.Pattern.FrugalThreadPool _threadPool;

        #region Class Types
        /// <summary>
        /// ThreadContext
        /// </summary>
        public class ThreadContext
        {
            private TimeEngine<TListValue, TContext> _timeEngine;

            /// <summary>
            /// Initializes a new instance of the <see cref="TimeEngine&lt;TValue, TContext&gt;.ThreadContext"/> class.
            /// </summary>
            /// <param name="timeEngine">The time engine.</param>
            internal ThreadContext(TimeEngine<TListValue, TContext> timeEngine)
            {
                _timeEngine = timeEngine;
            }

            /// <summary>
            /// 
            /// </summary>
            internal CommandBinaryLog Commands = new CommandBinaryLog();
            /// <summary>
            /// 
            /// </summary>
            internal LinkChain<ContextChange> ChangeChain = new LinkChain<ContextChange>();

            /// <summary>
            /// Adds the value.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="time">The time.</param>
            public void AddValue(Link value, ulong time)
            {
            }

            /// <summary>
            /// Adds the value.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="time">The time.</param>
            public void AddValue(TListValue value, ulong time)
            {
            }

            /// <summary>
            /// Adds the change.
            /// </summary>
            /// <param name="change">The change.</param>
            /// <param name="updateVectors">The update vectors.</param>
            public void AddChange(ContextChange change, ulong updateVectors)
            {
                _timeEngine.AddChange(this, change, updateVectors);
            }
        }
        #endregion Class Types
    }
}
