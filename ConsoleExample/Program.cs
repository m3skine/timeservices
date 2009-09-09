using System;
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
            var timeEngine = new TimeEngine();
            timeEngine.EvaluateFrame(100);
        }
    }
}
