using System.Collections.Generic;
using FluentAssertions;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using Xunit;

namespace SchoolManagement.UnitTests.SchoolAggregateTests.SchoolsTests
{
    public class GroupMembersLimitTests
    {
        [InlineData(30)]
        [InlineData(null)]
        [Theory]
        public void Creates_valid_group_members_limit(int? input)
        {
            var sut = GroupMembersLimit.Create(input);

            sut.IsSuccess.Should().BeTrue();
            (sut.Value == input).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(ErrorData))]
        public void Can_detect_invalid_limit(int inputLimit, string outputError, string propertyName = null)
        {
            var sut = propertyName is null
                ? GroupMembersLimit.Create(inputLimit)
                : GroupMembersLimit.Create(inputLimit, propertyName);
            sut.IsFailure.Should().BeTrue();
            sut.Error.Should().Be(outputError);
        }

        public static List<object[]> ErrorData()
        {
            return new List<object[]>
            {
                new object[] { 0, MinValueExceededError("limit"), "limit"},
                new object[] { -10, MinValueExceededError() },
                new object[] { 1000, MaxValueExceededError("groupLimit"), "groupLimit" },
                new object[] { 501, MaxValueExceededError() }
            };
        }

        static string MinValueExceededError(string propertyName = nameof(GroupMembersLimit))
            => $"{propertyName} must be at least {GroupMembersLimit.MinValue}!";

        static string MaxValueExceededError(string propertyName = nameof(GroupMembersLimit))
            => $"{propertyName} can not be greater than {GroupMembersLimit.MaxValue}!";
    }
}
