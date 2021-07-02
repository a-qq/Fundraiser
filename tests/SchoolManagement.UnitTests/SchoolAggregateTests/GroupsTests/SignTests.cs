using FluentAssertions;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using System.Collections.Generic;
using Xunit;

namespace SchoolManagement.UnitTests.SchoolAggregateTests.GroupsTests
{
    public class SignTests
    {
        [Fact]
        public void Creates_valid_sign()
        {
            const string inputName = " EfDI ";

            var sut = Sign.Create(inputName);

            sut.IsSuccess.Should().BeTrue();

            sut.Value.Value.Should().Be("EfDI");
        }

        [Theory]
        [MemberData(nameof(ErrorData))]
        public void Can_detect_invalid_sign(string inputSign, IEnumerable<string> outputErrors, string propertyName = null)
        {
            var sut = propertyName is null
                ? Sign.Create(inputSign)
                : Sign.Create(inputSign, propertyName);

            sut.IsFailure.Should().BeTrue();
            sut.Error.Errors.Should().BeEquivalentTo(outputErrors);
        }

        public static List<object[]> ErrorData()
        {
            return new List<object[]>
            {
                new object[] {null, new[] {SignRequiredError("groupSign")}, "groupSign"},
                new object[] {string.Empty, new[] {SignRequiredError()}},
                new object[] {" ", new[] {SignRequiredError()}},
                new object[] {"asdfc ", new[] {SignLengthExceededError("groupSign")}, "groupSign"},
                new object[] {"asdfc1 ", new[] {OnlyLettersAllowedError(), SignLengthExceededError()}},
                new object[] {" e-di ", new[] {OnlyLettersAllowedError()}}
            };
        }

        static string SignRequiredError(string propertyName = nameof(Sign))
            => $"{propertyName} is required!";

        static string SignLengthExceededError(string propertyName = nameof(Sign))
            => $"{propertyName} should consist of max {Sign.MaxLength} characters!";

        static string OnlyLettersAllowedError(string propertyName = nameof(Sign))
            => $"{propertyName} should consist of only letters!";
    }
}
