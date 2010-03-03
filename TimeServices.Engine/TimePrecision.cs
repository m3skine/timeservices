using System;
namespace TimeServices.Engine
{
    /// <summary>
    /// TimePrecision
    /// </summary>
    public static class TimePrecision
    {
        public const int TimePrecisionBits = 4;
        public const ulong TimePrecisionMask = (1 << TimePrecisionBits) - 1;
        public const ulong TimeScaler = (1 << TimePrecision.TimePrecisionBits);
        private const decimal TimeScaleUnit = (1M / TimeScaler);

        /// <summary>
        /// Parses the time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        public static ulong ParseTime(decimal time)
        {
            int integer = (int)Math.Truncate(time);
            decimal fraction = time - integer;
            return ((ulong)(integer << TimePrecisionBits) + (ulong)Math.Round(fraction / TimeScaleUnit));
        }

        /// <summary>
        /// Formats the time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        public static decimal FormatTime(ulong time)
        {
            ulong integer = (time >> TimePrecision.TimePrecisionBits);
            ulong fraction = (time & TimePrecision.TimePrecisionMask);
            return integer + (fraction * TimeScaleUnit);
        }
    }
}
