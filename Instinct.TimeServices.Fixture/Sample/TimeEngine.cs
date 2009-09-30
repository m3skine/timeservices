using Instinct.Time;
namespace Instinct.Sample
{
    /// <summary>
    /// TimeEngine
    /// </summary>
    public class TimeEngine : TimeEngine<ListItem, SimpleContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeEngine"/> class.
        /// </summary>
        public TimeEngine()
            : base()
        {
        }

        /// <summary>
        /// Creates the context.
        /// </summary>
        /// <returns></returns>
        protected override SimpleContext CreateContext()
        {
            return new SimpleContext();
        }

        /// <summary>
        /// Updates the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contextFlag">The context flag.</param>
        protected override void UpdateContext(SimpleContext thisContext, SimpleContext nextContext, ulong updateVectors)
        {
            var vectors = (SimpleContext.UpdateVector)updateVectors;
            string value;
            if (((vectors & SimpleContext.UpdateVector.Value) != 0) && ((value = nextContext.Value) != null))
            {
                thisContext.Value = value;
            }
            System.ICloneable tag;
            if (((vectors & SimpleContext.UpdateVector.Tag) != 0) && ((tag = nextContext.Tag) != null))
            {
                thisContext.Tag = (System.ICloneable)tag.Clone();
            }
        }
    }
}
