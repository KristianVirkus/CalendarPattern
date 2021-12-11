using System;

namespace CalendarPattern
{
    /// <summary>
    /// Implements a month pattern.
    /// </summary>
    public class MonthPattern : IDateTimePattern
    {
        /// <inheritdoc/>
        public DateTimeComponent AffectedDateTimeComponents => DateTimeComponent.Month;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthPattern"/> class.
        /// </summary>
        /// <param name="month">The month of the year.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the month is either
        ///     less than 1 or greater than 12.</exception>
        public MonthPattern(byte month)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
            this.Month = month;
        }

        /// <summary>
        /// Gets the months of the year.
        /// </summary>
        public byte Month { get; }

        /// <inheritdoc/>
        public bool Matches(DateTime now)
            => now.Month == this.Month;

        /// <inheritdoc/>
        public DateTime? Next(DateTime now, TimeZoneInfo tz)
        {
            try
            {
                var candidate = TimeZoneInfo.ConvertTime(now, tz);
                var firstIteration = true;

                while (true)
                {
                    if (!Helper.ComplyWithBound(now, candidate, DateTime.MaxValue, DateTimeComponent.Month, this.Month, Helper.CalculationDirection.Next))
                        return null;

                    if ((candidate.Month >= this.Month && firstIteration)
                        || tz.IsInvalidTime(new DateTime(candidate.Year, this.Month, 01, 00, 00, 00, candidate.Kind)))
                    {
                        firstIteration = false;
                        candidate = candidate.AddYears(1);
                    }
                    else
                    {
                        break;
                    }
                }

                return new DateTime(candidate.Year, this.Month, 01, 00, 00, 00, candidate.Kind);
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
                    if (!Helper.ComplyWithBound(now, candidate, DateTime.MinValue, DateTimeComponent.Month, this.Month, Helper.CalculationDirection.Previous))
                        return null;

                    if ((candidate.Month <= this.Month && firstIteration)
                        || tz.IsInvalidTime(new DateTime(candidate.Year, this.Month, 01, 00, 00, 00, candidate.Kind)))
                    {
                        firstIteration = false;
                        candidate = candidate.AddYears(-1);
                    }
                    else
                    {
                        break;
                    }
                }

                return new DateTime(candidate.Year, this.Month, DateTime.DaysInMonth(candidate.Year, this.Month), 23, 59, 59, candidate.Kind).Add(Constants.SecondMaximum);
            }
            catch
            {
                return null;
            }
        }
    }
}
