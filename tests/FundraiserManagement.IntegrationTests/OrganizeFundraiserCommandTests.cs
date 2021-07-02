using FluentAssertions;
using FundraiserManagement.Application.Fundraisers.Commands.OrganizeFundraiser;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using System;
using System.Threading.Tasks;
using FundraiserManagement.Domain.Common.Models;
using Xunit;
using Range = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.Range;
using Type = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.Type;

namespace FundraiserManagement.IntegrationTests
{
    [Collection("Sequentially")]
    public class OrganizeFundraiserCommandTests : IntegrationTests
    {
        [Fact]
        public async Task Fundraiser_is_organized()
        {
            //Arrange
            var schoolId = new SchoolId(Guid.Parse("7f022ac4-fd95-477d-8d83-e61b9c04b0b0"));
            var manager = await CreateMember(schoolId);
            var command = new OrganizeFundraiserTypeCommand(
                name: "Fundraiser example name",
                description: "Testing description",
                groupId: null,
                schoolId: Guid.Parse("7f022ac4-fd95-477d-8d83-e61b9c04b0b0"),
                range: Range.Intraschool,
                type: Type.Normal,
                goal: 10000m,
                isShared: true,
                managerId: manager.Id);

            //act
            var result = await Execute(command);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Fundraiser example name");
            result.Value.Description.Should().Be("Testing description");
            result.Value.GroupId.Should().BeNull();
            result.Value.SchoolId.Should().Be(schoolId);
            result.Value.Type.Should().Be("Normal");
            result.Value.Range.Should().Be("Intraschool");
            result.Value.State.Should().Be("Preparation");
            result.Value.Goal.Should().Be(10000m);
            result.Value.IsShared.Should().BeTrue();
            result.Value.Id.Should().NotBeEmpty();
            var fundraiserOrNone = await QueryFundraiser(new FundraiserId(result.Value.Id), schoolId);
            fundraiserOrNone.HasValue.Should().BeTrue();
            var fundraiser = fundraiserOrNone.Value;
            fundraiser.Name.Value.Should().Be("Fundraiser example name");
            fundraiser.Description.Value.Should().Be("Testing description");
            fundraiser.GroupId.Should().BeNull();
            fundraiser.SchoolId.Should().Be(schoolId);
            fundraiser.Type.Should().Be(Type.Normal);
            fundraiser.Range.Should().Be(Range.Intraschool);
            fundraiser.State.Should().Be(State.Preparation);
            fundraiser.Goal.Value.Should().Be(10000m);
            fundraiser.Goal.IsShared.Should().BeTrue();
            fundraiser.Manager.Should().Be(manager);
        }

        [Fact]
        public async Task Member_not_found()
        {
            var schoolId = new SchoolId(Guid.Parse("7f022ac4-fd95-477d-8d83-e61b9c04b0b0"));
            var otherSchoolId = new SchoolId(Guid.Parse("90f7a68c-5630-4e3d-b371-9c006bbbaf14"));
            var manager = await CreateMember(schoolId);
            var command = new OrganizeFundraiserTypeCommand(
                name: "Fundraiser example name",
                description: "Testing description",
                groupId: null,
                schoolId: otherSchoolId,
                range: Range.Intraschool,
                type: Type.Normal,
                goal: 10000m,
                isShared: true,
                managerId: manager.Id);

            //act
            var result = await Execute(command);

            //assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("record.not.found");
        }
        public OrganizeFundraiserCommandTests(CompositionRootFixture compositionRoot) : base(compositionRoot)
        {
        }
    }
}