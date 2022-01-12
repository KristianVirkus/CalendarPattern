using System;
using System.Collections.Generic;

namespace CalendarPattern
{
    /// <summary>
    /// Common interface of all calendar calculators.
    /// </summary>
    public interface ICalculator
    {
        /// <summary>
        /// Calculates the next point in time when all patterns match.
        /// </summary>
        /// <param name="patterns">The date & time patterns to consider.</param>
        /// <param name="startTime">The starting point in time.</param>
        /// <param name="tz">The time zone calculations take place in.</param>
        /// <returns>The previous date & time or null if there is no next date & time possible.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="patterns"/> or
        ///     <paramref name="tz"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="patterns"/> is empty.</exception>
        DateTime? Next(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz);

        /// <summary>
        /// Calculates the next point in time when all patterns match and performs edge alignment of all
        /// lower ranked date & time components as affected by the specified patterns.
        /// </summary>
        /// <param name="patterns">The date & time patterns to consider.</param>
        /// <param name="startTime">The starting point in time.</param>
        /// <param name="tz">The time zone calculations take place in.</param>
        /// <param name="edge">The requested date & time range edge.</param>
        /// <returns>The previous date & time or null if there is no next date & time possible.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="patterns"/> or
        ///     <paramref name="tz"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="patterns"/> is empty.</exception>
        DateTime? Next(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz, DateTimeRangeEdge edge);

        /// <summary>
        /// Calculates the previous point in time when all patterns match.
        /// </summary>
        /// <param name="patterns">The date & time patterns to consider.</param>
        /// <param name="startTime">The starting point in time.</param>
        /// <param name="tz">The time zone calculations take place in.</param>
        /// <returns>The previous date & time or null if there is no previous date & time possible.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="patterns"/> or
        ///     <paramref name="tz"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="patterns"/> is empty.</exception>
        DateTime? Previous(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz);

        /// <summary>
        /// Calculates the previous point in time when all patterns match and performs edge alignment of all
        /// lower ranked date & time components as affected by the specified patterns.
        /// </summary>
        /// <param name="patterns">The date & time patterns to consider.</param>
        /// <param name="startTime">The starting point in time.</param>
        /// <param name="tz">The time zone calculations take place in.</param>
        /// <param name="edge">The requested date & time range edge.</param>
        /// <returns>The previous date & time or null if there is no previous date & time possible.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="patterns"/> or
        ///     <paramref name="tz"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="patterns"/> is empty.</exception>
        DateTime? Previous(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz, DateTimeRangeEdge edge);
    }
}
