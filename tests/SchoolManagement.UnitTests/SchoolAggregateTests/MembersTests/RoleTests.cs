using FluentAssertions;
using SchoolManagement.Domain.SchoolAggregate.Members;
using System.Collections.Generic;
using Xunit;

namespace SchoolManagement.UnitTests.SchoolAggregateTests.MembersTests
{
    public class RoleTests
    {
        [Theory]
        [MemberData(nameof(ValidData))]
        public void Creates_valid_role(string inputRole, Role outputRole)
        {
            var sut = Role.Create(inputRole);

            sut.IsSuccess.Should().BeTrue();
            sut.Value.Should().Be(outputRole);
        }

        public static List<object[]> ValidData()
        {
            return new List<object[]>
            {
                new object[] { "Teacher", Role.Teacher },
                new object[] { " headmaster ", Role.Headmaster },
                new object[] { "student ", Role.Student }
            };
        }

        [Theory]
        [MemberData(nameof(ErrorData))]
        public void Can_detect_invalid_role(string inputRole, string outputRole, string propertyName = null)
        {
            var sut = propertyName is null
                ? Role.Create(inputRole)
                : Role.Create(inputRole, propertyName);

            sut.IsFailure.Should().BeTrue();
            sut.Error.Should().Be(outputRole);
        }

        public static List<object[]> ErrorData()
        {
            return new List<object[]>
            {
                new object[] { null, RoleRequiredError("RoleProp"), "RoleProp" },
                new object[] { string.Empty, RoleRequiredError() },
                new object[] { " ", RoleRequiredError() },
                new object[] {"students ", RoleInvalidError("RoleProp"), "RoleProp" },
                new object[] { "asd", RoleInvalidError()}
            };
        }

        static string RoleRequiredError(string propertyName = nameof(Role))
            => $"{propertyName} is required!";

        static string RoleInvalidError(string propertyName = nameof(Role))
            => $"{propertyName} is invalid!";
    }
}
