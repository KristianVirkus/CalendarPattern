using System;

namespace CalendarPattern
{
    /// <summary>
    /// Common interface of all date patterns.
    /// </summary>
    public interface IDateTimePattern : IPattern
    {
        /// <summary>
        /// Gets the date & time components affected by this pattern.
        /// </summary>
        DateTimeComponent AffectedDateTimeComponents { get; }

        /// <summary>
        /// Determines the previous point in time matching this pattern.
        /// </summary>
        /// <remarks>
        /// Generates the end of a range within lower ranked date & time components,
        /// e.g. for an hour pattern, it will generate 59 minutes, 59 seconds,
        /// 999 milliseconds, and 9999 more ticks.
        /// </remarks>
        /// <param name="now">The starting point in time.</param>
        /// <param name="tz">The time zone calculations take place in.</param>
        /// <returns>The calculated point in time or null if not possible.</returns>
        DateTime? Previous(DateTime now, TimeZoneInfo tz);

        /// <summary>
        /// Determines the next point in time matching this pattern.
        /// </summary>
        /// <remarks>
        /// Generates the beginning of a range within lower ranked date & time components,
        /// e.g. for an hour pattern, it will generate 00 minutes, 00 seconds,
        /// 999 milliseconds, and 9999 more ticks.
        /// </remarks>
        /// <param name="now">The starting point in time.</param>
        /// <param name="tz">The time zone calculations take place in.</param>
        /// <returns>The calculated point in time or null if not possible.</returns>
        DateTime? Next(DateTime now,  TimeZoneInfo tz);
    }
}
