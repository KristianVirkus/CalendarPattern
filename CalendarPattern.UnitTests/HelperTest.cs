using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

namespace CalendarPattern.UnitTests
{
    [TestFixture]
    internal class HelperTest
    {
        public class Flags
        {
            [Test]
            public void SingleFlagWithValueZero_ShouldReturn_SingleFlag()
            {
                // Arrange
                var input = TestFlags.Zero;

                // Act
                var result = Helper.GetFlags(input);

                // Assert
                result.Should().HaveCount(1);
                result.Single().Should().Be(input);
            }

            [Test]
            public void MultipleFlags_ShouldReturn_AllSetFlags()
            {
                // Arrange
                var input = TestFlags.One | TestFlags.Two;

                // Act
                var result = Helper.GetFlags(input);

                // Assert
                result.Should().HaveCount(2);
                result.First().Should().Be(TestFlags.One);
                result.Last().Should().Be(TestFlags.Two);
            }

            [Test]
            public void UndefinedFlag_ShouldReturn_UndefinedFlag()
            {
                // Arrange
                var input = 1 << 3;

                // Act
                var result = Helper.GetFlags<TestFlags>((TestFlags)input);

                // Assert
                result.Should().HaveCount(1);
                ((int)result.Single()).Should().Be(input);
            }

            [Test]
            public void SinglePlainEnumNumericMemberWithValueZero_ShouldReturn_SingleEnumMember()
            {
                // Arrange
                var input = TestEnum.Zero;

                // Act
                var result = Helper.GetFlags(input);

                // Assert
                result.Should().HaveCount(1);
                result.Single().Should().Be(input);
            }

            [Test]
            public void MultiplePlainEnumNumericMembers_ShouldReturn_EnumMembers()
            {
                // Arrange
                var input = TestEnum.One | TestEnum.Two;

                // Act
                var result = Helper.GetFlags<TestEnum>(input);

                // Assert
                result.Should().HaveCount(2);
                result.First().Should().Be(TestEnum.One);
                result.Last().Should().Be(TestEnum.Two);
            }

            [Test]
            public void UndefinedPlainEnumNumericMember_ShouldReturn_UndefinedEnumMember()
            {
                // Arrange
                var input = 1 << 3;

                // Act
                var result = Helper.GetFlags<TestEnum>((TestEnum)input);

                // Assert
                result.Should().HaveCount(1);
                ((int)result.Single()).Should().Be(input);
            }

            [Flags]
            public enum TestFlags
            {
                Zero = 0,
                One = 1 << 0,
                Two = 1 << 1,
                Three = 1 << 2,
                Any = One | Two | Three
            }

            public enum TestEnum // No flags by intention.
            {
                Zero = 0,
                One = 1 << 0,
                Two = 1 << 1,
                Three = 1 << 2,
                Any = One | Two | Three
            }
        }

        public class Rank
        {
            public class Low
            {
                [Test]
                public void LowerRankedForNullArgument_ShouldThrow_ArgumentNullException()
                {
                    // Arrange
                    // Act & Assert
                    Assert.Throws<ArgumentNullException>(() => Helper.GetLowerRankedDateTimeComponents(null));
                }

                [Test]
                public void LowerRankedForNoComponentAsArgument_ShouldReturn_AllComponents()
                {
                    // Arrange
                    // Act
                    var results = Helper.GetLowerRankedDateTimeComponents(Array.Empty<DateTimeComponent>());

                    // Assert
                    var expectedCount = Enum.GetValues(typeof(DateTimeComponent)).Length - 2 /* exclude `None`, and `Any` */;
                    results.Should().HaveCount(expectedCount);
                    results.Should().Contain(Enum.GetValues(typeof(DateTimeComponent)).Cast<DateTimeComponent>().Where(c => c != DateTimeComponent.None && c != DateTimeComponent.Any));
                }

                [Test]
                public void LowerRankedForLowestComponentAsArgument_ShouldReturn_EmptyList()
                {
                    // Arrange
                    var usedComponents = new DateTimeComponent[] { DateTimeComponent.Ticks };

                    // Act
                    var results = Helper.GetLowerRankedDateTimeComponents(usedComponents);

                    // Assert
                    results.Should().BeEmpty();
                }

                [Test]
                public void LowerRankedForHightestComponentAsArgument_ShouldReturn_AllLowerRankedComponents()
                {
                    // Arrange
                    var usedComponents = new DateTimeComponent[] { DateTimeComponent.Year };

                    // Act
                    var results = Helper.GetLowerRankedDateTimeComponents(usedComponents);

                    // Assert
                    var expectedCount = Enum.GetValues(typeof(DateTimeComponent)).Length - 3 /* exclude `None`, `Any`, and `Year` */;
                    results.Should().HaveCount(expectedCount);
                    results.Should().Contain(Enum.GetValues(typeof(DateTimeComponent)).Cast<DateTimeComponent>().Where(c => c != DateTimeComponent.None && c != DateTimeComponent.Any && c != DateTimeComponent.Year));
                }

                [Test]
                public void LowerRankedForMultipleComponentsAsArgument_ShouldReturn_AllComponentsRankedLowerThanTheLowestComponentFromArgument()
                {
                    // Arrange
                    var usedComponents = new DateTimeComponent[] { DateTimeComponent.Year, DateTimeComponent.Millisecond };

                    // Act
                    var results = Helper.GetLowerRankedDateTimeComponents(usedComponents);

                    // Assert
                    results.Should().HaveCount(1);
                    results.Single().Should().Be(DateTimeComponent.Ticks);
                }
            }

            public class High
            {
                [Test]
                public void HigherRankedForNullArgument_ShouldThrow_ArgumentNullException()
                {
                    // Arrange
                    // Act & Assert
                    Assert.Throws<ArgumentNullException>(() => Helper.GetHigherRankedDateTimeComponents(null));
                }

                [Test]
                public void HigherRankedForNoComponentAsArgument_ShouldReturn_AllComponents()
                {
                    // Arrange
                    // Act
                    var results = Helper.GetHigherRankedDateTimeComponents(Array.Empty<DateTimeComponent>());

                    // Assert
                    var expectedCount = Enum.GetValues(typeof(DateTimeComponent)).Length - 2 /* exclude `None`, and `Any` */;
                    results.Should().HaveCount(expectedCount);
                    results.Should().Contain(Enum.GetValues(typeof(DateTimeComponent)).Cast<DateTimeComponent>().Where(c => c != DateTimeComponent.None && c != DateTimeComponent.Any));
                }

                [Test]
                public void HigherRankedForHighestComponentAsArgument_ShouldReturn_EmptyList()
                {
                    // Arrange
                    var usedComponents = new DateTimeComponent[] { DateTimeComponent.Year };

                    // Act
                    var results = Helper.GetHigherRankedDateTimeComponents(usedComponents);

                    // Assert
                    results.Should().BeEmpty();
                }

                [Test]
                public void HigherRankedForLowestComponentAsArgument_ShouldReturn_AllHigherRankedComponents()
                {
                    // Arrange
                    var usedComponents = new DateTimeComponent[] { DateTimeComponent.Ticks };

                    // Act
                    var results = Helper.GetHigherRankedDateTimeComponents(usedComponents);

                    // Assert
                    var expectedCount = Enum.GetValues(typeof(DateTimeComponent)).Length - 3 /* exclude `None`, `Any`, and `Ticks` */;
                    results.Should().HaveCount(expectedCount);
                    results.Should().Contain(Enum.GetValues(typeof(DateTimeComponent)).Cast<DateTimeComponent>().Where(c => c != DateTimeComponent.None && c != DateTimeComponent.Any && c != DateTimeComponent.Ticks));
                }

                [Test]
                public void HigherRankedForMultipleComponentsAsArgument_ShouldReturn_AllComponentsRankedHigherThanTheHighestComponentFromArgument()
                {
                    // Arrange
                    var usedComponents = new DateTimeComponent[] { DateTimeComponent.Month, DateTimeComponent.Ticks };

                    // Act
                    var results = Helper.GetHigherRankedDateTimeComponents(usedComponents);

                    // Assert
                    results.Should().HaveCount(1);
                    results.Single().Should().Be(DateTimeComponent.Year);
                }
            }
        }

        public class DateTimeComponents
        {
            private readonly DateTime Dt = new DateTime(2022, 01, 08, 22, 44, 13, 789, DateTimeKind.Utc).AddTicks(1234);

            [Test]
            public void UndefinedPlainEnumNumericMemberDateTimeComponent_ShouldThrow_NotSupportedException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<NotSupportedException>(() => Helper.GetDateTimeComponent(dt: Dt, component: (DateTimeComponent)1234));
            }

            [Test]
            public void Year_Should_ExtractYear()
            {
                // Arrange
                // Act
                // Assert
                Helper.GetDateTimeComponent(dt: Dt, component: DateTimeComponent.Year).Should().Be((ushort)Dt.Year);
            }

            [Test]
            public void Month_Should_ExtractMonth()
            {
                // Arrange
                // Act
                // Assert
                Helper.GetDateTimeComponent(dt: Dt, component: DateTimeComponent.Month).Should().Be((ushort)Dt.Month);
            }

            [Test]
            public void Day_Should_ExtractDay()
            {
                // Arrange
                // Act
                // Assert
                Helper.GetDateTimeComponent(dt: Dt, component: DateTimeComponent.Day).Should().Be((ushort)Dt.Day);
            }

            [Test]
            public void Hour_Should_ExtractHour()
            {
                // Arrange
                // Act
                // Assert
                Helper.GetDateTimeComponent(dt: Dt, component: DateTimeComponent.Hour).Should().Be((ushort)Dt.Hour);
            }

            [Test]
            public void Minute_Should_ExtractMinute()
            {
                // Arrange
                // Act
                // Assert
                Helper.GetDateTimeComponent(dt: Dt, component: DateTimeComponent.Minute).Should().Be((ushort)Dt.Minute);
            }

            [Test]
            public void Second_Should_ExtractSecond()
            {
                // Arrange
                // Act
                // Assert
                Helper.GetDateTimeComponent(dt: Dt, component: DateTimeComponent.Second).Should().Be((ushort)Dt.Second);
            }

            [Test]
            public void Milliseconds_Should_ExtractMilliseconds()
            {
                // Arrange
                // Act
                // Assert
                Helper.GetDateTimeComponent(dt: Dt, component: DateTimeComponent.Millisecond).Should().Be((ushort)Dt.Millisecond);
            }

            [Test]
            public void Ticks_Should_ExtractAdditionalTicks()
            {
                // Arrange
                // Act
                // Assert
                Helper.GetDateTimeComponent(dt: Dt, component: DateTimeComponent.Ticks).Should().Be((ushort)(Dt - new DateTime(Dt.Year, Dt.Month, Dt.Day, Dt.Hour, Dt.Minute, Dt.Second, Dt.Millisecond, Dt.Kind)).Ticks);
            }
        }

        public class EdgeAlignment
        {
            public class Misc
            {
                [Test]
                public void InvalidEdgeValue_ShouldThrow_NotSupportedException()
                {
                    // Arrange
                    // Act & Assert
                    Assert.Throws<NotSupportedException>(() => Helper.AlignDateTimeComponentsToEdge(DateTime.UtcNow, (DateTimeRangeEdge)0xfffe, new[] { DateTimeComponent.Year }));
                }

                [Test]
                public void DateTimeComponentsNull_ShouldThrow_ArgumentNullException()
                {
                    // Arrange
                    // Act & Assert
                    Assert.Throws<ArgumentNullException>(() => Helper.AlignDateTimeComponentsToEdge(DateTime.UtcNow, DateTimeRangeEdge.End, null));
                }

                [Test]
                public void DateTimeComponentsEmpty_ShouldNot_ChangeDateAndTime()
                {
                    // Arrange
                    var utcNow = DateTime.UtcNow;

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(utcNow, DateTimeRangeEdge.End, Array.Empty<DateTimeComponent>());

                    // Assert
                    result.Should().Be(utcNow);
                }

                [Test]
                public void InvalidDateTimeComponent_ShouldThrow_NotSupportedException()
                {
                    // Arrange
                    // Act & Assert
                    Assert.Throws<NotSupportedException>(() => Helper.AlignDateTimeComponentsToEdge(DateTime.UtcNow, DateTimeRangeEdge.End, new DateTimeComponent[] { (DateTimeComponent)0xfffe }));
                }
            }

            public class Beginning
            {
                [Test]
                public void AtAnyTimeWithYearComponentToAlign_Should_OnlyAlignYear()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.Beginning, new[] { DateTimeComponent.Year });

                    // Assert
                    result.Should().Be(new DateTime(0001, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithMonthComponentToAlign_Should_OnlyAlignMonth()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.Beginning, new[] { DateTimeComponent.Month });

                    // Assert
                    result.Should().Be(new DateTime(2000, 01, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithDayComponentToAlign_Should_OnlyAlignDay()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.Beginning, new[] { DateTimeComponent.Day });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 01, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithHourComponentToAlign_Should_OnlyAlignHour()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.Beginning, new[] { DateTimeComponent.Hour });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 14, 00, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithMinuteComponentToAlign_Should_OnlyAlignMinute()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.Beginning, new[] { DateTimeComponent.Minute });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 14, 12, 00, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithSecondComponentToAlign_Should_OnlyAlignSecond()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.Beginning, new[] { DateTimeComponent.Second });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 14, 12, 30, 00, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithMillisecondComponentToAlign_Should_OnlyAlignMillisecond()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.Beginning, new[] { DateTimeComponent.Millisecond });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 14, 12, 30, 30, 000, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithTicksComponentToAlign_Should_OnlyAlignTicks()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.Beginning, new[] { DateTimeComponent.Ticks });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(0000));
                }

                [Test]
                public void AtAnyTimeWithMultipleComponentsToAlign_Should_OnlyAlignSelectedComponents()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.Beginning, new[] { DateTimeComponent.Month, DateTimeComponent.Hour, DateTimeComponent.Millisecond });

                    // Assert
                    result.Should().Be(new DateTime(2000, 01, 14, 00, 30, 30, 000, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }
            }

            public class End
            {
                [Test]
                public void AtAnyTimeWithYearComponentToAlign_Should_OnlyAlignYear()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.End, new[] { DateTimeComponent.Year });

                    // Assert
                    result.Should().Be(new DateTime(9999, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithMonthComponentToAlign_Should_OnlyAlignMonth()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.End, new[] { DateTimeComponent.Month });

                    // Assert
                    result.Should().Be(new DateTime(2000, 12, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithDayComponentToAlign_Should_OnlyAlignDay()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.End, new[] { DateTimeComponent.Day });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 30, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtFebruaryWithDayComponentToAlign_Should_OnlyAlignDayAndRegardDaysInMonth()
                {
                    // Arrange
                    var dt = new DateTime(2000, 02, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.End, new[] { DateTimeComponent.Day });

                    // Assert
                    result.Should().Be(new DateTime(2000, 02, 29, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithHourComponentToAlign_Should_OnlyAlignHour()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.End, new[] { DateTimeComponent.Hour });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 14, 23, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithMinuteComponentToAlign_Should_OnlyAlignMinute()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.End, new[] { DateTimeComponent.Minute });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 14, 12, 59, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithSecondComponentToAlign_Should_OnlyAlignSecond()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.End, new[] { DateTimeComponent.Second });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 14, 12, 30, 59, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithMillisecondComponentToAlign_Should_OnlyAlignMillisecond()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.End, new[] { DateTimeComponent.Millisecond });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 14, 12, 30, 30, 999, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }

                [Test]
                public void AtAnyTimeWithTicksComponentToAlign_Should_OnlyAlignTicks()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.End, new[] { DateTimeComponent.Ticks });

                    // Assert
                    result.Should().Be(new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(9999));
                }

                [Test]
                public void AtAnyTimeWithMultipleComponentsToAlign_Should_OnlyAlignSelectedComponents()
                {
                    // Arrange
                    var dt = new DateTime(2000, 06, 14, 12, 30, 30, 500, DateTimeKind.Utc) + TimeSpan.FromTicks(5000);

                    // Act
                    var result = Helper.AlignDateTimeComponentsToEdge(dt, DateTimeRangeEdge.End, new[] { DateTimeComponent.Month, DateTimeComponent.Hour, DateTimeComponent.Millisecond });

                    // Assert
                    result.Should().Be(new DateTime(2000, 12, 14, 23, 30, 30, 999, DateTimeKind.Utc) + TimeSpan.FromTicks(5000));
                }
            }
        }

        public class BoundsCompliance
        {
            [Test]
            public void UndefinedPlainEnumNumericMemberDirection_ShouldThrow_NotSupportedException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<NotSupportedException>(() => Helper.ComplyWithBound(dt: DateTime.MaxValue, bound: DateTime.UtcNow, component: DateTimeComponent.Day, componentValue: 1, direction: (Helper.CalculationDirection)1234));
            }

            [Test]
            public void UndefinedPlainEnumNumericMemberDirectionWithHighestRankedComponentAsParameter_ShouldThrow_NotSupportedException()
            {
                // Arrange
                // Act & Assert
                Assert.Throws<NotSupportedException>(() => Helper.ComplyWithBound(dt: DateTime.MaxValue, bound: DateTime.MaxValue.AddYears(-1), component: DateTimeComponent.Year, componentValue: DateTime.MaxValue.Year, direction: (Helper.CalculationDirection)1234));
            }

            [Test]
            public void CandidateExactlyAtBound_ShouldReturn_True()
            {
                // Arrange
                var dt = new DateTime(2022, 01, 12, 20, 41, 12, 345, DateTimeKind.Utc);

                // Act
                // Assert
                Helper.ComplyWithBound(dt: dt, bound: dt, component: DateTimeComponent.Minute, componentValue: dt.Minute, direction: Helper.CalculationDirection.Next).Should().BeTrue();
            }

            [Test]
            public void CandidateBeforeBoundWithDirectionNext_ShouldReturn_True()
            {
                // Arrange
                var dt = new DateTime(2022, 01, 12, 20, 41, 12, 345, DateTimeKind.Utc);

                // Act
                // Assert
                Helper.ComplyWithBound(dt: dt, bound: DateTime.MaxValue, component: DateTimeComponent.Minute, componentValue: dt.Minute, direction: Helper.CalculationDirection.Next).Should().BeTrue();
            }

            [Test]
            public void CandidateAfterBoundWithDirectionNext_ShouldReturn_False()
            {
                // Arrange
                var dt = new DateTime(2022, 01, 12, 20, 41, 12, 345, DateTimeKind.Utc);

                // Act
                // Assert
                Helper.ComplyWithBound(dt: dt, bound: dt.AddDays(-2), component: DateTimeComponent.Minute, componentValue: dt.Minute, direction: Helper.CalculationDirection.Next).Should().BeFalse();
            }

            [Test]
            public void CandidateOnlyWithNewNextComponentValueNotWithinBound_ShouldReturn_False()
            {
                // Arrange
                var dt = new DateTime(2022, 01, 12, 20, 41, 12, 345, DateTimeKind.Utc);

                // Act
                // Assert
                Helper.ComplyWithBound(dt: dt, bound: dt.AddMinutes(-1), component: DateTimeComponent.Minute, componentValue: dt.Minute, direction: Helper.CalculationDirection.Next).Should().BeFalse();
            }

            [Test]
            public void CandidateAfterBoundWithDirectionPrevious_ShouldReturn_True()
            {
                // Arrange
                var dt = new DateTime(2022, 01, 12, 20, 41, 12, 345, DateTimeKind.Utc);

                // Act
                // Assert
                Helper.ComplyWithBound(dt: dt, bound: DateTime.MinValue, component: DateTimeComponent.Minute, componentValue: dt.Minute, direction: Helper.CalculationDirection.Previous).Should().BeTrue();
            }

            [Test]
            public void CandidateBeforeBoundWithDirectionPrevious_ShouldReturn_False()
            {
                // Arrange
                var dt = new DateTime(2022, 01, 12, 20, 41, 12, 345, DateTimeKind.Utc);

                // Act
                // Assert
                Helper.ComplyWithBound(dt: dt, bound: dt.AddDays(2), component: DateTimeComponent.Minute, componentValue: dt.Minute, direction: Helper.CalculationDirection.Previous).Should().BeFalse();
            }

            [Test]
            public void CandidateOnlyWithNewPreviousComponentValueNotWithinBound_ShouldReturn_False()
            {
                // Arrange
                var dt = new DateTime(2022, 01, 12, 20, 41, 12, 345, DateTimeKind.Utc);

                // Act
                // Assert
                Helper.ComplyWithBound(dt: dt, bound: dt.AddMinutes(1), component: DateTimeComponent.Minute, componentValue: dt.Minute, direction: Helper.CalculationDirection.Previous).Should().BeFalse();
            }
        }
    }
}
