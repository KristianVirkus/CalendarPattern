using System;

namespace CalendarPattern
{
    /// <summary>
    /// Common interface of all date or time patterns.
    /// </summary>
    public interface IPattern
    {
        /// <summary>
        /// Checks whether a given point in time <paramref name="now"/> matches the pattern.
        /// </summary>
        /// <param name="now">The point in time to check.</param>
        /// <returns>true if <paramref name="now"/> matches the pattern, false otherwise.</returns>
        bool Matches(DateTime now);
    }
}
