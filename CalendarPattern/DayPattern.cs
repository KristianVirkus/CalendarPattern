using System;

namespace CalendarPattern
{
    /// <summary>
    /// Implements a day pattern.
    /// </summary>
    public class DayPattern : IDateTimePattern
    {
        /// <inheritdoc/>
        public DateTimeComponent AffectedDateTimeComponents => DateTimeComponent.Day;

        /// <summary>
        /// Initializes a new instance of the <see cref="DayPattern"/> class.
        /// </summary>
        /// <param name="day">The day of the month.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the day is either
        ///     less than 1 or greater than 31.</exception>
        public DayPattern(byte day)
        {
            if (day < 1 || day > 31) throw new ArgumentOutOfRangeException(nameof(day));
            this.Day = day;
        }

        /// <summary>
        /// Gets the day of the month.
        /// </summary>
        public byte Day { get; }

        /// <inheritdoc/>
        public bool Matches(DateTime now)
            => now.Day == this.Day;

        /// <inheritdoc/>
        public DateTime? Next(DateTime now, TimeZoneInfo tz)
        {
            try
            {
                var candidate = TimeZoneInfo.ConvertTime(now, tz);
                var firstIteration = true;

                while (true)
                {
                    if (!Helper.ComplyWithBound(candidate, DateTime.MaxValue, DateTimeComponent.Day, this.Day, Helper.CalculationDirection.Next))
                        return null;

                    if ((candidate.Day >= this.Day && firstIteration)
                        || DateTime.DaysInMonth(candidate.Year, candidate.Month) < this.Day
                        || tz.IsInvalidTime(new DateTime(candidate.Year, candidate.Month, this.Day, 00, 00, 00, candidate.Kind)))
                    {
                        firstIteration = false;
                        candidate = candidate.AddMonths(1);
                    }
                    else
                    {
                        break;
                    }
                }

                return new DateTime(candidate.Year, candidate.Month, this.Day, 00, 00, 00, candidate.Kind);
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
                var firstIteration = true;

                while (true)
                {
                    if (!Helper.ComplyWithBound(candidate, DateTime.MinValue, DateTimeComponent.Day, this.Day, Helper.CalculationDirection.Previous))
                        return null;

                    if ((candidate.Day <= this.Day && firstIteration)
                        || DateTime.DaysInMonth(candidate.Year, candidate.Month) < this.Day
                        || tz.IsInvalidTime(new DateTime(candidate.Year, candidate.Month, this.Day, 00, 00, 00, candidate.Kind)))
                    {
                        firstIteration = false;
                        candidate = candidate.AddMonths(-1);
                    }
                    else
                    {
                        break;
                    }
                }

                return new DateTime(candidate.Year, candidate.Month, this.Day, 23, 59, 59, candidate.Kind).Add(Constants.SecondMaximum);
            }
            catch
            {
                return null;
            }
        }
    }
}
