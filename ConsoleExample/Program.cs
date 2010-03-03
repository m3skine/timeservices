using System;
//using Instinct.Sample;
//using Instinct.Time;
using TimeServices.Engine;
namespace ConsoleExample
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Start.");
            using (var provider = EngineProvider.CreateProvider())
            {
                //provider.Types.Add(new SimpleType());
                //provider.Objects.Add(new EngineObject { Type = "A", DataMisalignedException  });
                provider.EvaluateFrame(100);
            }

            //using (var timeEngine = new TimeEngine())
            //{
            //    var linkItem = new LinkItem();
            //    linkItem.Action = delegate(TimeEngine.ThreadContext threadContext)
            //    {
            //        threadContext.AddValue(linkItem.Link, TimePrecision.ParseTime(1.0M));
            //        Console.WriteLine(".");
            //    };
            //    timeEngine.AddValue(linkItem.Link, TimePrecision.ParseTime(1.25M));
            //    timeEngine.EvaluateFrame(100);
            //}
            Console.WriteLine("Done.");
        }
    }
}
