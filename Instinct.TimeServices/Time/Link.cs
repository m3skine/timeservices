using System;
using Instinct_;
namespace Instinct.Time
{
    /// <summary>
    /// Link
    /// </summary>
    public class Link : LinkNode<Link>
    {
        /// <summary>
        /// Gets or sets the hibernate time.
        /// </summary>
        /// <value>The hibernate time.</value>
        public ulong HibernateTime;

        /// <summary>
        /// Evaluates the time.
        /// </summary>
        public Action Execute;
    }
}
