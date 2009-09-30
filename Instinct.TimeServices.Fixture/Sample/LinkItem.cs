using Instinct.Time;
namespace Instinct.Sample
{
    /// <summary>
    /// LinkItem
    /// </summary>
    public class LinkItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkItem"/> class.
        /// </summary>
        public LinkItem()
        {
            Link = new Link();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public LinkItem(System.Action<TimeEngine.ThreadContext> action)
        {
            Link = new Link();
            Action = action;
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public System.Action<TimeEngine.ThreadContext> Action { get; set; }

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        /// <value>The link.</value>
        public Link Link { get; private set; }

        /// <summary>
        /// Evaluates the time.
        /// </summary>
        public void Execute(TimeEngine.ThreadContext threadContext)
        {
            Action(threadContext);
        }
    }
}
