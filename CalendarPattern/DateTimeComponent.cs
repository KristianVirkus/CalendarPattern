using System;

namespace CalendarPattern
{
    /// <summary>
    /// Enumeration of date time components ordered by descending rank.
    /// </summary>
    [Flags]
    public enum DateTimeComponent
    {
        None = 0,
        Year = 1 << 0,
        Month = 1 << 1,
        Day = 1 << 2,
        Hour = 1 << 3,
        Minute = 1 << 4,
        Second = 1 << 5,
        Millisecond = 1 << 6,
        Ticks = 1 << 7,
        Any = Year | Month | Day | Hour | Minute | Second | Millisecond | Ticks,
    }
}
