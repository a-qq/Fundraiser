using FluentAssertions;
using SchoolManagement.Domain.SchoolAggregate.Members;
using System.Collections.Generic;
using Xunit;

namespace SchoolManagement.UnitTests.SchoolAggregateTests.MembersTests
{
    public class LastNameTests
    {
        [Fact]
        public void Creates_valid_last_name()
        {
            const string inputLastName = " noWak-kowaLska ";

            var sut = LastName.Create(inputLastName);

            sut.IsSuccess.Should().BeTrue();

            sut.Value.Value.Should().Be("Nowak-Kowalska");
        }

        [Theory]
        [MemberData(nameof(ErrorData))]
        public void Can_detect_invalid_last_name(string inputFirstName, IEnumerable<string> outputError, string propertyName = null)
        {
            var sut = propertyName is null
                ? LastName.Create(inputFirstName)
                : LastName.Create(inputFirstName, propertyName);

            sut.IsFailure.Should().BeTrue();
            sut.Error.Errors.Should().BeEquivalentTo(outputError);
        }

        public static List<object[]> ErrorData()
        {
            return new List<object[]>
            {
                new object[] { null, new[] {LastNameRequiredError("Name")}, "Name" },
                new object[] { string.Empty, new[] {LastNameRequiredError()} },
                new object[] { " ", new[] {LastNameRequiredError()} },
                new object[]
                {
                    "rqpfvbxqdscaertcfbfiigjtrnpvonyjtppvemofuzqezatmpxaffqaaeirirrxqaginpkmhsvvautdpholtefcxeftyviebpbldeycqnscqaqzodimslaiumtpgueezwofeegeckfuecxjuggtoxgkywiadwvbhszdacztttuduipdigwihamlbzurmnokvajeifnpva",
                    new[]{MaxLengthExceededError("Firstname")},
                    "Firstname"
                },
                new object[] { " a ", new[] {LastNameNotLongEnoughError("Lastname")}, "Lastname" },
                new object[] { " 1", new[] { OnlyLettersWithOneDashAllowedError(), LastNameNotLongEnoughError() } },
                new object[] { "n0wak ", new[] {OnlyLettersWithOneDashAllowedError("TestName")}, "TestName"},
                new object[] { " -kowalska", new[] {OnlyLettersWithOneDashAllowedError()}},
                new object[] { " nowak- ", new[] {OnlyLettersWithOneDashAllowedError()}},
                new object[]
                {
                    "rqpfvbxqdscae1tcfbfiigjtrnpvonyjtppvemofuzqezatmpxaffqaaeirirrxqaginpkmhsvvautdpholtefcxeftyviebpbldeycqnscqaqzodimslaiumtpgueezwofeegeckfuecxjuggtoxgkywiadwvbhszdacztttuduipdigwihamlbzurmnokvajeifnpva",
                    new[] {MaxLengthExceededError(), OnlyLettersWithOneDashAllowedError()}
                }
            };
        }

        static string LastNameRequiredError(string propertyName = nameof(LastName))
            => $"{propertyName} is required!";

        static string MaxLengthExceededError(string propertyName = nameof(LastName))
            => $"{propertyName} should contain max {LastName.MaxLength} characters!";

        static string LastNameNotLongEnoughError(string propertyName = nameof(LastName))
            => $"{propertyName} should consist of min {LastName.MinLength} characters!";

        static string OnlyLettersWithOneDashAllowedError(string propertyName = nameof(LastName))
            => $"{propertyName} should consist of only letters optionally divided by one '-'!";
    }
}