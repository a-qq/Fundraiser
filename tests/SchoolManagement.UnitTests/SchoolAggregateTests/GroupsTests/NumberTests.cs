using FluentAssertions;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using System.Collections.Generic;
using Xunit;

namespace SchoolManagement.UnitTests.SchoolAggregateTests.GroupsTests
{
    public class NumberTests
    {
        [Fact]
        public void Creates_valid_number()
        {
            const int input = 1;

            var sut = Number.Create(input);

            sut.IsSuccess.Should().BeTrue();
            sut.Value.Value.Should().Be(input);
        }

        [Theory]
        [MemberData(nameof(ErrorData))]
        public void Can_detect_invalid_number(int inputNumber, string outputError, string propertyName = null)
        {
            var sut = propertyName is null 
                ? Number.Create(inputNumber) 
                : Number.Create(inputNumber, propertyName);

            sut.IsFailure.Should().BeTrue();
            sut.Error.Should().Be(outputError);
        }

        public static List<object[]> ErrorData()
        {
            return new List<object[]>
            {
                new object[] { 0, MinValueExceededError("numberProp"), "numberProp" },
                new object[] { -10, MinValueExceededError() },
                new object[] { 1000, MaxValueExceededError("numberProp"), "numberProp" },
                new object[] { 5, MaxValueExceededError() }
            };
        }

        static string MinValueExceededError(string propertyName = nameof(Number))
            => $"{propertyName} is required and must be at least {Number.MinValue}!";

        static string MaxValueExceededError(string propertyName = nameof(Number))
            => $"{propertyName} can not be greater than {Number.MaxValue}!";
    }
}
