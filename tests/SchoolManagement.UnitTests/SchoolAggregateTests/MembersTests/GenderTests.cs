using FluentAssertions;
using SchoolManagement.Domain.SchoolAggregate.Members;
using System.Collections.Generic;
using Xunit;

namespace SchoolManagement.UnitTests.SchoolAggregateTests.MembersTests
{
    public class GenderTests
    {
        [Theory]
        [MemberData(nameof(ValidData))]
        public void Creates_valid_gender(string inputGender, Gender outputGender)
        {
            var sut = Gender.Create(inputGender);

            sut.IsSuccess.Should().BeTrue();
            sut.Value.Should().Be(outputGender);
        }

        public static List<object[]> ValidData()
        {
            return new List<object[]>
            {
                new object[] { "Male", Gender.Male },
                new object[] { "Female", Gender.Female },
                new object[] { " male ", Gender.Male },
                new object[] { " female ", Gender.Female }
            };
        }

        [Theory]
        [MemberData(nameof(ErrorData))]
        public void Can_detect_invalid_gender(string inputGender, string outputError, string propertyName = null)
        {
            var sut = propertyName is null
                ? Gender.Create(inputGender)
                : Gender.Create(inputGender, propertyName);

            sut.IsFailure.Should().BeTrue();
            sut.Error.Should().Be(outputError);
        }

        public static List<object[]> ErrorData()
        {
            return new List<object[]>
            {
                new object[] { null, GenderRequiredError("GenderProp"), "GenderProp" },
                new object[] { string.Empty, GenderRequiredError() },
                new object[] { " ", GenderRequiredError() },
                new object[] {"M ", GenderInvalidError("GenderProp"), "GenderProp"},
                new object[] { "asd", GenderInvalidError()}
            };
        }

        static string GenderRequiredError(string propertyName = nameof(Gender))
            => $"{propertyName} is required!";

        static string GenderInvalidError(string propertyName = nameof(Gender))
            => $"{propertyName} is invalid!";
    }
}