using FluentAssertions;
using NUnit.Framework;
using System;

namespace CalendarPattern.UnitTests
{
    [TestFixture]
    internal class DayOfWeekPatternTest
    {
        public class Constructors
        {
            [Test]
            public void ConstructorWithValidDayOfWeek_Should_SetProperties()
            {
                // Arrange
                var dayOfWeek = DayOfWeek.Wednesday;

                // Act
                var sut = new DayOfWeekPattern(dayOfWeek);

                // Assert
                sut.DayOfWeek.Should().Be(dayOfWeek);
            }

            [Test]
            public void ConstructorWithDayOfWeekUndefinedPlainNumericEnum_ShouldThrow_ArgumentOutOfRangeException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new DayOfWeekPattern(dayOfWeek: (DayOfWeek)1234));
            }
        }

        public class Matching
        {
            [Test]
            public void SameDayOfWeek_Should_Match()
            {
                // Arrange
                var sut = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Tuesday);

                // Act
                // Assert
                sut.Matches(dt: new DateTime(2022, 01, 18)).Should().BeTrue();
            }

            [Test]
            public void OtherDayOfWeek_Should_NotMatch()
            {
                // Arrange
                var sut = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Tuesday);

                // Act
                // Assert
                sut.Matches(dt: new DateTime(2022, 01, 16)).Should().BeFalse();
            }
        }

        public class Next
        {
            [Test]
            public void AtBeginningOfWeekWhileSameDayOfWeekWanted_Should_AdvanceToBeginningOfSameDayOfNextWeek()
            {
                // Arrange
                var sut = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Sunday);
                var now = new DateTime(2022, 01, 16, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2022, 01, 23, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtMiddleOfWeekWhileNextDayOfWeekWanted_Should_AdvanceToBeginningOfNextDayOfWeek()
            {
                // Arrange
                var sut = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Thursday);
                var now = new DateTime(2022, 01, 12, 12, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2022, 01, 13, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtMiddleOfWeekWhileBeginningOfWeekWanted_Should_AdvanceToBeginningOfNextWeek()
            {
                // Arrange
                var sut = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Sunday);
                var now = new DateTime(2022, 01, 12, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2022, 01, 16, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void NearEndOfDateTimeRangeWhileDayOfWeekWithinSameYearWanted_ShouldReturn_NextDayOfWeek()
            {
                // Arrange
                var sut = new DayOfWeekPattern(DayOfWeek.Sunday);
                var now = DateTime.SpecifyKind(new DateTime(9999, 12, 01), DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().BeAfter(now);
            }

            [Test]
            public void AtEndOf9998_ShouldReturn_ProceedToNexteYear()
            {
                // Arrange
                var sut = new DayOfWeekPattern(DayOfWeek.Sunday);
                var now = DateTime.SpecifyKind(new DateTime(9998, 12, 31), DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().BeAfter(now);
            }

            [Test]
            public void AtEndOf9999_ShouldReturn_Null()
            {
                // Arrange
                var sut = new DayOfWeekPattern(DayOfWeek.Sunday);
                var now = DateTime.SpecifyKind(new DateTime(9999, 12, 31), DateTimeKind.Utc);
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
            public void AtEndOfWeekWhileSameDayOfWeekWanted_Should_AdvanceToSameDayOfWeekOfPreviousMonth()
            {
                // Arrange
                var sut = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Saturday);
                var now = new DateTime(2022, 01, 15, 12, 00, 00);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2022, 01, 08, 23, 59, 59).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtMiddleOfWeekWhilePreviousDayWanted_Should_AdvanceToEndOfPreviousDay()
            {
                // Arrange
                var sut = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Tuesday);
                var now = new DateTime(2022, 01, 12, 12, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2022, 01, 11, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtBeginningOfWeekWhileEndOfWeekWanted_Should_AdvanceToEndOfPreviousWeek()
            {
                // Arrange
                var sut = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Saturday);
                var now = new DateTime(2022, 01, 16, 12, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2022, 01, 15, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtEndOfWeekWhileAnyEndOfWeekWanted_Should_AlwaysAdvanceToPreviousEndOfWeek()
            {
                // Arrange
                var sut = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Saturday);
                var now = new DateTime(2022, 01, 15, 12, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2022, 01, 08, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtBeginningOfDateTimeRangeWhileAnyDayOfWeekWanted_ShouldReturn_Null()
            {
                // Arrange
                var sut = new DayOfWeekPattern(dayOfWeek: DayOfWeek.Saturday);
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
