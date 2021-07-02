using System.Collections.Generic;
using FluentAssertions;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using Xunit;

namespace SchoolManagement.UnitTests.SchoolAggregateTests.SchoolsTests
{
    public class YearsOfEducationTests
    {
        [Fact]
        public void Creates_valid_years_of_education()
        {
            const int input = 2;

            var sut = YearsOfEducation.Create(input);

            sut.IsSuccess.Should().BeTrue();
            sut.Value.Value.Should().Be(input);
        }

        [Theory]
        [MemberData(nameof(ErrorData))]
        public void Can_detect_invalid_years_of_education(int inputLimit, string outputError, string propertyName = null)
        {
            var sut = propertyName is null
                ? YearsOfEducation.Create(inputLimit)
                : YearsOfEducation.Create(inputLimit, propertyName);
            sut.IsFailure.Should().BeTrue();
            sut.Error.Should().Be(outputError);
        }

        public static List<object[]> ErrorData()
        {
            return new List<object[]>
            {
                new object[] { 1, MinValueExceededError("years"), "years"},
                new object[] { 0, MinValueExceededError() },
                new object[] { -10, MinValueExceededError() },
                new object[] { 1000, MaxValueExceededError("years"), "years" },
                new object[] { 5, MaxValueExceededError() }
            };
        }

        static string MinValueExceededError(string propertyName = nameof(YearsOfEducation))
            => $"{propertyName} is required and must be at least {YearsOfEducation.MinValue}!";

        static string MaxValueExceededError(string propertyName = nameof(YearsOfEducation))
            => $"{propertyName} can not be greater than {YearsOfEducation.MaxValue}!";
    }
}
