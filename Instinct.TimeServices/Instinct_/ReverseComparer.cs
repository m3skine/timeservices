using System.Collections.Generic;
namespace Instinct_
{
    /// <summary>
    /// ReverseComparer
    /// </summary>
    public class ReverseComparer : IComparer<ulong>
    {
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value
        /// Condition
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public int Compare(ulong x, ulong y)
        {
            if (x < y)
            {
                return 1;
            }
            if (x > y)
            {
                return -1;
            }
            return 0;
        }
    }
}