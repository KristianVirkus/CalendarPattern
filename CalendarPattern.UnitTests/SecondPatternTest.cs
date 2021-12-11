using FluentAssertions;
using NUnit.Framework;
using System;

namespace CalendarPattern.UnitTests
{
    [TestFixture]
    internal class SecondPatternTest
    {
        public class Constructors
        {
            [Test]
            public void ConstructorWithValidSecond_Should_SetProperties()
            {
                // Arrange
                var second = (byte)30;

                // Act
                var sut = new SecondPattern(second);

                // Assert
                sut.Second.Should().Be(second);
            }

            [Test]
            public void ConstructorWithSecondTooGreat_ShouldThrow_ArgumentOutOfRangeException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new SecondPattern(60));
            }
        }

        public class Next
        {
            [Test]
            public void AtBeginningOfSecondWhileSameSecondWanted_Should_AdvanceToBeginningOfSameSecondOfNextMinute()
            {
                // Arrange
                var sut = new SecondPattern((byte)10);
                var now = new DateTime(2000, 06, 01, 12, 00, 10, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 01, 12, 01, 10, DateTimeKind.Utc));
            }

            [Test]
            public void AtMiddleOfSecondWhileNextSecondWanted_Should_AdvanceToBeginningOfNextSecond()
            {
                // Arrange
                var sut = new SecondPattern((byte)31);
                var now = new DateTime(2000, 06, 01, 12, 30, 30, 500, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 01, 12, 30, 31, DateTimeKind.Utc));
            }

            [Test]
            public void AtMiddleOfSecondWhileBeginningOfSameSecondWanted_Should_AdvanceToBeginningOfNextMinute()
            {
                // Arrange
                var sut = new SecondPattern((byte)0);
                var now = new DateTime(2000, 06, 14, 12, 30, 30, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 14, 12, 31, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtEndOfDateTimeRangeWhileAnySecondWanted_ShouldReturn_Null()
            {
                // Arrange
                var sut = new SecondPattern((byte)0);
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
            public void AtEndOfSecondWhileSameSecondWanted_Should_AdvanceToEndOfSameSecondOfPreviousMinute()
            {
                // Arrange
                var sut = new SecondPattern((byte)10);
                var now = new DateTime(2000, 06, 01, 12, 00, 10, 500, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 01, 11, 59, 10, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtMiddleOfSecondWhilePreviousSecondWanted_Should_AdvanceToEndOfPreviousSecond()
            {
                // Arrange
                var sut = new SecondPattern((byte)29);
                var now = new DateTime(2000, 06, 01, 12, 30, 30, 500, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 01, 12, 30, 29, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtMiddleOfSecondWhileEndOfSameSecondWanted_Should_AdvanceToEndOfSameSecondOfPreviousMinute()
            {
                // Arrange
                var sut = new SecondPattern((byte)30);
                var now = new DateTime(2000, 06, 14, 12, 30, 30, DateTimeKind.Utc).Add(Constants.SecondMaximum);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 06, 14, 12, 29, 30, DateTimeKind.Utc).Add(Constants.SecondMaximum));
            }

            [Test]
            public void AtBeginningOfDateTimeRangeWhileAnySecondWanted_ShouldReturn_Null()
            {
                // Arrange
                var sut = new SecondPattern((byte)0);
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
