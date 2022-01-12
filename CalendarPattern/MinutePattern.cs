using System;

namespace CalendarPattern
{
    /// <summary>
    /// Implements a minute pattern.
    /// </summary>
    public class MinutePattern : IDateTimePattern
    {
        /// <inheritdoc/>
        public DateTimeComponent AffectedDateTimeComponents => DateTimeComponent.Minute;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinutePattern"/> class.
        /// </summary>
        /// <param name="minute">The minute of the hour.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the minute is either
        ///     less than 0 or greater than 59.</exception>
        public MinutePattern(byte minute)
        {
            if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException(nameof(minute));
            this.Minute = minute;
        }

        /// <summary>
        /// Gets the minute of the hour.
        /// </summary>
        public byte Minute { get; }

        /// <inheritdoc/>
        public bool Matches(DateTime now)
            => now.Minute == this.Minute;

        /// <inheritdoc/>
        public DateTime? Next(DateTime now, TimeZoneInfo tz)
        {
            try
            {
                var candidate = TimeZoneInfo.ConvertTime(now, tz);
                var firstIteration = true;

                while (true)
                {
                    if (!Helper.ComplyWithBound(candidate, DateTime.MaxValue, DateTimeComponent.Minute, this.Minute, Helper.CalculationDirection.Next))
                        return null;

                    if ((candidate.Minute >= this.Minute && firstIteration)
                        || tz.IsInvalidTime(new DateTime(candidate.Year, candidate.Month, candidate.Day, candidate.Hour, this.Minute, 00, candidate.Kind)))
                    {
                        firstIteration = false;
                        candidate = candidate.AddHours(1);
                    }
                    else
                    {
                        break;
                    }
                }

                return new DateTime(candidate.Year, candidate.Month, candidate.Day, candidate.Hour, this.Minute, 00, candidate.Kind);
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
                    if (!Helper.ComplyWithBound(candidate, DateTime.MinValue, DateTimeComponent.Minute, this.Minute, Helper.CalculationDirection.Previous))
                        return null;

                    if ((candidate.Minute <= this.Minute && firstIteration)
                        || tz.IsInvalidTime(new DateTime(candidate.Year, candidate.Month, candidate.Day, candidate.Hour, this.Minute, 00, candidate.Kind)))
                    {
                        firstIteration = false;
                        candidate = candidate.AddHours(-1);
                    }
                    else
                    {
                        break;
                    }
                }

                return new DateTime(candidate.Year, candidate.Month, candidate.Day, candidate.Hour, this.Minute, 00, candidate.Kind).Add(Constants.MinuteMaximum);
            }
            catch
            {
                return null;
            }
        }
    }
}
