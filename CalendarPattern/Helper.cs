using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CalendarPattern
{
    internal static class Helper
    {
        public static IReadOnlyCollection<DateTimeComponent> GetLowerRankedDateTimeComponents(IEnumerable<DateTimeComponent> usedComponents)
        {
            usedComponents = usedComponents.SelectMany(c => GetFlags(c)).Distinct().Where(c => c != DateTimeComponent.None && c != DateTimeComponent.Any).ToArray();
            var lowestRankUsed = usedComponents.OrderBy(r => r).LastOrDefault();
            return Enum.GetValues(typeof(DateTimeComponent)).Cast<DateTimeComponent>().Where(c => c != DateTimeComponent.None
                                                                                            && c != DateTimeComponent.Any 
                                                                                            && (usedComponents?.Any() != true || c > lowestRankUsed))
                                                                                    .ToArray();
        }

        public static IReadOnlyCollection<DateTimeComponent> GetHigherRankedDateTimeComponents(IEnumerable<DateTimeComponent> usedComponents)
        {
            usedComponents = usedComponents.SelectMany(c => GetFlags(c)).Distinct().Where(c => c != DateTimeComponent.None && c != DateTimeComponent.Any).ToArray();
            var highestRankUsed = usedComponents.OrderBy(r => r).FirstOrDefault();
            return Enum.GetValues(typeof(DateTimeComponent)).Cast<DateTimeComponent>().Where(c => c != DateTimeComponent.None
                                                                                            && c != DateTimeComponent.Any
                                                                                            && (usedComponents?.Any() != true || c < highestRankUsed))
                                                                                    .ToArray();
        }

        public static IEnumerable<TEnum> GetFlags<TEnum>(TEnum input) where TEnum : Enum
        {
            if ((int)(object)input == 0) return new[] { input };

            var results = new List<TEnum>();
            var inputBits = new BitArray(BitConverter.GetBytes((int)(object)input));

            for (int i = 0; i < inputBits.Length; i++)
            {
                if (inputBits[i])
                    results.Add((TEnum)(object)(1 << i));
            }

            return results;
        }

        public static ushort GetDateTimeComponent(DateTime dt, DateTimeComponent component)
            => component switch
            {
                DateTimeComponent.Year => (ushort)dt.Year,
                DateTimeComponent.Month => (ushort)dt.Month,
                DateTimeComponent.Day => (ushort)dt.Day,
                DateTimeComponent.Hour => (ushort)dt.Hour,
                DateTimeComponent.Minute => (ushort)dt.Minute,
                DateTimeComponent.Second => (ushort)dt.Second,
                DateTimeComponent.Millisecond => (ushort)dt.Millisecond,
                DateTimeComponent.Ticks => (ushort)(dt-new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, dt.Kind)).Ticks,
                _ => throw new NotSupportedException(nameof(component))
            };

        /// <summary>
        /// Aligns the <paramref name="dateTimeComponents"/> of a date & time <paramref name="dt"/>
        /// to an <paramref name="edge"/>.
        /// </summary>
        /// <param name="dt">The date & time to align.</param>
        /// <param name="edge">The edge.</param>
        /// <param name="dateTimeComponents">The date & time components to align.</param>
        /// <returns>The aligned date & time.</returns>
        /// <exception cref="ArgumentNullException">Thrown, if
        ///     <paramref name="dateTimeComponents"/> is null.</exception>
        /// <exception cref="NotSupportedException">Thrown, if
        ///     <paramref name="edge"/> is not supported.</exception>
        public static DateTime AlignDateTimeComponentsToEdge(DateTime dt, DateTimeRangeEdge edge, IEnumerable<DateTimeComponent> dateTimeComponents)
        {
            if (dateTimeComponents is null)
                throw new ArgumentNullException(nameof(dateTimeComponents));

            if (edge != DateTimeRangeEdge.Beginning && edge != DateTimeRangeEdge.End)
                throw new NotSupportedException(nameof(edge));

            foreach (var c in dateTimeComponents)
            {
                switch (c)
                {
                    case DateTimeComponent.Year:
                        if (edge == DateTimeRangeEdge.Beginning)
                            dt = dt.AddYears(DateTime.MinValue.Year - dt.Year);
                        else
                            dt = dt.AddYears(DateTime.MaxValue.Year - dt.Year);
                        break;
                    case DateTimeComponent.Month:
                        if (edge == DateTimeRangeEdge.Beginning)
                            dt = dt.AddMonths(-dt.Month + 1);
                        else
                            dt = dt.AddMonths(12 - dt.Month);
                        break;
                    case DateTimeComponent.Day:
                        if (edge == DateTimeRangeEdge.Beginning)
                            dt = dt.AddDays(-dt.Day + 1);
                        else
                            dt = dt.AddDays(DateTime.DaysInMonth(dt.Year, dt.Month) - dt.Day);
                        break;
                    case DateTimeComponent.Hour:
                        if (edge == DateTimeRangeEdge.Beginning)
                            dt = dt.AddHours(-dt.Hour);
                        else
                            dt = dt.AddHours(23 - dt.Hour);
                        break;
                    case DateTimeComponent.Minute:
                        if (edge == DateTimeRangeEdge.Beginning)
                            dt = dt.AddMinutes(-dt.Minute);
                        else
                            dt = dt.AddMinutes(59 - dt.Minute);
                        break;
                    case DateTimeComponent.Second:
                        if (edge == DateTimeRangeEdge.Beginning)
                            dt = dt.AddSeconds(-dt.Second);
                        else
                            dt = dt.AddSeconds(59 - dt.Second);
                        break;
                    case DateTimeComponent.Millisecond:
                        if (edge == DateTimeRangeEdge.Beginning)
                            dt = dt.AddMilliseconds(-dt.Millisecond);
                        else
                            dt = dt.AddMilliseconds(999 - dt.Millisecond);
                        break;
                    case DateTimeComponent.Ticks:
                        if (edge == DateTimeRangeEdge.Beginning)
                            dt -= GetAdditionalTicks(dt);
                        else
                            dt = dt - GetAdditionalTicks(dt) + Constants.MillisecondMaximum;
                        break;
                    default:
                        throw new NotSupportedException(nameof(dateTimeComponents));
                }
            }

            return dt;
        }

        /// <summary>
        /// Determines the number of ticks of a date & time below 999 milliseconds.
        /// </summary>
        /// <param name="dt">The date & time.</param>
        /// <returns>The number of ticks below 999 milliseconds.</returns>
        public static TimeSpan GetAdditionalTicks(DateTime dt)
            => TimeSpan.FromTicks(dt.Ticks % 10000);

        /// <summary>
        /// Checks whether the arguments comply with the bound.
        /// </summary>
        /// <remarks>
        /// Depending on the date & time patterns used, this method could deliver false
        /// positives for use cases with patterns which are too complex for a simple generic
        /// method to check the compliance with the bound.
        /// </remarks>
        /// <param name="dt">The date & time to check. Must be the same time zone as
        ///     <paramref name="bound"/>.</param>
        /// <param name="bound">The bound to comply with. Must be the same time zone as
        ///     <paramref name="dt"/>.</param>
        /// <param name="component">The date & time component aimed at.</param>
        /// <param name="componentValue">The date & time component's value aimed at.</param>
        /// <param name="direction">Whether the next or previous occurrence of the
        ///     date & time component's value is aimed at.</param>
        /// <returns>true if the arguments comply with the <paramref name="bound"/>,
        ///     false otherwise.</returns>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="component"/> or
        ///     <paramref name="direction"/> is not supported.</exception>
        public static bool ComplyWithBound(DateTime dt, DateTime bound, DateTimeComponent component,
            int componentValue, CalculationDirection direction)
        {
            // See if all higher ranked date & time components are already above the bound or below
            // the bound, respectively. If within the bound, there is a chance, the candidate date & time
            // could be with its new component value also within the bound.
            foreach (var c in Helper.GetHigherRankedDateTimeComponents(usedComponents: new[] { component }))
            {
                var dtComponent = Helper.GetDateTimeComponent(dt, c);
                var boundComponent = Helper.GetDateTimeComponent(bound, c);
                switch (direction)
                {
                    case CalculationDirection.Next:
                        if (dtComponent < boundComponent)
                            return true;
                        if (dtComponent > boundComponent)
                            return false;
                        break;
                    case CalculationDirection.Previous:
                        if (dtComponent > boundComponent)
                            return true;
                        if (dtComponent < boundComponent)
                            return false;
                        break;
                    default:
                        throw new NotSupportedException(nameof(direction));
                }
            }

            // All higher ranked date & time components are within the bound. Now test whether the new
            // component value is also within the bound.
            switch (direction)
            {
                case CalculationDirection.Next:
                    if (componentValue <= Helper.GetDateTimeComponent(bound, component))
                        return true;
                    break;
                case CalculationDirection.Previous:
                    if (componentValue >= Helper.GetDateTimeComponent(bound, component))
                        return true;
                    break;
                default:
                    throw new NotSupportedException(nameof(direction));
            }

            // Obviously the component value is not within the bound, thus report not within the bound.
            return false;
        }

        /// <summary>
        /// Enumeration of calculation directions.
        /// </summary>
        public enum CalculationDirection
        {
            /// <summary>
            /// Ahead to the future.
            /// </summary>
            Next,

            /// <summary>
            /// Back to the past.
            /// </summary>
            Previous,
        }
    }
}
