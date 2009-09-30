using NUnit.Framework;
using Instinct.Sample;
namespace Instinct.Time
{
    /// <summary>
    /// TimeEngineFixture
    /// </summary>
    public partial class TimeEngineFixture
    {
        /// <summary>
        /// Adds the value zero.
        /// </summary>
        [Test]
        [Category("Add")]
        public void AddValueZero()
        {
            var linkItem = new LinkItem();
            var listItem = new ListItem();
            using (var timeEngine = new TimeEngine())
            {
                timeEngine.AddValue(linkItem.Link, 0);
                timeEngine.AddValue(listItem, 0);
                var fractions = timeEngine.Timeslices[0].Fractions;
                Assert.AreEqual(1, fractions[0].Chain.Count);
                Assert.AreEqual(1, fractions[0].List.Count);
                Assert.AreEqual(linkItem.Link, fractions[0].Chain.Head);
                Assert.AreEqual(listItem, fractions[0].List[0]);
            }
        }

        /// <summary>
        /// Adds the value half time.
        /// </summary>
        [Test]
        [Category("Add")]
        public void AddValueHalfTime()
        {
            var linkItem = new LinkItem();
            var listItem = new ListItem();
            ulong time = (TimePrecision.TimeScaler >> 1);
            using (var timeEngine = new TimeEngine())
            {
                timeEngine.AddValue(linkItem.Link, time);
                timeEngine.AddValue(listItem, time);
                //+
                var fractions = timeEngine.Timeslices[0].Fractions;
                ulong fractionTime = (time & TimePrecision.TimePrecisionMask);
                Assert.AreEqual(1, fractions[fractionTime].Chain.Count);
                Assert.AreEqual(1, fractions[fractionTime].List.Count);
                Assert.AreEqual(linkItem.Link, fractions[fractionTime].Chain.Head);
                Assert.AreEqual(listItem, fractions[fractionTime].List[0]);
            }
        }

        /// <summary>
        /// Adds the value whole time.
        /// </summary>
        [Test]
        [Category("Add")]
        public void AddValueWholeTime()
        {
            var linkItem = new LinkItem();
            var listItem = new ListItem();
            ulong time = TimePrecision.TimeScaler;
            using (var timeEngine = new TimeEngine())
            {
                timeEngine.AddValue(linkItem.Link, time);
                timeEngine.AddValue(listItem, time);
                //+
                var fractions = timeEngine.Timeslices[1].Fractions;
                ulong fractionTime = (time & TimePrecision.TimePrecisionMask);
                Assert.AreEqual(1, fractions[fractionTime].Chain.Count);
                Assert.AreEqual(1, fractions[fractionTime].List.Count);
                Assert.AreEqual(linkItem.Link, fractions[fractionTime].Chain.Head);
                Assert.AreEqual(listItem, fractions[fractionTime].List[0]);
            }
        }

        /// <summary>
        /// Adds the value hibernate time.
        /// </summary>
        [Test]
        [Category("Add")]
        public void AddValueHibernateTime()
        {
            var linkItem = new LinkItem();
            var listItem = new ListItem();
            ulong time = (TimeSettings.MaxTimeslices << TimePrecision.TimePrecisionBits);
            using (var timeEngine = new TimeEngine())
            {
                timeEngine.AddValue(linkItem.Link, time);
                timeEngine.AddValue(listItem, time);
                //+
                var fractions = timeEngine.Timeslices[0].Fractions;
                ulong fractionTime = (time & TimePrecision.TimePrecisionMask);
                Assert.AreEqual(0, fractions.Count);
                //+
                var hibernateSegment = timeEngine.HibernateSegments[0];
                Assert.AreEqual(1, hibernateSegment.Chain.Count);
                Assert.AreEqual(1, hibernateSegment.List.Count);
                Assert.AreEqual(linkItem.Link, hibernateSegment.Chain.Head);
                Assert.AreEqual(listItem, hibernateSegment.List.First.Value.Object);
            }
        }
    }
}
