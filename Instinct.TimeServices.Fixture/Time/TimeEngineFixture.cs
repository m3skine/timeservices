using NUnit.Framework;
using Instinct.Sample;
namespace Instinct.Time
{
    /// <summary>
    /// TimeEngineFixture
    /// </summary>
    [TestFixture]
    public partial class TimeEngineFixture
    {
        /// <summary>
        /// Creates the instance.
        /// </summary>
        [Test]
        public void CreateInstance()
        {
            using (var timeEngine = new TimeEngine()) { };
        }

        /// <summary>
        /// Adds the item test execute.
        /// </summary>
        [Test]
        [Category("FrameTest")]
        public void AddItemTestExecute()
        {
            int linkCompleted = 0;
            var linkItem = new LinkItem(delegate(TimeEngine.ThreadContext threadContext)
            {
                linkCompleted++;
            });
            int listCompleted = 0;
            var listItem = new ListItem(delegate(TimeEngine.ThreadContext threadContext)
            {
                listCompleted++;
            });
            using (var timeEngine = new TimeEngine())
            {
                timeEngine.AddValue(linkItem.Link, 0);
                timeEngine.AddValue(listItem, 0);
                timeEngine.EvaluateFrame(100);
            }
            Assert.AreEqual(1, linkCompleted);
            Assert.AreEqual(1, listCompleted);
        }

        /// <summary>
        /// Adds the future item test execute.
        /// </summary>
        [Test]
        [Category("FrameTest")]
        public void AddFutureItemTestExecute()
        {
            int linkCompleted = 0;
            var linkItem = new LinkItem(delegate(TimeEngine.ThreadContext threadContext)
            {
                linkCompleted++;
            });
            int listCompleted = 0;
            var listItem = new ListItem(delegate(TimeEngine.ThreadContext threadContext)
            {
                listCompleted++;
            });
            using (var timeEngine = new TimeEngine())
            {
                timeEngine.AddValue(linkItem.Link, TimePrecision.ParseTime(1.0M));
                timeEngine.AddValue(listItem, TimePrecision.ParseTime(1.0M));
                timeEngine.EvaluateFrame(100);
            }
            Assert.AreEqual(1, linkCompleted);
            Assert.AreEqual(1, listCompleted);
        }

        /// <summary>
        /// Adds the item test multiple execute.
        /// </summary>
        [Test]
        [Category("FrameTest")]
        public void AddItemTestMultipleExecute()
        {
            int linkCompleted = 0;
            var linkItem = new LinkItem();
            linkItem.Action = delegate(TimeEngine.ThreadContext threadContext)
            {
                linkCompleted++;
                threadContext.AddValue(linkItem.Link, 0);
            };
            int listCompleted = 0;
            var listItem = new ListItem();
            listItem.Action = delegate(TimeEngine.ThreadContext threadContext)
            {
                listCompleted++;
                threadContext.AddValue(listItem, 0);
            };
            using (var timeEngine = new TimeEngine())
            {
                timeEngine.AddValue(linkItem.Link, 0);
                timeEngine.AddValue(listItem, 0);
                timeEngine.EvaluateFrame(100);
            }
            Assert.GreaterOrEqual(linkCompleted, 2);
            Assert.GreaterOrEqual(listCompleted, 2);
        }
    }
}
