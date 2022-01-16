using FluentAssertions;
using NUnit.Framework;
using System;

namespace CalendarPattern.UnitTests
{
    [TestFixture]
    internal class MonthPatternTest
    {
        public class Constructors
        {
            [Test]
            public void ConstructorWithValidMonth_Should_SetProperties()
            {
                // Arrange
                var month = (byte)6;

                // Act
                var sut = new MonthPattern(month);

                // Assert
                sut.Month.Should().Be(month);
            }

            [Test]
            public void ConstructorWithMonthTooLittle_ShouldThrow_ArgumentOutOfRangeException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new MonthPattern(0));
            }

            [Test]
            public void ConstructorWithMonthTooGreat_ShouldThrow_ArgumentOutOfRangeException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new MonthPattern(13));
            }
        }

        public class Matching
        {
            [Test]
            public void SameMonth_Should_Match()
            {
                // Arrange
                var sut = new MonthPattern(month: 8);

                // Act
                // Assert
                sut.Matches(dt: new DateTime(2022, 08, 18)).Should().BeTrue();
            }

            [Test]
            public void OtherMonth_Should_NotMatch()
            {
                // Arrange
                var sut = new MonthPattern(month: 12);

                // Act
                // Assert
                sut.Matches(dt: new DateTime(2022, 08, 18)).Should().BeFalse();
            }
        }

        public class Next
        {
            [Test]
            public void AtBeginningOfMonthWhileSameMonthWanted_Should_AdvanceToBeginningOfSameMonthOfNextYear()
            {
                // Arrange
                var sut = new MonthPattern((byte)6);
                var now = new DateTime(2000, 06, 01, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2001, 06, 01, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtMiddleOfMonthWhileNextMonthWanted_Should_AdvanceToBeginningOfNextMonth()
            {
                // Arrange
                var sut = new MonthPattern((byte)7);
                var now = new DateTime(2000, 06, 01, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 07, 01, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtMiddleOfDecemberWhileJanuaryWanted_Should_AdvanceToBeginningOfJanuaryOfNextYear()
            {
                // Arrange
                var sut = new MonthPattern((byte)1);
                var now = new DateTime(2000, 12, 14, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2001, 01, 01, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtEndOfDateTimeRangeWhileAnyMonthWanted_ShouldReturn_Null()
            {
                // Arrange
                var sut = new MonthPattern((byte)1);
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
            public void AtEndOfMonthWhileSameMonthWanted_Should_AdvanceToEndOfSameMonthOfPreviousYear()
            {
                // Arrange
                var sut = new MonthPattern((byte)6);
                var now = new DateTime(2000, 06, 30, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(1999, 06, 30, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtMiddleOfMonthWhilePreviousMonthWanted_Should_AdvanceToEndOfPreviousMonth()
            {
                // Arrange
                var sut = new MonthPattern((byte)7);
                var now = new DateTime(2000, 08, 01, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 07, 31, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtMiddleOfJanuaryWhileDecemberWanted_Should_AdvanceToEndOfDecemberOfPreviousYear()
            {
                // Arrange
                var sut = new MonthPattern((byte)12);
                var now = new DateTime(2000, 01, 14, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(1999, 12, 31, 23, 59, 59, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtBeginningOfDateTimeRangeWhileAnyMonthWanted_ShouldReturn_Null()
            {
                // Arrange
                var sut = new MonthPattern((byte)1);
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
