using System;

namespace CalendarPattern
{
    /// <summary>
    /// Implements a year pattern.
    /// </summary>
    public class YearPattern : IDateTimePattern
    {
        /// <inheritdoc/>
        public DateTimeComponent AffectedDateTimeComponents => DateTimeComponent.Year;

        /// <summary>
        /// Initializes a new instance of the <see cref="YearPattern"/> class.
        /// </summary>
        /// <param name="year">The year A.C.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the year exceeds the
        ///     range of <see cref="DateTime"/>.</exception>
        public YearPattern(ushort year)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
                throw new ArgumentOutOfRangeException(nameof(year));
            this.Year = year;
        }

        /// <summary>
        /// Gets the year A.C.
        /// </summary>
        public ushort Year { get; }

        /// <inheritdoc/>
        public bool Matches(DateTime now)
            => now.Year == this.Year;

        /// <inheritdoc/>
        public DateTime? Next(DateTime now, TimeZoneInfo tz)
        {
            try
            {
                now = TimeZoneInfo.ConvertTime(now, tz);
                var candidate = new DateTime(this.Year, 01, 01, 00, 00, 00, now.Kind);

                if (!Helper.ComplyWithBound(now, candidate, DateTime.MaxValue, DateTimeComponent.Year, this.Year, Helper.CalculationDirection.Next)
                    || now.Year >= this.Year)
                {
                    return null;
                }

                return new DateTime(this.Year, 01, 01, 00, 00, 00, now.Kind);
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
                now = TimeZoneInfo.ConvertTime(now, tz);
                var candidate = new DateTime(this.Year, 01, 01, 00, 00, 00, now.Kind);

                if (!Helper.ComplyWithBound(now, candidate, DateTime.MinValue, DateTimeComponent.Year, this.Year, Helper.CalculationDirection.Previous)
                    || now.Year <= this.Year)
                {
                    return null;
                }

                return new DateTime(this.Year, 12, 31, 23, 59, 59, now.Kind).Add(Constants.SecondMaximum);
            }
            catch
            {
                return null;
            }
        }
    }
}
