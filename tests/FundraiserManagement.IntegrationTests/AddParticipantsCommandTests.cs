using FluentAssertions;
using FundraiserManagement.Application.Fundraisers.Commands.AddParticipants;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel.Domain.Constants;
using SharedKernel.Domain.Extensions;
using Xunit;
using FMD = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;

namespace FundraiserManagement.IntegrationTests
{
    [Collection("Sequentially")]
    public sealed class AddParticipantsCommandTests : IntegrationTests
    {
        public AddParticipantsCommandTests(CompositionRootFixture compositionRoot)
            : base(compositionRoot) { }

        [Fact]
        public async Task Participants_are_added()
        {
            var groupId = new GroupId(Guid.Parse("7f022ac4-fd95-477d-8d83-e61b9c04b0b0"));
            var schoolId = new SchoolId(Guid.Parse("301a67ea-1bbd-435a-85a1-3786601e1951"));
            var manager = await CreateMember(schoolId);
            var fundraiser = await CreateFundraiser(manager, schoolId);
            var participants = new List<Member>
            {
                await CreateMember(schoolId, SchoolRole.Student, groupId, isTreasurer: true),
                await CreateMember(schoolId, SchoolRole.Student),
                await CreateMember(schoolId, SchoolRole.Teacher, groupId, isFormTutor: true),
                await CreateMember(schoolId, SchoolRole.Teacher),
                manager
            };

            var command = new AddParticipantsCommand(fundraiser.Id, schoolId,
                participants.Select(s => s.Id.Value));

            var result = await Execute(command);

            result.IsSuccess.Should().BeTrue();
            var fundraiserFromDb = (await QueryFundraiser(fundraiser.Id, schoolId)).Value;
            fundraiserFromDb.Participations.Should().HaveCount(5);
            fundraiserFromDb.Participations.Select(m => m.Participant)
                .Should().BeEquivalentTo(participants);
        }

        [Fact]
        public async Task Teacher_trying_to_modify_teacher_day_fundraiser_gets_forbidden_access()
        {
            var groupId = new GroupId(Guid.Parse("7f022ac4-fd95-477d-8d83-e61b9c04b0b0"));
            var schoolId = new SchoolId(Guid.Parse("301a67ea-1bbd-435a-85a1-3786601e1951"));
            var manager = await CreateMember(schoolId, SchoolRole.Student, groupId, isTreasurer: true);
            var fundraiser = await CreateFundraiser(manager, schoolId, groupId, FMD.Range.Intragroup, FMD.Type.TeacherDay);
            //var command = new 
        }


        private class MemberDto
        {
            public MemberId MemberId { get; }
            public SchoolId SchoolId { get; }
            public Role Role { get; }
            public GroupId? GroupId { get; }

            public MemberDto(string memberId, string schoolId, Role role, string groupId = null)
            {
                MemberId = new MemberId(Guid.Parse(memberId));
                SchoolId = new SchoolId(Guid.Parse(schoolId));
                Role = role;
                GroupId = null;
                if(!string.IsNullOrWhiteSpace(groupId))
                    GroupId = new GroupId(Guid.Parse(groupId));
            }
            
        }
        //notauthorized, if schoolId doest match change to not authorized
        //not found multiple members + double the input
        //not found fundraiser
    }

}