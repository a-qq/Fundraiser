using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;

namespace FundraiserManagement.Application.Members.Commands.PromoteFormTutor
{
    internal sealed class PromoteFormTutorCommand : IInternalCommand
    {
        public MemberId MemberId { get; }
        public GroupId GroupId { get; }

        public PromoteFormTutorCommand(MemberId memberId, GroupId groupId)
        {
            MemberId = memberId;
            GroupId = groupId;
        }
    }

    internal sealed class PromoteFormTutorCommandHandler : IRequestHandler<PromoteFormTutorCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;

        public PromoteFormTutorCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }
        public async Task<Result> Handle(PromoteFormTutorCommand request, CancellationToken token)
        {
            var memberOrNone = await _memberRepository.GetByIdAsync(request.MemberId, token);
            if(memberOrNone.HasNoValue)
                return Result.Failure($"Member (Id:{request.MemberId}) not found!");

            var result = memberOrNone.Value.PromoteToFormTutor(request.GroupId);

            return result;
        }
    }
}
