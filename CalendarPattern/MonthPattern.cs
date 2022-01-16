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
        public bool Matches(DateTime dt)
            => dt.Month == this.Month;

        /// <inheritdoc/>
        public DateTime? Next(DateTime after, TimeZoneInfo tz)
        {
            try
            {
                var candidate = TimeZoneInfo.ConvertTime(after, tz);
                var firstIteration = true;
                
                // Progress candidate date & time until the next candidate would be too late and as
                // long as the current candidate would be invalid but only until the bound would be
                // exceeded.
                while (true)
                {
                    // Tricky: Cannot create DateTime instance with wanted component because it might fail due to
                    // being an invalid date & time at all (e.g. Feb 31 or leap year.) Instead, check last known valid
                    // date & time candidate along with the wanted date & time component as isolated number. If the
                    // candidate exceeds the bound then abort with no result.
                    if (!Helper.ComplyWithBound(candidate, DateTime.MaxValue, DateTimeComponent.Month, this.Month, Helper.CalculationDirection.Next))
                        return null;

                    // Move to next candidate if this one is both the first attempt and has the requested component's value already
                    // or move to next candidate if the candidate is invalid for the given time zone, i.e. regarding daylight saving.
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
        public DateTime? Previous(DateTime before, TimeZoneInfo tz)
        {
            try
            {
                var candidate = TimeZoneInfo.ConvertTime(before, tz);
                var firstIteration = true;

                while (true)
                {
                    if (!Helper.ComplyWithBound(candidate, DateTime.MinValue, DateTimeComponent.Month, this.Month, Helper.CalculationDirection.Previous))
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
