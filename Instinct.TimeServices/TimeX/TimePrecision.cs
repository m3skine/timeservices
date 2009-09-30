namespace Instinct.TimeX
{
    /// <summary>
    /// TimePrecision
    /// </summary>
    public static class TimePrecision
    {
        public const int TimePrecisionBits = 4;
        public const ulong TimePrecisionMask = (1 << TimePrecisionBits) - 1;
        public const ulong TimeScaler = (1 << TimePrecision.TimePrecisionBits);
        //+
        public const int ListWorkingFractionSize = 10;
        public const int LinkWorkingFractionSize = 10;
    }
}
