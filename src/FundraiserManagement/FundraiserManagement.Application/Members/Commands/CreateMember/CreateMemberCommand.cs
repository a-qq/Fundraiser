using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using SharedKernel.Domain.Constants;
using SharedKernel.Domain.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;
using Gender = SharedKernel.Domain.Constants.Gender;

namespace FundraiserManagement.Application.Members.Commands.CreateMember
{
    internal sealed class CreateMemberCommand : IInternalCommand
    {
        public MemberId MemberId { get; }
        public SchoolId SchoolId { get; }
        public SchoolRole Role { get; }
        public Gender Gender { get; }
        public string Email { get; }
        public GroupId? GroupId { get; }
        public bool IsFormTutor { get; }
        public bool IsTreasurer { get; }

        public CreateMemberCommand(MemberId memberId, SchoolId schoolId, SchoolRole role,
            Gender gender, string email, GroupId? groupId, bool isFormTutor, bool isTreasurer)
        {
            MemberId = memberId;
            SchoolId = schoolId;
            Role = role;
            Gender = gender;
            Email = email;
            GroupId = groupId;
            IsFormTutor = isFormTutor;
            IsTreasurer = isTreasurer;
        }
    }

    internal sealed class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;

        public CreateMemberCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public Task<Result> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email).Value;
            var member = new Member(request.MemberId, request.SchoolId, request.Gender, request.Role, email);

            var result = Result.Success();

            if (request.IsFormTutor)
            {
                result = member.PromoteToFormTutor(request.GroupId ?? throw new ArgumentNullException(nameof(request.GroupId)));
            }

            else if(request.GroupId.HasValue)
            {
                result = Result.Combine(result, member.EnrollToGroup(request.GroupId.Value));
                if (request.IsTreasurer)
                    result = Result.Combine(result, member.PromoteToTreasurer());
            }

            if(result.IsSuccess)
                _memberRepository.Add(member);

            return Task.FromResult(result);
        }
    }
}
