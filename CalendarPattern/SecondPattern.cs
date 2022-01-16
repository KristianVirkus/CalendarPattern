using System;

namespace CalendarPattern
{
    /// <summary>
    /// Implements a second pattern.
    /// </summary>
    public class SecondPattern : IDateTimePattern
    {
        /// <inheritdoc/>
        public DateTimeComponent AffectedDateTimeComponents => DateTimeComponent.Second;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecondPattern"/> class.
        /// </summary>
        /// <param name="second">The second of the minute.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the second is either
        ///     less than 0 or greater than 59.</exception>
        public SecondPattern(byte second)
        {
            if (second < 0 || second > 59) throw new ArgumentOutOfRangeException(nameof(second));
            this.Second = second;
        }

        /// <summary>
        /// Gets the second of the minute.
        /// </summary>
        public byte Second { get; }

        /// <inheritdoc/>
        public bool Matches(DateTime dt)
            => dt.Second == this.Second;

        /// <inheritdoc/>
        public DateTime? Next(DateTime after, TimeZoneInfo tz)
        {
            try
            {
                var candidate = TimeZoneInfo.ConvertTime(after, tz);
                var firstIteration = true;

                while (true)
                {
                    if (!Helper.ComplyWithBound(candidate, DateTime.MaxValue, DateTimeComponent.Second, this.Second, Helper.CalculationDirection.Next))
                        return null;

                    if ((candidate.Second >= this.Second && firstIteration)
                        || tz.IsInvalidTime(new DateTime(candidate.Year, candidate.Month, candidate.Day, candidate.Hour, candidate.Minute, this.Second, candidate.Kind)))
                    {
                        firstIteration = false;
                        candidate = candidate.AddMinutes(1);
                    }
                    else
                    {
                        break;
                    }
                }

                return new DateTime(candidate.Year, candidate.Month, candidate.Day, candidate.Hour, candidate.Minute, this.Second, candidate.Kind);
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
                    if (!Helper.ComplyWithBound(candidate, DateTime.MinValue, DateTimeComponent.Second, this.Second, Helper.CalculationDirection.Previous))
                        return null;

                    if ((candidate.Second <= this.Second && firstIteration)
                        || tz.IsInvalidTime(new DateTime(candidate.Year, candidate.Month, candidate.Day, candidate.Hour, candidate.Minute, this.Second, candidate.Kind)))
                    {
                        firstIteration = false;
                        candidate = candidate.AddMinutes(-1);
                    }
                    else
                    {
                        break;
                    }
                }

                return new DateTime(candidate.Year, candidate.Month, candidate.Day, candidate.Hour, candidate.Minute, this.Second, candidate.Kind).Add(Constants.SecondMaximum);
            }
            catch
            {
                return null;
            }
        }
    }
}
