using System;

namespace CalendarPattern
{
    /// <summary>
    /// Implements an hour pattern.
    /// </summary>
    public class HourPattern : IDateTimePattern
    {
        /// <inheritdoc/>
        public DateTimeComponent AffectedDateTimeComponents => DateTimeComponent.Hour;

        /// <summary>
        /// Initializes a new instance of the <see cref="HourPattern"/> class.
        /// </summary>
        /// <param name="hour">The hour of the day.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the hour is either
        ///     less than 0 or greater than 23.</exception>
        public HourPattern(byte hour)
        {
            if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException(nameof(hour));
            this.Hour = hour;
        }

        /// <summary>
        /// Gets the hour of the day.
        /// </summary>
        public byte Hour { get; }

        /// <inheritdoc/>
        public bool Matches(DateTime now)
            => now.Hour == this.Hour;

        /// <inheritdoc/>
        public DateTime? Next(DateTime now, TimeZoneInfo tz)
        {
            try
            {
                var candidate = TimeZoneInfo.ConvertTime(now, tz);
                var firstIteration = true;

                while (true)
                {
                    if (!Helper.ComplyWithBound(candidate, DateTime.MaxValue, DateTimeComponent.Hour, this.Hour, Helper.CalculationDirection.Next))
                        return null;

                    if ((candidate.Hour >= this.Hour && firstIteration)
                        || tz.IsInvalidTime(new DateTime(candidate.Year, candidate.Month, candidate.Day, this.Hour, 00, 00, candidate.Kind)))
                    {
                        firstIteration = false;
                        candidate = candidate.AddDays(1);
                    }
                    else
                    {
                        break;
                    }
                }

                return new DateTime(candidate.Year, candidate.Month, candidate.Day, this.Hour, 00, 00, candidate.Kind);
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
                    if (!Helper.ComplyWithBound(candidate, DateTime.MinValue, DateTimeComponent.Hour, this.Hour, Helper.CalculationDirection.Previous))
                        return null;

                    if ((candidate.Hour <= this.Hour && firstIteration)
                        || tz.IsInvalidTime(new DateTime(candidate.Year, candidate.Month, candidate.Day, this.Hour, 00, 00, candidate.Kind)))
                    {
                        firstIteration = false;
                        candidate = candidate.AddDays(-1);
                    }
                    else
                    {
                        break;
                    }
                }

                return new DateTime(candidate.Year, candidate.Month, candidate.Day, this.Hour, 00, 00, now.Kind).Add(Constants.HourMaximum);
            }
            catch
            {
                return null;
            }
        }
    }
}
