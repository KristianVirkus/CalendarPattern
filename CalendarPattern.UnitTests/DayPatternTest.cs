using FluentAssertions;
using NUnit.Framework;
using System;

namespace CalendarPattern.UnitTests
{
    [TestFixture]
    internal class DayPatternTest
    {
        public class Constructors
        {
            [Test]
            public void ConstructorWithValidDay_Should_SetProperties()
            {
                // Arrange
                var day = (byte)14;

                // Act
                var sut = new DayPattern(day);

                // Assert
                sut.Day.Should().Be(day);
            }

            [Test]
            public void ConstructorWithDayTooLittle_ShouldThrow_ArgumentOutOfRangeException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new DayPattern(0));
            }

            [Test]
            public void ConstructorWithDayTooGreat_ShouldThrow_ArgumentOutOfRangeException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new DayPattern(32));
            }
        }

        public class Next
        {
            [Test]
            public void AtBeginningOfDayWhileSameDayWanted_Should_AdvanceToBeginningOfSameDayOfNextMonth()
            {
                // Arrange
                var sut = new DayPattern(day: (byte)14);
                var now = new DateTime(2000, 06, 14, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 07, 14, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtMiddleOfDayWhileNextDayWanted_Should_AdvanceToBeginningOfNextDay()
            {
                // Arrange
                var sut = new DayPattern(day: (byte)5);
                var now = new DateTime(2000, 06, 04, 12, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 05, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void At14thOfMonthWhile1stWanted_Should_AdvanceToBeginningOfNextMonth()
            {
                // Arrange
                var sut = new DayPattern(day: (byte)1);
                var now = new DateTime(2000, 06, 14, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 07, 01, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void At31thOfMonthWhileAny31thWanted_Should_AlwaysAdvanceToNext31thSkippingTwoMonth()
            {
                // Arrange
                var sut = new DayPattern(day: (byte)31);
                var now = new DateTime(2000, 03, 31, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 05, 31, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void At31thOfJulyWhileAny31thWanted_Should_AlwaysAdvanceToNext31thInAugust()
            {
                // Arrange
                var sut = new DayPattern(day: (byte)31);
                var now = new DateTime(2000, 07, 31, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 08, 31, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void At01Feb2000WhileAny31thWanted_Should_AdvanceTo31Mar2000()
            {
                // Arrange
                var sut = new DayPattern(day: (byte)31);
                var now = new DateTime(2000, 02, 01, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 03, 31, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtEndOfDateTimeRangeWhileAnyDayWanted_ShouldReturn_Null()
            {
                // Arrange
                var sut = new DayPattern((byte)1);
                var now = DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().BeNull();
            }
        }

        public class Previous
        {
            [Test]
            public void AtEndOfDayWhileSameDayWanted_Should_AdvanceToEndOfSameDayOfPreviousMonth()
            {
                // Arrange
                var sut = new DayPattern(day: (byte)14);
                var now = new DateTime(2000, 06, 14, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 05, 14, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtMiddleOfDayWhilePreviousDayWanted_Should_AdvanceToEndOfPreviousDay()
            {
                // Arrange
                var sut = new DayPattern(day: (byte)5);
                var now = new DateTime(2000, 06, 06, 12, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 05, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void At1stOfMonthWhile31thWanted_Should_AdvanceToEndOfPreviousMonth()
            {
                // Arrange
                var sut = new DayPattern(day: (byte)31);
                var now = new DateTime(2000, 06, 01, 12, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 05, 31, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void At31thOfMonthWhileAny31thWanted_Should_AlwaysAdvanceToPrevious31thSkippingTwoMonth()
            {
                // Arrange
                var sut = new DayPattern(day: (byte)31);
                var now = new DateTime(2000, 05, 31, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 03, 31, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void At31thOfAugustWhileAny31thWanted_Should_AlwaysAdvanceToPrevious31thInJuly()
            {
                // Arrange
                var sut = new DayPattern(day: (byte)31);
                var now = new DateTime(2000, 08, 31, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 07, 31, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtBeginningOfDateTimeRangeWhileAnyDayWanted_ShouldReturn_Null()
            {
                // Arrange
                var sut = new DayPattern((byte)1);
                var now = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().BeNull();
            }
        }
    }
}
