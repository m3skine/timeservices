//using Instinct.Time;
//namespace Instinct.Sample
//{
//    /// <summary>
//    /// ListItem
//    /// </summary>
//    public class ListItem
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="ListItem"/> class.
//        /// </summary>
//        public ListItem()
//        {
//        }
//        /// <summary>
//        /// Initializes a new instance of the <see cref="ListItem"/> class.
//        /// </summary>
//        /// <param name="action">The action.</param>
//        public ListItem(System.Action<TimeEngine.ThreadContext> action)
//        {
//            Action = action;
//        }

//        /// <summary>
//        /// Gets or sets the action.
//        /// </summary>
//        /// <value>The action.</value>
//        public System.Action<TimeEngine.ThreadContext> Action { get; set; }

//        /// <summary>
//        /// Evaluates the time.
//        /// </summary>
//        public void Execute(TimeEngine.ThreadContext threadContext)
//        {
//            Action(threadContext);
//        }
//    }
//}
