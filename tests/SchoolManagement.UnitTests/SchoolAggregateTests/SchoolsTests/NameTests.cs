using System.Collections.Generic;
using FluentAssertions;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using Xunit;

namespace SchoolManagement.UnitTests.SchoolAggregateTests.SchoolsTests
{
    public class NameTests
    {
        [Fact]
        public void Creates_valid_name()
        {
            const string inputName = " valid school name - 123 ";

            var sut = Name.Create(inputName);

            sut.IsSuccess.Should().BeTrue();

            sut.Value.Value.Should().Be("Valid school name - 123");
        }

        [Theory]
        [MemberData(nameof(ErrorData))]
        public void Can_detect_invalid_name(string inputName, string outputError, string propertyName = null)
        {
            var sut = propertyName is null
                ? Name.Create(inputName)
                : Name.Create(inputName, propertyName);

            sut.IsFailure.Should().BeTrue();
            sut.Error.Should().Be(outputError);
        }

        public static List<object[]> ErrorData()
        {
            return new List<object[]>
            {
                new object[] { null, NameRequiredError("SchoolName"), "SchoolName"},
                new object[] { string.Empty, NameRequiredError() },
                new object[] { " ", NameRequiredError() },
                new object[]
                {
                    "auzyydezcuvsdrsimbgxwiwkxrgninrxjfcwkodbvdkovyfbcrholppuqrhyqqdxklcjbsdugeozuhrhsebqckdnigpduhlzqyemzrvnznjkgdnquvqntmmvnzcmdsruoxiotskkshdgfqheilnpfwkvhyaueeipsuzmcmgolouxnjatihfqmrokohyvovbxbnvrctyoxtzyoobxjvwzfxqffwypgrztinyzofiuhwvjisknwajpfjykbtaauzyydezcuvsdrsimbgxwiwkxrgninrxjfcwkodbvdkovyfbcrholppuqrhyqqdxklcjbsdugeozuhrhsebqckdnigpduhlzqyemzrvnznjkgdnquvqntmmvnzcmdsruoxiotskkshdgfqheilnpfwkvhyaueeipsuzmcmgolouxnjatihfqmrokohyvovbxbnvrctyoxtzyoobxjvwzfxqffwypgrztinyzofiuhwvjisknwajpfjykbt",
                    MaxLengthExceededError("SchoolName"),
                    "SchoolName"
                },
                new object[] { "testNam ", NameNotLongEnoughError("SchoolName"), "SchoolName"},
            };
        }

        static string NameRequiredError(string propertyName = nameof(Name))
            => $"{propertyName} is required!";

        static string MaxLengthExceededError(string propertyName = nameof(Name))
            => $"{propertyName} should contain max {Name.MaxLength} characters!";

        static string NameNotLongEnoughError(string propertyName = nameof(Name))
            => $"{propertyName} should contain min {Name.MinLength} characters!";
    }
}