using System;

namespace CalendarPattern
{
    /// <summary>
    /// Contains constants for time handling.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Gets a single tick which is the minimum time resolution.
        /// </summary>
        public static readonly TimeSpan Tick;

        /// <summary>
        /// Gets the maximum time of a day.
        /// </summary>
        public static readonly TimeSpan DayMaximum;

        /// <summary>
        /// Gets the maximum time of an hour.
        /// </summary>
        public static readonly TimeSpan HourMaximum;

        /// <summary>
        /// Gets the maximum time of a minute.
        /// </summary>
        public static readonly TimeSpan MinuteMaximum;

        /// <summary>
        /// Gets the maximum time of a second.
        /// </summary>
        public static readonly TimeSpan SecondMaximum;

        /// <summary>
        /// Gets the maximum time of a millisecond.
        /// </summary>
        public static readonly TimeSpan MillisecondMaximum;

        /// <summary>
        /// Initializes the static instance of the <see cref="Constants"/> class.
        /// </summary>
        static Constants()
        {
            Tick = TimeSpan.FromTicks(1);
            DayMaximum = TimeSpan.FromDays(1) - Tick;
            HourMaximum = TimeSpan.FromHours(1) - Tick;
            MinuteMaximum = TimeSpan.FromMinutes(1) - Tick;
            SecondMaximum = TimeSpan.FromSeconds(1) - Tick;
            MillisecondMaximum = TimeSpan.FromMilliseconds(1) - Tick;
        }
    }
}
