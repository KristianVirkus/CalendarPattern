using FluentAssertions;
using NUnit.Framework;
using System;

namespace CalendarPattern.UnitTests
{
    [TestFixture]
    internal class YearPatternTest
    {
        public class Constructors
        {
            [Test]
            public void ConstructorWithValidYear_Should_SetProperties()
            {
                // Arrange
                var year = (ushort)2000;

                // Act
                var sut = new YearPattern(year);

                // Assert
                sut.Year.Should().Be(year);
            }

            [Test]
            public void ConstructorWithYearTooLittle_ShouldThrow_ArgumentOutOfRangeException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new YearPattern(0000));
            }

            [Test]
            public void ConstructorWithYearTooGreat_ShouldThrow_ArgumentOutOfRangeException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new YearPattern(10000));
            }
        }

        public class Next
        {
            [Test]
            public void AtMiddleOfYearWhileNextYearWanted_Should_AdvanceToBeginningOfNextYear()
            {
                // Arrange
                var sut = new YearPattern((ushort)2000);
                var now = new DateTime(1999, 07, 01, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Next(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 01, 01, 00, 00, 00, DateTimeKind.Utc));
            }

            [Test]
            public void AtYear9999_ShouldReturn_Null()
            {
                // Arrange
                var sut = new YearPattern((ushort)9999);
                var now = new DateTime(9999, 01, 01, 00, 00, 00, DateTimeKind.Utc);
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
            public void AtMiddleOfYearWhilePreviousYearWanted_Should_AdvanceToEndOfPreviousYear()
            {
                // Arrange
                var sut = new YearPattern((ushort)2000);
                var now = new DateTime(2001, 07, 01, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().Be(new DateTime(2000, 12, 31, 23, 59, 59, DateTimeKind.Utc) + Constants.SecondMaximum);
            }

            [Test]
            public void AtYear0001_ShouldReturn_Null()
            {
                // Arrange
                var sut = new YearPattern((ushort)1);
                var now = new DateTime(0001, 01, 01, 00, 00, 00, DateTimeKind.Utc);
                var tz = TimeZoneInfo.Utc;

                // Act
                var result = sut.Previous(now, tz);

                // Assert
                result.Should().BeNull();
            }
        }
    }
}
