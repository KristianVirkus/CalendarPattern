using FluentAssertions;
using NUnit.Framework;
using System;

namespace CalendarPattern.UnitTests
{
    [TestFixture]
    internal class MinutePatternTest
    {
        static readonly TimeZoneInfo Cet;

        static MinutePatternTest()
        {
            Cet = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        }

        public class Constructors
        {
            [Test]
            public void ConstructorWithValidMinute_Should_SetProperties()
            {
                // Arrange
                var minute = (byte)30;

                // Act
                var sut = new MinutePattern(minute);

                // Assert
                sut.Minute.Should().Be(minute);
            }

            [Test]
            public void ConstructorWithMinuteTooGreat_ShouldThrow_ArgumentOutOfRangeException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new MinutePattern(60));
            }
        }

        public class Matching
        {
            [Test]
            public void SameMinute_Should_Match()
            {
                // Arrange
                var sut = new MinutePattern(minute: 30);

                // Act
                // Assert
                sut.Matches(dt: new DateTime(2022, 01, 18, 12, 30, 00)).Should().BeTrue();
            }

            [Test]
            public void OtherMinute_Should_NotMatch()
            {
                // Arrange
                var sut = new MinutePattern(minute: 00);

                // Act
                // Assert
                sut.Matches(dt: new DateTime(2022, 01, 18, 12, 30, 00)).Should().BeFalse();
            }
        }

        public class Next
        {
            [Test]
            public void AtBeginningOfMinuteWhileSameMinuteWanted_Should_AdvanceToBeginningOfSameMinuteOfNextHour()
            {
                // Arrange
                var sut = new MinutePattern((byte)10);
                var now = new DateTime(2000, 06, 01, 12, 10, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 01, 13, 10, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtMiddleOfMinuteWhileNextMinuteWanted_Should_AdvanceToBeginningOfNextMinute()
            {
                // Arrange
                var sut = new MinutePattern((byte)31);
                var now = new DateTime(2000, 06, 01, 12, 30, 30, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 01, 12, 31, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtMiddleOfMinuteWhileBeginningOfSameMinuteWanted_Should_AdvanceToBeginningOfSameMinuteOfNextHour()
            {
                // Arrange
                var sut = new MinutePattern((byte)30);
                var now = new DateTime(2000, 06, 14, 12, 30, 30, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 14, 13, 30, 00, DateTimeKind.Utc));
            }

            [Test]
            public void BeforeGapAt27March2022At0130CentralEuropeanWhileAnyMinute30Wanted_Should_AdvanceTo27March2022At0330_DueTo_DayLightSavings()
            {
                // Arrange
                var sut = new MinutePattern((byte)30);
                var now = new DateTime(2022, 03, 27, 01, 30, 00, DateTimeKind.Local);
                var tz = Cet;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2022, 03, 27, 03, 30, 00, DateTimeKind.Local));
            }

            [Test]
            public void AtEndOfDateTimeRangeWhileAnyMinuteWanted_ShouldReturn_Null()
            {
                // Arrange
                var sut = new MinutePattern((byte)0);
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
            public void AtEndOfMinuteWhileSameMinuteWanted_Should_AdvanceToEndOfSameMinuteOfPreviousHour()
            {
                // Arrange
                var sut = new MinutePattern((byte)10);
                var now = new DateTime(2000, 06, 01, 12, 10, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 01, 11, 10, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtMiddleOfMinuteWhilePreviousMinuteWanted_Should_AdvanceToEndOfPreviousMinute()
            {
                // Arrange
                var sut = new MinutePattern((byte)29);
                var now = new DateTime(2000, 06, 01, 12, 30, 30, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 01, 12, 29, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtMiddleOfMinuteWhileEndOfSameMinuteWanted_Should_AdvanceToEndOfMinuteOfPreviousHour()
            {
                // Arrange
                var sut = new MinutePattern((byte)30);
                var now = new DateTime(2000, 06, 14, 12, 30, 30, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 14, 11, 30, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AfterGapAt27March2022At0330CentralEuropeanWhileAnyMinute30Wanted_Should_AdvanceTo27March2022At0130_DueTo_DayLightSavings()
            {
                // Arrange
                var sut = new MinutePattern((byte)30);
                var now = new DateTime(2022, 03, 27, 03, 30, 00, DateTimeKind.Local);
                var tz = Cet;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2022, 03, 27, 01, 30, 59, DateTimeKind.Local).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtBeginningOfDateTimeRangeWhileAnyMinuteWanted_ShouldReturn_Null()
            {
                // Arrange
                var sut = new MinutePattern((byte)0);
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
