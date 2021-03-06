using System;
using System.Collections.Generic;
using System.Linq;

namespace CalendarPattern
{
    /// <summary>
    /// Implements a calculator for next and previous date & times matching some patterns.
    /// </summary>
    public class Calculator : ICalculator
    {
        /// <summary>
        /// Gets the calculator's default instance.
        /// </summary>
        public static readonly Calculator Default;

        /// <summary>
        /// Initializes the static instance of the class <see cref="Calculator"/>.
        /// </summary>
        static Calculator()
        {
            Default = new Calculator();
        }

        /// <inheritdoc/>
        public DateTime? Next(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz)
            => Next(patterns, startTime, tz, null, null);

        /// <inheritdoc/>
        public DateTime? Next(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz, DateTimeRangeEdge edge)
            => Next(patterns, startTime, tz, edge, null);

        /// <summary>
        /// Calculates the next point in time when all patterns match and performs edge alignment of all
        /// lower ranked date & time components as affected by the specified patterns if requested.
        /// </summary>
        /// <param name="patterns">The date & time patterns to consider.</param>
        /// <param name="startTime">The starting point in time.</param>
        /// <param name="tz">The time zone calculations take place in.</param>
        /// <param name="edge">The optional requested date & time range edge.</param>
        /// <param name="debugIterationCallback">The callback method to invoke for each iteration for debugging.</param>
        /// <returns>The previous date & time or null if there is no next date & time possible.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="patterns"/> or
        ///     <paramref name="tz"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="patterns"/> is empty.</exception>
        internal static DateTime? Next(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz, DateTimeRangeEdge? edge,
            Action<DebugIterationEventArgs> debugIterationCallback)
        {
            if (patterns is null) throw new ArgumentNullException(nameof(patterns));
            if (patterns.Any() != true)
                throw new ArgumentOutOfRangeException(nameof(patterns));

            // Get date & time to start with (always advance time & date.)
            var tempTimeResult = getTime(patterns, p => p.Next(startTime, tz), patternDt => patternDt - startTime, debugIterationCallback);
            if (tempTimeResult is null) return null;
            var timeResult = tempTimeResult.Value;

            while (true)
            {
                // If all patterns already match, then finish.
                if (patterns.All(p => p.Matches(timeResult)))
                {
                    if (edge is not null)
                    {
                        var lowerRankedDateTimeComponents = Helper.GetLowerRankedDateTimeComponents(patterns.Select(p => p.AffectedDateTimeComponents));
                        timeResult = Helper.AlignDateTimeComponentsToEdge(timeResult, edge.Value, lowerRankedDateTimeComponents);
                    }

                    return timeResult;
                }

                // Get alternatives (next times) for patterns currently not matching.
                tempTimeResult = getTime(patterns.Where(p => !p.Matches(timeResult)), p => p.Next(timeResult, tz), patternDt => patternDt - timeResult, debugIterationCallback);
                if (tempTimeResult is null)
                    return null;
                else
                    timeResult = tempTimeResult.Value;
            }
        }

        /// <inheritdoc/>
        public DateTime? Previous(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz)
            => Previous(patterns, startTime, tz, null, null);

        /// <inheritdoc/>
        public DateTime? Previous(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz, DateTimeRangeEdge edge)
            => Previous(patterns, startTime, tz, edge, null);

        /// <summary>
        /// Calculates the previous point in time when all patterns match and performs edge alignment of all
        /// lower ranked date & time components as affected by the specified patterns if requested.
        /// </summary>
        /// <param name="patterns">The date & time patterns to consider.</param>
        /// <param name="startTime">The starting point in time.</param>
        /// <param name="tz">The time zone calculations take place in.</param>
        /// <param name="edge">The optional requested date & time range edge.</param>
        /// <param name="debugIterationCallback">The callback method to invoke for each iteration for debugging.</param>
        /// <returns>The previous date & time or null if there is no previous date & time possible.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="patterns"/> or
        ///     <paramref name="tz"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="patterns"/> is empty.</exception>
        internal static DateTime? Previous(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz, DateTimeRangeEdge? edge,
            Action<DebugIterationEventArgs> debugIterationCallback)
        {
            if (patterns is null) throw new ArgumentNullException(nameof(patterns));
            if (patterns.Any() != true)
                throw new ArgumentOutOfRangeException(nameof(patterns));

            // Get date & time to start with (always advance time & date.)
            var tempTimeResult = getTime(patterns, p => p.Previous(startTime, tz), patternDt => startTime - patternDt, debugIterationCallback);
            if (tempTimeResult is null) return null;
            var timeResult = tempTimeResult.Value;

            while (true)
            {
                // If all patterns already match, then finish.
                if (patterns.All(p => p.Matches(timeResult)))
                {
                    if (edge is not null)
                    {
                        var lowerRankedDateTimeComponents = Helper.GetLowerRankedDateTimeComponents(patterns.Select(p => p.AffectedDateTimeComponents));
                        timeResult = Helper.AlignDateTimeComponentsToEdge(timeResult, edge.Value, lowerRankedDateTimeComponents);
                    }

                    return timeResult;
                }

                // Get alternatives (previous times) for patterns currently not matching.
                tempTimeResult = getTime(patterns.Where(p => !p.Matches(timeResult)), p => p.Previous(timeResult, tz), patternDt => timeResult - patternDt, debugIterationCallback);
                if (tempTimeResult is null)
                    return null;
                else
                    timeResult = tempTimeResult.Value;
            }
        }

        /// <summary>
        /// Gets the nearest next time from multiple alternatives (from patterns.)
        /// </summary>
        /// <param name="patterns">The date & time patterns.</param>
        /// <param name="getTimeCallback">The callback method to get a time for a pattern.</param>
        /// <param name="distanceCallback">The callback method to determine the distance between the
        ///     newly calculated pattern date & time and some reference date & time.</param>
        /// <param name="debugIterationCallback">The callback method to invoke for tracing
        ///     calculation steps.</param>
        /// <returns>The nearest next time from the patterns.</returns>
        private static DateTime? getTime(IEnumerable<IDateTimePattern> patterns,
            Func<IDateTimePattern, DateTime?> getTimeCallback, Func<DateTime, TimeSpan> distanceCallback,
            Action<DebugIterationEventArgs> debugIterationCallback)
        {
            // Get initial next date & time as we need to advance in any case.
            var initialAlternatives = patterns.Select(p =>
            {
                var time = getTimeCallback(p);
                var distance = time is null ? null : distanceCallback(time.Value) as TimeSpan?;
                return new PatternAlternative { Pattern = p, Dt = time, Distance = distance };
            }).ToArray();

            // Quit if all non-matching patterns cannot provide any alternative.
            if (initialAlternatives.Length > 0 && initialAlternatives.All(a => a.Dt is null))
                return null;

            // Filter alternatives without an actual alternative date & time.
            initialAlternatives = initialAlternatives.Where(a => a.Dt is not null).ToArray();

            // Proceed with nearest date & time.
            var nearestFirstAlternative = initialAlternatives.OrderBy(a => a.Distance).First();
            debugIterationCallback?.Invoke(new DebugIterationEventArgs { Alternatives = initialAlternatives, ChosenAlternative = Array.IndexOf(initialAlternatives, nearestFirstAlternative) });
            return nearestFirstAlternative.Dt.Value;
        }

        /// <summary>
        /// Internal class for debugging purposes.
        /// </summary>
        internal class PatternAlternative
        {
            public IPattern Pattern { get; set; }
            public DateTime? Dt { get; set; }
            public TimeSpan? Distance { get; set; }
        }
    }
}
