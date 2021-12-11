using FluentAssertions;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;

namespace CalendarPattern.UnitTests
{
    [TestFixture]
    public class CalculatorTest
    {
        static readonly TimeZoneInfo Cet;

        static CalculatorTest()
        {
            Cet = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        }

        public class Next
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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { yearPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { monthPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { dayPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { hourPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { minutePattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { secondPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { monthPattern, dayPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

                        // Assert
                        result.Should().Be(new DateTime(2024, 02, 29, 00, 00, 00, DateTimeKind.Utc));
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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { yearPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { monthPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { dayPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { hourPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { minutePattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { secondPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

                        // Assert
                        sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
                        result.Should().Be(new DateTime(2000, 06, 14, 12, 30, 02, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                    }
                }
            }
        }

        public class Previous
        {
            public class EdgeBeginning
            {
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
                        var result = Calculator.Previous(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern }, now, tz, DateTimeRangeEdge.End/*, debugPrint*/);

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
                        var result = Calculator.Previous(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Next(patterns: new IDateTimePattern[] { yearPattern, monthPattern, dayPattern, hourPattern, minutePattern, secondPattern }, now, tz, DateTimeRangeEdge.Beginning, DebugPrint);

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
                        var result = Calculator.Previous(patterns: new IDateTimePattern[] { yearPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Previous(patterns: new IDateTimePattern[] { monthPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Previous(patterns: new IDateTimePattern[] { dayPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Previous(patterns: new IDateTimePattern[] { hourPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Previous(patterns: new IDateTimePattern[] { minutePattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Previous(patterns: new IDateTimePattern[] { secondPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

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
                        var result = Calculator.Previous(patterns: new IDateTimePattern[] { monthPattern, dayPattern }, now, tz, DateTimeRangeEdge.End, DebugPrint);

                        // Assert
                        result.Should().Be(new DateTime(2020, 02, 29, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
                    }
                }
            }
        }

        static void DebugPrint(DebugIterationEventArgs e)
        {
            var alternative = e.Alternatives.ElementAt(e.ChosenAlternative);
            System.Diagnostics.Debug.WriteLine($"Iteration #?, alternative: p={alternative.Pattern.GetType().Name}, dt={alternative.Dt}");
        }
    }
}