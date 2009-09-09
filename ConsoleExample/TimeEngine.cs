namespace ConsoleExample
{
    /// <summary>
    /// TimeEngine
    /// </summary>
    public class TimeEngine : Instinct.Time.TimeEngine<Sample, SampleContext>
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
        protected override SampleContext CreateContext()
        {
            return new SampleContext();
        }

        /// <summary>
        /// Updates the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contextFlag">The context flag.</param>
        public override void UpdateContext(SampleContext thisContext, SampleContext nextContext, ulong updateVectors)
        {
            var vectors = (SampleContext.UpdateVector)updateVectors;
            string value;
            if (((vectors & SampleContext.UpdateVector.Value) != 0) && ((value = nextContext.Value) != null))
            {
                thisContext.Value = value;
            }
            System.ICloneable tag;
            if (((vectors & SampleContext.UpdateVector.Tag) != 0) && ((tag = nextContext.Tag) != null))
            {
                thisContext.Tag = (System.ICloneable)tag.Clone();
            }
        }
    }
}
