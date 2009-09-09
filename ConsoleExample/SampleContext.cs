﻿namespace ConsoleExample
{
    /// <summary>
    /// SampleContext
    /// </summary>
    public class SampleContext : System.ICloneable
    {
        #region Class Types
        /// <summary>
        /// UpdateVector
        /// </summary>
        [System.Flags]
        public enum UpdateVector
        {
            /// <summary>
            /// Value
            /// </summary>
            Value = 0x01,
            /// <summary>
            /// Tag
            /// </summary>
            Tag = 0x02,
        }
        #endregion Class Types

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleContext"/> class.
        /// </summary>
        public SampleContext()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public string Value;

        /// <summary>
        /// 
        /// </summary>
        public System.ICloneable Tag;

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return (SampleContext)MemberwiseClone();
        }
    }
}
