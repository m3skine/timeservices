using Instinct_.Pattern;
namespace Instinct.Time
{
    /// <summary>
    /// TimeLinkBase
    /// </summary>
    public abstract class TimeLinkBase : FrugalThreadPool.IThreadWork
    {
        /// <summary>
        /// Gets or sets the hibernate time.
        /// </summary>
        /// <value>The hibernate time.</value>
        public ulong HibernateTime;

        /// <summary>
        /// Gets or sets the next.
        /// </summary>
        /// <value>The next.</value>
        public TimeLinkBase NextTimeLink;

        /// <summary>
        /// Evaluates the time.
        /// </summary>
        public abstract void Execute();
    }
}
