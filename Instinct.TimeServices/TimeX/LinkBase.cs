using Instinct_.Pattern;
namespace Instinct.TimeX
{
    /// <summary>
    /// LinkBase
    /// </summary>
    public abstract class LinkBase : FrugalThreadPool.IThreadWork
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
        public LinkBase NextLink;

        /// <summary>
        /// Evaluates the time.
        /// </summary>
        public abstract void Execute(object threadContext);
    }
}
