#define DEBUG_ITERATIONS

using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CalendarPattern.UnitTests
{
    [TestFixture]
    public class CalculatorTest
    {
        static readonly TimeZoneInfo Cet;
        static readonly Action<DebugIterationEventArgs> DebugIterationCallback;

        static CalculatorTest()
        {
            Cet = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
#if DEBUG_ITERATIONS
            DebugIterationCallback = DebugPrint2;
#endif
        }

        public class Next
        {
            public class WithEdgeAlignment
            {
                public class EdgeBeginning
                {
                    public class Misc
                    {
                        [Test]
                        public void AtMiddleOfTheYearWhileExactDateInNextYearWanted_Should_AdvanceToWantedDate()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: (ushort)2000);
                            var monthPattern = new MonthPattern(month: (byte)5);
                            var dayPattern = new DayPattern(day: (byte)12);
                            var now = new DateTime(1999, 07, 01, 00, 00, 00, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var result = CallNext(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            result.Should().Be(new DateTime(2000, 05, 12, 00, 00, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtMinimumWhileMinimumWanted_ShouldReturn_Null()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: (ushort)DateTime.MinValue.Year);
                            var monthPattern = new MonthPattern(month: (byte)DateTime.MinValue.Month);
                            var dayPattern = new DayPattern(day: (byte)DateTime.MinValue.Day);
                            var hourPattern = new HourPattern(hour: (byte)DateTime.MinValue.Hour);
                            var minutePattern = new MinutePattern(minute: (byte)DateTime.MinValue.Minute);
                            var secondPattern = new SecondPattern(second: (byte)DateTime.MinValue.Second);
                            var now = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().BeNull();
                        }

                        [Test]
                        public void AtMinimumWhileMaximumWanted_Should_AdvanceToMaximumInNoTime()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: (ushort)DateTime.MaxValue.Year);
                            var monthPattern = new MonthPattern(month: (byte)DateTime.MaxValue.Month);
                            var dayPattern = new DayPattern(day: (byte)DateTime.MaxValue.Day);
                            var hourPattern = new HourPattern(hour: (byte)DateTime.MaxValue.Hour);
                            var minutePattern = new MinutePattern(minute: (byte)DateTime.MaxValue.Minute);
                            var secondPattern = new SecondPattern(second: (byte)DateTime.MaxValue.Second);
                            var now = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(DateTime.SpecifyKind(DateTime.MaxValue - Constants.SecondMaximum, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtMaximumSecondWhileMaximumWanted_ShouldReturn_Null()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: (ushort)DateTime.MaxValue.Year);
                            var monthPattern = new MonthPattern(month: (byte)DateTime.MaxValue.Month);
                            var dayPattern = new DayPattern(day: (byte)DateTime.MaxValue.Day);
                            var hourPattern = new HourPattern(hour: (byte)DateTime.MaxValue.Hour);
                            var minutePattern = new MinutePattern(minute: (byte)DateTime.MaxValue.Minute);
                            var secondPattern = new SecondPattern(second: (byte)DateTime.MaxValue.Second);
                            var now = DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().BeNull();
                        }
                    }

                    public class EdgeApplication
                    {
                        [Test]
                        public void AtAnyTimeWithYearPattern_Should_ApplyBeginningEdgeToMonthsAndLowerRanked()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: 2001);
                            var now = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { yearPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2001, 01, 01, 00, 00, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtAnyTimeWithMonthPattern_Should_ApplyBeginningEdgeToDaysAndLowerRanked()
                        {
                            // Arrange
                            var monthPattern = new MonthPattern(month: 2);
                            var now = new DateTime(2000, 01, 14, 12, 30, 30, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { monthPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 02, 01, 00, 00, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtAnyTimeWithDayPattern_Should_ApplyBeginningEdgeToHoursAndLowerRanked()
                        {
                            // Arrange
                            var dayPattern = new DayPattern(day: 2);
                            var now = new DateTime(2000, 06, 01, 12, 30, 30, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { dayPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 02, 00, 00, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtAnyTimeWithHourPattern_Should_ApplyBeginningEdgeToMinutesAndLowerRanked()
                        {
                            // Arrange
                            var hourPattern = new HourPattern(hour: 2);
                            var now = new DateTime(2000, 06, 14, 01, 30, 30, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { hourPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 02, 00, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtAnyTimeWithMinutePattern_Should_ApplyBeginningEdgeToSecondsAndLowerRanked()
                        {
                            // Arrange
                            var minutePattern = new MinutePattern(minute: 2);
                            var now = new DateTime(2000, 06, 14, 12, 01, 30, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { minutePattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 12, 02, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtAnyTimeWithSecondPattern_Should_ApplyBeginningEdgeToMillisecondsAndLowerRanked()
                        {
                            // Arrange
                            var secondPattern = new SecondPattern(second: 2);
                            var now = new DateTime(2000, 06, 14, 12, 30, 01, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { secondPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 12, 30, 02, DateTimeKind.Utc));
                        }
                    }

                    public class Day
                    {
                        [Test]
                        public void At29FebWith29DaysInMonthWhileAny29thFebWanted_Should_Skip4Years()
                        {
                            // Arrange
                            var monthPattern = new MonthPattern(month: (byte)2);
                            var dayPattern = new DayPattern(day: (byte)29);
                            var now = new DateTime(2020, 02, 29, 00, 00, 00, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var result = CallNext(patterns: new IDateTimePattern[] { monthPattern, dayPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            result.Should().Be(new DateTime(2024, 02, 29, 00, 00, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void At31FebWanted_ShouldReturn_Null()
                        {
                            // Arrange
                            var monthPattern = new MonthPattern(month: (byte)2);
                            var dayPattern = new DayPattern(day: (byte)31);
                            var now = new DateTime(2020, 01, 01, 00, 00, 00, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var result = CallNext(patterns: new IDateTimePattern[] { monthPattern, dayPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            result.Should().BeNull();
                        }
                    }
                }

                public class EdgeEnd
                {
                    public class Misc
                    {
                        [Test]
                        public void AtMinimumWhileMaximumWanted_Should_AdvanceToMaximumInNoTime()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: (ushort)DateTime.MaxValue.Year);
                            var monthPattern = new MonthPattern(month: (byte)DateTime.MaxValue.Month);
                            var dayPattern = new DayPattern(day: (byte)DateTime.MaxValue.Day);
                            var hourPattern = new HourPattern(hour: (byte)DateTime.MaxValue.Hour);
                            var minutePattern = new MinutePattern(minute: (byte)DateTime.MaxValue.Minute);
                            var secondPattern = new SecondPattern(second: (byte)DateTime.MaxValue.Second);
                            var now = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc));
                        }
                    }

                    public class EdgeApplication
                    {
                        [Test]
                        public void AtAnyTimeWithYearPattern_Should_ApplyEndEdgeToMonthsAndLowerRanked()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: 2001);
                            var now = new DateTime(2000, 06, 14, 12, 30, 30, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { yearPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2001, 12, 31, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }

                        [Test]
                        public void AtAnyTimeWithMonthPattern_Should_ApplyEndEdgeToDaysAndLowerRanked()
                        {
                            // Arrange
                            var monthPattern = new MonthPattern(month: 2);
                            var now = new DateTime(2000, 01, 14, 12, 30, 30, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { monthPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 02, 29, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }

                        [Test]
                        public void AtAnyTimeWithDayPattern_Should_ApplyEndEdgeToHoursAndLowerRanked()
                        {
                            // Arrange
                            var dayPattern = new DayPattern(day: 2);
                            var now = new DateTime(2000, 06, 01, 12, 30, 30, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { dayPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 02, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }

                        [Test]
                        public void AtAnyTimeWithHourPattern_Should_ApplyEndEdgeToMinutesAndLowerRanked()
                        {
                            // Arrange
                            var hourPattern = new HourPattern(hour: 2);
                            var now = new DateTime(2000, 06, 14, 01, 30, 30, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { hourPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 02, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }

                        [Test]
                        public void AtAnyTimeWithMinutePattern_Should_ApplyEndEdgeToSecondsAndLowerRanked()
                        {
                            // Arrange
                            var minutePattern = new MinutePattern(minute: 2);
                            var now = new DateTime(2000, 06, 14, 12, 01, 30, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { minutePattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 12, 02, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }

                        [Test]
                        public void AtAnyTimeWithSecondPattern_Should_ApplyEndEdgeToMillisecondsAndLowerRanked()
                        {
                            // Arrange
                            var secondPattern = new SecondPattern(second: 2);
                            var now = new DateTime(2000, 06, 14, 12, 30, 01, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { secondPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 12, 30, 02, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }
                    }
                }
            }

            public class WithoutEdgeAlignment
            {
                [Test]
                public void Next_Should_AdvanceToWantedTime()
                {
                    // Arrange
                    var yearPattern = new YearPattern(year: 2001);
                    var now = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc);
                    var tz = TimeZoneInfo.Utc;

                    // Act
                    var sw = new Stopwatch();
                    sw.Start();
                    var result = CallNext(patterns: new IDateTimePattern[] { yearPattern }, now, tz, null);

                    // Assert
                    sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                    result.Should().Be(new DateTime(2001, 01, 01, 00, 00, 00, DateTimeKind.Utc));
                }
            }

            public class Misc
            {
                [Test]
                public void NextOverloads_ShouldHave_DifferentResults()
                {
                    // Arrange
                    var monthPattern = new MonthPattern(month: 2);
                    var now = new DateTime(2000, 01, 14, 12, 30, 30, DateTimeKind.Utc);
                    var tz = TimeZoneInfo.Utc;

                    // Act
                    var sw = new Stopwatch();
                    sw.Start();
                    var resultPublicWithoutEdge = Calculator.Default.Next(patterns: new IDateTimePattern[] { monthPattern }, now, tz);
                    var resultPublicWithEdgeEnd = Calculator.Default.Next(patterns: new IDateTimePattern[] { monthPattern }, now, tz, DateTimeRangeEdge.End);
                    var resultInternalWithEdgeEnd = CallNext(patterns: new IDateTimePattern[] { monthPattern }, now, tz, DateTimeRangeEdge.End);

                    // Assert
                    sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                    resultPublicWithoutEdge.Should().NotBe(resultPublicWithEdgeEnd);
                    resultPublicWithEdgeEnd.Should().Be(resultInternalWithEdgeEnd);
                }

                [Test]
                public void ImpossibleToAchievePattern_ShouldReturn_NullInNoTime()
                {
                    // Arrange
                    var yearPattern = new YearPattern(year: 4099);
                    var monthPattern = new MonthPattern(month: 12);
                    var dayPattern = new DayPattern(day: 31);
                    var dayOfWeekPattern = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Friday);
                    var now = DateTime.MinValue;
                    var tz = TimeZoneInfo.Utc;

                    // Act
                    var sw = new Stopwatch();
                    sw.Start();
                    var result = Calculator.Default.Next(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, dayOfWeekPattern }, now, tz);

                    // Assert
                    sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                    result.Should().BeNull();
                }
            }
        }

        public class Previous
        {
            public class WithEdgeAlignment
            {
                public class EdgeBeginning
                {
                    public class Misc
                    {
                        [Test]
                        public void AtMaximumWhileMinimumWanted_Should_AdvanceToMinimumInNoTime()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: (ushort)DateTime.MinValue.Year);
                            var monthPattern = new MonthPattern(month: (byte)DateTime.MinValue.Month);
                            var dayPattern = new DayPattern(day: (byte)DateTime.MinValue.Day);
                            var hourPattern = new HourPattern(hour: (byte)DateTime.MinValue.Hour);
                            var minutePattern = new MinutePattern(minute: (byte)DateTime.MinValue.Minute);
                            var secondPattern = new SecondPattern(second: (byte)DateTime.MinValue.Second);
                            var now = DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc));
                        }
                    }

                    public class EdgeApplication
                    {
                        [Test]
                        public void AtAnyTimeWithYearPattern_Should_ApplyBeginningEdgeToMonthsAndLowerRanked()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: 1999);
                            var now = new DateTime(2000, 06, 14, 12, 30, 30, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { yearPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(1999, 01, 01, 00, 00, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtAnyTimeWithMonthPattern_Should_ApplyBeginningEdgeToDaysAndLowerRanked()
                        {
                            // Arrange
                            var monthPattern = new MonthPattern(month: 2);
                            var now = new DateTime(2000, 03, 14, 12, 30, 30, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { monthPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 02, 01, 00, 00, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtAnyTimeWithDayPattern_Should_ApplyBeginningEdgeToHoursAndLowerRanked()
                        {
                            // Arrange
                            var dayPattern = new DayPattern(day: 2);
                            var now = new DateTime(2000, 06, 03, 12, 30, 30, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { dayPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 02, 00, 00, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtAnyTimeWithHourPattern_Should_ApplyBeginningEdgeToMinutesAndLowerRanked()
                        {
                            // Arrange
                            var hourPattern = new HourPattern(hour: 2);
                            var now = new DateTime(2000, 06, 14, 03, 30, 30, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { hourPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 02, 00, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtAnyTimeWithMinutePattern_Should_ApplyBeginningEdgeToSecondsAndLowerRanked()
                        {
                            // Arrange
                            var minutePattern = new MinutePattern(minute: 2);
                            var now = new DateTime(2000, 06, 14, 12, 03, 30, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { minutePattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 12, 02, 00, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtAnyTimeWithSecondPattern_Should_ApplyBeginningEdgeToMillisecondsAndLowerRanked()
                        {
                            // Arrange
                            var secondPattern = new SecondPattern(second: 2);
                            var now = new DateTime(2000, 06, 14, 12, 30, 03, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { secondPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 12, 30, 02, DateTimeKind.Utc));
                        }
                    }

                    public class Day
                    {
                        [Test]
                        public void At31FebWanted_ShouldReturn_Null()
                        {
                            // Arrange
                            var monthPattern = new MonthPattern(month: (byte)2);
                            var dayPattern = new DayPattern(day: (byte)31);
                            var now = new DateTime(2020, 05, 01, 00, 00, 00, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var result = CallPrevious(patterns: new IDateTimePattern[] { monthPattern, dayPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            result.Should().BeNull();
                        }
                    }
                }

                public class EdgeEnd
                {
                    public class Misc
                    {
                        [Test]
                        public void MiddleOfTheYearWhileExactDateInPreviousYearWanted_Should_AdvanceToWantedDate()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: (ushort)2000);
                            var monthPattern = new MonthPattern(month: (byte)5);
                            var dayPattern = new DayPattern(day: (byte)12);
                            var now = new DateTime(2001, 07, 01, 00, 00, 00, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var result = CallPrevious(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            result.Should().Be(new DateTime(2000, 05, 12, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }

                        [Test]
                        public void AtMinimumWhileMinimumWanted_ShouldReturn_Null()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: (ushort)DateTime.MinValue.Year);
                            var monthPattern = new MonthPattern(month: (byte)DateTime.MinValue.Month);
                            var dayPattern = new DayPattern(day: (byte)DateTime.MinValue.Day);
                            var hourPattern = new HourPattern(hour: (byte)DateTime.MinValue.Hour);
                            var minutePattern = new MinutePattern(minute: (byte)DateTime.MinValue.Minute);
                            var secondPattern = new SecondPattern(second: (byte)DateTime.MinValue.Second);
                            var now = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().BeNull();
                        }

                        [Test]
                        public void AtMinimumWhileMaximumWanted_Should_AdvanceToMaximumInNoTime()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: (ushort)DateTime.MaxValue.Year);
                            var monthPattern = new MonthPattern(month: (byte)DateTime.MaxValue.Month);
                            var dayPattern = new DayPattern(day: (byte)DateTime.MaxValue.Day);
                            var hourPattern = new HourPattern(hour: (byte)DateTime.MaxValue.Hour);
                            var minutePattern = new MinutePattern(minute: (byte)DateTime.MaxValue.Minute);
                            var secondPattern = new SecondPattern(second: (byte)DateTime.MaxValue.Second);
                            var now = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(DateTime.SpecifyKind(DateTime.MaxValue - Constants.SecondMaximum, DateTimeKind.Utc));
                        }

                        [Test]
                        public void AtMaximumSecondWhileMaximumWanted_ShouldReturn_Null()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: (ushort)DateTime.MaxValue.Year);
                            var monthPattern = new MonthPattern(month: (byte)DateTime.MaxValue.Month);
                            var dayPattern = new DayPattern(day: (byte)DateTime.MaxValue.Day);
                            var hourPattern = new HourPattern(hour: (byte)DateTime.MaxValue.Hour);
                            var minutePattern = new MinutePattern(minute: (byte)DateTime.MaxValue.Minute);
                            var secondPattern = new SecondPattern(second: (byte)DateTime.MaxValue.Second);
                            var now = DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallNext(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.Beginning);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().BeNull();
                        }
                    }

                    public class EdgeApplication
                    {
                        [Test]
                        public void AtAnyTimeWithYearPattern_Should_ApplyEndEdgeToMonthsAndLowerRanked()
                        {
                            // Arrange
                            var yearPattern = new YearPattern(year: 1999);
                            var now = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { yearPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(1999, 12, 31, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }

                        [Test]
                        public void AtAnyTimeWithMonthPattern_Should_ApplyEndEdgeToDaysAndLowerRanked()
                        {
                            // Arrange
                            var monthPattern = new MonthPattern(month: 2);
                            var now = new DateTime(2000, 03, 14, 12, 30, 30, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { monthPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 02, 29, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }

                        [Test]
                        public void AtAnyTimeWithDayPattern_Should_ApplyEndEdgeToHoursAndLowerRanked()
                        {
                            // Arrange
                            var dayPattern = new DayPattern(day: 2);
                            var now = new DateTime(2000, 06, 03, 12, 30, 30, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { dayPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 02, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }

                        [Test]
                        public void AtAnyTimeWithHourPattern_Should_ApplyEndEdgeToMinutesAndLowerRanked()
                        {
                            // Arrange
                            var hourPattern = new HourPattern(hour: 2);
                            var now = new DateTime(2000, 06, 14, 03, 30, 30, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { hourPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 02, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }

                        [Test]
                        public void AtAnyTimeWithMinutePattern_Should_ApplyEndEdgeToSecondsAndLowerRanked()
                        {
                            // Arrange
                            var minutePattern = new MinutePattern(minute: 2);
                            var now = new DateTime(2000, 06, 14, 12, 03, 30, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { minutePattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 12, 02, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }

                        [Test]
                        public void AtAnyTimeWithSecondPattern_Should_ApplyEndEdgeToMillisecondsAndLowerRanked()
                        {
                            // Arrange
                            var secondPattern = new SecondPattern(second: 2);
                            var now = new DateTime(2000, 06, 14, 12, 30, 03, 500, DateTimeKind.Utc);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var sw = new Stopwatch();
                            sw.Start();
                            var result = CallPrevious(patterns: new IDateTimePattern[] { secondPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                            result.Should().Be(new DateTime(2000, 06, 14, 12, 30, 02, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }
                    }

                    public class Day
                    {
                        [Test]
                        public void AtEndOf29FebWith29DaysInMonthWhileAny29thFebWanted_Should_Skip4Years()
                        {
                            // Arrange
                            var monthPattern = new MonthPattern(month: (byte)2);
                            var dayPattern = new DayPattern(day: (byte)29);
                            var now = new DateTime(2024, 02, 29, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum);
                            var tz = TimeZoneInfo.Utc;

                            // Act
                            var result = CallPrevious(patterns: new IDateTimePattern[] { monthPattern, dayPattern }, now, tz, DateTimeRangeEdge.End);

                            // Assert
                            result.Should().Be(new DateTime(2020, 02, 29, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                        }
                    }
                }
            }

            public class WithoutEdgeAlignment
            {
                [Test]
                public void AtAnyTimeWhilePreviousYearWanted_Should_AdvanceToEndOfWantedTimeWithoutMilliseconds()
                {
                    // Arrange
                    var yearPattern = new YearPattern(year: 1999);
                    var now = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc);
                    var tz = TimeZoneInfo.Utc;

                    // Act
                    var sw = new Stopwatch();
                    sw.Start();
                    var result = CallPrevious(patterns: new IDateTimePattern[] { yearPattern }, now, tz);

                    // Assert
                    sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                    result.Should().Be(new DateTime(1999, 12, 31, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                }
            }

            public class Misc
            {
                [Test]
                public void PreviousOverloads_ShouldHave_DifferentResults()
                {
                    // Arrange
                    var monthPattern = new MonthPattern(month: 2);
                    var now = new DateTime(2000, 01, 14, 12, 30, 30, DateTimeKind.Utc);
                    var tz = TimeZoneInfo.Utc;

                    // Act
                    var sw = new Stopwatch();
                    sw.Start();
                    var resultPublicWithoutEdge = Calculator.Default.Previous(patterns: new IDateTimePattern[] { monthPattern }, now, tz);
                    var resultPublicWithEdgeBeginning = Calculator.Default.Previous(patterns: new IDateTimePattern[] { monthPattern }, now, tz, DateTimeRangeEdge.Beginning);
                    var resultInternalWithEdgeBeginning = CallPrevious(patterns: new IDateTimePattern[] { monthPattern }, now, tz, DateTimeRangeEdge.Beginning);

                    // Assert
                    sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                    resultPublicWithoutEdge.Should().NotBe(resultPublicWithEdgeBeginning);
                    resultPublicWithEdgeBeginning.Should().Be(resultInternalWithEdgeBeginning);
                }

                [Test]
                public void ImpossibleToAchievePattern_ShouldReturn_NullInNoTime()
                {
                    // Arrange
                    var yearPattern = new YearPattern(year: 1800);
                    var monthPattern = new MonthPattern(month: 1);
                    var dayPattern = new DayPattern(day: 1);
                    var dayOfWeekPattern = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Tuesday);
                    var now = DateTime.MaxValue;
                    var tz = TimeZoneInfo.Utc;

                    // Act
                    var sw = new Stopwatch();
                    sw.Start();
                    var result = Calculator.Default.Previous(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, dayOfWeekPattern }, now, tz);

                    // Assert
                    sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                    result.Should().BeNull();
                }
            }
        }

        static DateTime? CallNext(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz, DateTimeRangeEdge? edge = null)
        => DebugIterationCallback is null
            ? edge is null
                ? Calculator.Default.Next(patterns, startTime, tz)
                : Calculator.Default.Next(patterns, startTime, tz, edge.Value)
            : Calculator.Next(patterns, startTime, tz, edge, DebugIterationCallback);

        static DateTime? CallPrevious(IEnumerable<IDateTimePattern> patterns, DateTime startTime, TimeZoneInfo tz, DateTimeRangeEdge? edge = null)
        => DebugIterationCallback is null
            ? edge is null
                ? Calculator.Default.Previous(patterns, startTime, tz)
                : Calculator.Default.Previous(patterns, startTime, tz, edge.Value)
            : Calculator.Previous(patterns, startTime, tz, edge, DebugIterationCallback);

        static void DebugPrint2(DebugIterationEventArgs e)
        {
            var alternative = e.Alternatives.ElementAt(e.ChosenAlternative);
            System.Diagnostics.Debug.WriteLine($"Iteration #?, alternative: p={alternative.Pattern.GetType().Name}, dt={alternative.Dt}");
        }
    }
}