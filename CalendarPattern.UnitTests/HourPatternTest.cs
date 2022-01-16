using FluentAssertions;
using NUnit.Framework;
using System;

namespace CalendarPattern.UnitTests
{
    [TestFixture]
    internal class HourPatternTest
    {
        static readonly TimeZoneInfo Cet;

        static HourPatternTest()
        {
            Cet = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        }

        public class Constructors
        {
            [Test]
            public void ConstructorWithValidHour_Should_SetProperties()
            {
                // Arrange
                var hour = (byte)12;

                // Act
                var sut = new HourPattern(hour);

                // Assert
                sut.Hour.Should().Be(hour);
            }

            [Test]
            public void ConstructorWithHourTooGreat_ShouldThrow_ArgumentOutOfRangeException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new HourPattern((byte)24));
            }
        }

        public class Matching
        {
            [Test]
            public void SameHour_Should_Match()
            {
                // Arrange
                var sut = new HourPattern(hour: 12);

                // Act
                // Assert
                sut.Matches(dt: new DateTime(2022, 01, 18, 12, 00, 00)).Should().BeTrue();
            }

            [Test]
            public void OtherHour_Should_NotMatch()
            {
                // Arrange
                var sut = new HourPattern(hour: 14);

                // Act
                // Assert
                sut.Matches(dt: new DateTime(2022, 01, 18, 12, 00, 00)).Should().BeFalse();
            }
        }

        public class Next
        {
            [Test]
            public void AtBeginningOfHourWhileSameHourWanted_Should_AdvanceToBeginningOfSameHourOfNextDay()
            {
                // Arrange
                var sut = new HourPattern((byte)12);
                var now = new DateTime(2000, 06, 01, 12, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 02, 12, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtMiddleOfHourWhileNextHourWanted_Should_AdvanceToBeginningOfNextHour()
            {
                // Arrange
                var sut = new HourPattern((byte)13);
                var now = new DateTime(2000, 06, 01, 12, 30, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 01, 13, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtNoonOfDayWhileMidnightWanted_Should_AdvanceToMidnightOfNextDay()
            {
                // Arrange
                var sut = new HourPattern((byte)0);
                var now = new DateTime(2000, 06, 14, 12, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 15, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtGapAt27March2022At0100CentralEuropeanWhileAny0200Wanted_Should_AdvanceTo28March2022AtBeginningOf2am_DueTo_DayLightSavings()
            {
                // Arrange
                var sut = new HourPattern((byte)2);
                var now = new DateTime(2022, 03, 27, 01, 00, 00, DateTimeKind.Local);
                var tz = Cet;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2022, 03, 28, 02, 00, 00, DateTimeKind.Local));
            }

            [Test]
            public void AtEndOfDateTimeRangeWhileAnyHourWanted_ShouldReturn_Null()
            {
                // Arrange
                var sut = new HourPattern((byte)0);
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
            public void AtEndOfHourWhileSameHourWanted_Should_AdvanceToEndOfSameHourOfPreviousDay()
            {
                // Arrange
                var sut = new HourPattern((byte)12);
                var now = new DateTime(2000, 06, 01, 12, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 05, 31, 12, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtMiddleOfHourWhilePreviousHourWanted_Should_AdvanceToEndOfPreviousHour()
            {
                // Arrange
                var sut = new HourPattern((byte)11);
                var now = new DateTime(2000, 06, 01, 12, 30, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 01, 11, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtMidnightOfDayWhileNoonWanted_Should_AdvanceToNoonOfPreviousDay()
            {
                // Arrange
                var sut = new HourPattern((byte)12);
                var now = new DateTime(2000, 06, 14, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 13, 12, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtGapAt27March2022At0300CentralEuropeanWhileAny0200Wanted_Should_AdvanceTo26March2022AtEndOf2am_DueTo_DayLightSavings()
            {
                // Arrange
                var sut = new HourPattern((byte)2);
                var now = new DateTime(2022, 03, 27, 03, 00, 00, DateTimeKind.Local);
                var tz = Cet;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2022, 03, 26, 02, 59, 59, DateTimeKind.Local).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtBeginningOfDateTimeRangeWhileAnyHourWanted_ShouldReturn_Null()
            {
                // Arrange
                var sut = new HourPattern((byte)0);
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
