using System;

namespace CalendarPattern
{
    /// <summary>
    /// Implements a day of week pattern.
    /// </summary>
    public class DayOfWeekPattern : IDateTimePattern
    {
        /// <inheritdoc/>
        public DateTimeComponent AffectedDateTimeComponents => DateTimeComponent.Day;

        /// <summary>
        /// Initializes a new instance of the <see cref="DayOfPattern"/> class.
        /// </summary>
        /// <param name="dayOfWeek">The day of the week.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the day of week
        ///     is not a single element of <see cref="DayOfWeek"/>.</exception>
        public DayOfWeekPattern(DayOfWeek dayOfWeek)
        {
            if (!Enum.IsDefined(typeof(DayOfWeek), dayOfWeek))
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek));

            this.DayOfWeek = dayOfWeek;
        }

        /// <summary>
        /// Gets the day of the week.
        /// </summary>
        public DayOfWeek DayOfWeek { get; }

        /// <inheritdoc/>
        public bool Matches(DateTime dt)
            => dt.DayOfWeek == this.DayOfWeek;

        /// <inheritdoc/>
        public DateTime? Next(DateTime now, TimeZoneInfo tz)
        {
            try
            {
                var candidate = TimeZoneInfo.ConvertTime(now, tz);
                
                // Determine number of days to add until wanted day of week.
                // Make sure any days are added and the initial candidate is
                // not kept as we always want to advance to the next occurrence.
                var addDaysTemp = (int)this.DayOfWeek - (int)candidate.DayOfWeek;
                if (addDaysTemp <= 0) addDaysTemp += 7;
                var addDays = (byte)addDaysTemp;

                // If not complying with the bound, no valid next occurrence
                // can be found.
                if (!CompliesWithUpperBound(dt: candidate, daysToAdd: addDays, bound: DateTime.MaxValue))
                    return null;

                // Actually build the new candidate and check whether its
                // valid by means of the given time zone. If not valid
                // continue, otherwise return the candidate.
                candidate = candidate.AddDays(addDays);

                return new DateTime(candidate.Year, candidate.Month, candidate.Day, 00, 00, 00, candidate.Kind);
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public DateTime? Previous(DateTime now, TimeZoneInfo tz)
        {
            try
            {
                var candidate = TimeZoneInfo.ConvertTime(now, tz);

                // Determine number of days to subtract until wanted day of week.
                // Make sure any days are subtracted and the initial candidate is
                // not kept as we always want to advance to the previous occurrence.
                var subtractDaysTemp = (int)candidate.DayOfWeek - (int)this.DayOfWeek;
                if (subtractDaysTemp <= 0) subtractDaysTemp += 7;
                var subtractDays = (byte)subtractDaysTemp;

                // If not complying with the bound, no valid next occurrence
                // can be found.
                if (!CompliesWithLowerBound(dt: candidate, daysToSubtract: subtractDays, bound: DateTime.MinValue))
                    return null;

                // Actually build the new candidate and check whether its
                // valid by means of the given time zone. If not valid
                // continue, otherwise return the candidate.
                candidate = candidate.AddDays(-subtractDays);

                return new DateTime(candidate.Year, candidate.Month, candidate.Day, 23, 59, 59, candidate.Kind).Add(Constants.SecondMaximum);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether a date plus some additional days would still comply with an upper bound.
        /// </summary>
        /// <param name="dt">The date.</param>
        /// <param name="daysToAdd">The number of additional days, must not exceed one year by itself.</param>
        /// <param name="bound">The upper bound to check against.</param>
        /// <returns>true if <paramref name="dt"/> and <paramref name="daysToAdd"/> comply with the
        ///     <paramref name="bound"/> or false otherwise.</returns>
        internal static bool CompliesWithUpperBound(DateTime dt, byte daysToAdd, DateTime bound)
        {
            // A DateTime can be any day of a year including the last day of the year.
            // We can first check if the previousCandidate plus the daysToAdd remain within
            // the same year, then we can do a DateTime comparison. Also if the candidate
            // year is not the maximum year representable by DateTime then use regular
            // DateTime comparison.
            if (dt.DayOfYear + daysToAdd <= DaysInYear(dt.Year)
                || dt.Year < DateTime.MaxValue.Year)
            {
                return dt.AddDays(daysToAdd) <= bound;
            }

            // This point is only reached if the previous candidate year is already the maximum
            // year DateTime can represent. While a year overflow is required but not possible
            // this must be failure.
            return false;
        }

        /// <summary>
        /// Determines whether a date minus some additional days would still comply with a lower bound.
        /// </summary>
        /// <param name="dt">The date.</param>
        /// <param name="daysToSubtract">The number of additional days, must not exceed one year by itself.</param>
        /// <param name="bound">The lower bound to check against.</param>
        /// <returns>true if <paramref name="dt"/> and <paramref name="daysToSubtract"/> comply with the
        ///     <paramref name="bound"/> or false otherwise.</returns>
        internal static bool CompliesWithLowerBound(DateTime dt, byte daysToSubtract, DateTime bound)
        {
            // A DateTime can be any day of a year including the first day of the year.
            // We can first check if the previousCandidate minus the daysToSubtract remain within
            // the same year, then we can do a DateTime comparison. Also if the candidate
            // year is not the minimum year representable by DateTime then use regular
            // DateTime comparison.
            if (dt.DayOfYear - daysToSubtract >= 1
                || dt.Year > DateTime.MinValue.Year)
            {
                return dt.AddDays(-daysToSubtract) >= bound;
            }

            // This point is only reached if the previous candidate year is already the minimum
            // year DateTime can represent. While a year overflow is required but not possible
            // this must be failure.
            return false;
        }

        /// <summary>
        /// Determines the number of days in a year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>The number of days in the year.</returns>
        internal static ushort DaysInYear(int year)
            => (ushort)(DateTime.IsLeapYear(year) ? 366 : 365);
    }
}
