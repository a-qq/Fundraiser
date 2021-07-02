using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Members.Commands.RestoreMember
{
    internal sealed class RestoreMemberCommand : IInternalCommand
    {
        public MemberId MemberId { get; }

        public RestoreMemberCommand(MemberId memberId)
        {
            MemberId = memberId;
        }
    }

    internal sealed class RestoreMemberCommandHandler : IRequestHandler<RestoreMemberCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;

        public RestoreMemberCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<Result> Handle(RestoreMemberCommand request, CancellationToken token)
        {
            var memberOrNone = await _memberRepository.GetByIdAsync(request.MemberId, token);
            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member (Id:{request.MemberId}) not found!");

            var result = memberOrNone.Value.Restore();

            return result;
        }
    }
}