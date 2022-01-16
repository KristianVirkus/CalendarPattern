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
        public bool Matches(DateTime dt)
            => dt.Year == this.Year;

        /// <inheritdoc/>
        public DateTime? Next(DateTime after, TimeZoneInfo tz)
        {
            try
            {
                after = TimeZoneInfo.ConvertTime(after, tz);
                var candidate = new DateTime(this.Year, 01, 01, 00, 00, 00, after.Kind);

                if (!Helper.ComplyWithBound(candidate, DateTime.MaxValue, DateTimeComponent.Year, this.Year, Helper.CalculationDirection.Next)
                    || after.Year >= this.Year)
                {
                    return null;
                }

                return new DateTime(this.Year, 01, 01, 00, 00, 00, after.Kind);
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
                before = TimeZoneInfo.ConvertTime(before, tz);
                var candidate = new DateTime(this.Year, 01, 01, 00, 00, 00, before.Kind);

                if (!Helper.ComplyWithBound(candidate, DateTime.MinValue, DateTimeComponent.Year, this.Year, Helper.CalculationDirection.Previous)
                    || before.Year <= this.Year)
                {
                    return null;
                }

                return new DateTime(this.Year, 12, 31, 23, 59, 59, before.Kind).Add(Constants.SecondMaximum);
            }
            catch
            {
                return null;
            }
        }
    }
}
