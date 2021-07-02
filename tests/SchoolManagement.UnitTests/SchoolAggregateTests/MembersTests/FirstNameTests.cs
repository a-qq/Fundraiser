using FluentAssertions;
using SchoolManagement.Domain.SchoolAggregate.Members;
using System.Collections.Generic;
using Xunit;

namespace SchoolManagement.UnitTests.SchoolAggregateTests.MembersTests
{
    public class FirstNameTests
    {
        [Fact]
        public void Creates_valid_first_name()
        {
            const string inputFirstName = " aL ";

            var sut = FirstName.Create(inputFirstName);

            sut.IsSuccess.Should().BeTrue();

            sut.Value.Value.Should().Be("Al");
        }

        [Theory]
        [MemberData(nameof(ErrorData))]
        public void Can_detect_invalid_first_name(string inputFirstName, IEnumerable<string> outputErrors, string propertyName = null)
        {
            var sut = propertyName is null
                ? FirstName.Create(inputFirstName)
                : FirstName.Create(inputFirstName, propertyName);

            sut.IsFailure.Should().BeTrue();
            sut.Error.Errors.Should().BeEquivalentTo(outputErrors);
        }

        public static List<object[]> ErrorData()
        {
            return new List<object[]>
            {
                new object[] { null, new[] {FirstNameRequiredError("Name")}, "Name" },
                new object[] { string.Empty, new[] {FirstNameRequiredError()} },
                new object[] { " ", new[] {FirstNameRequiredError()} },
                new object[]
                {
                    "rqpfvbxqdscaertcfbfiigjtrnpvonyjtppvemofuzqezatmpxaffqaaeirirrxqaginpkmhsvvautdpholtefcxeftyviebpbldeycqnscqaqzodimslaiumtpgueezwofeegeckfuecxjuggtoxgkywiadwvbhszdacztttuduipdigwihamlbzurmnokvajeifnpva",
                    new[]{MaxLengthExceededError("Firstname")},
                    "Firstname"
                },
                new object[] { " a ", new[] {FirstNameNotLongEnoughError("Firstname")}, "Firstname" },
                new object[] { " 1", new[] { OnlyLettersAllowedError(), FirstNameNotLongEnoughError() } },
                new object[] { "j4cek ", new[] {OnlyLettersAllowedError("TestName")}, "TestName"},
                new object[] { " jack-enrique ", new[] {OnlyLettersAllowedError()}},
                new object[] 
                {
                    "rqpfvbxqdscaertc1bfiigjtrnpvonyjtppvemofuzqezatmpxaffqaaeirirrxqaginpkmhsvvautdpholtefcxeftyviebpbldeycqnscqaqzodimslaiumtpgueezwofeegeckfuecxjuggtoxgkywiadwvbhszdacztttuduipdigwihamlbzurmnokvajeifnpva",
                    new[] {MaxLengthExceededError(), OnlyLettersAllowedError()}
                }
            };
        }

        static string FirstNameRequiredError(string propertyName = nameof(FirstName))
            => $"{propertyName} is required!";

        static string MaxLengthExceededError(string propertyName = nameof(FirstName))
            => $"{propertyName} should contain max {FirstName.MaxLength} characters!";

        static string FirstNameNotLongEnoughError(string propertyName = nameof(FirstName))
            => $"{propertyName} should consist of min {FirstName.MinLength} characters!";

        static string OnlyLettersAllowedError(string propertyName = nameof(FirstName))
            => $"{propertyName} should consist of only letters!";
    }
}
