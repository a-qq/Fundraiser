using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Members.Commands.PromoteHeadmaster
{
    internal sealed class PromoteHeadmasterCommand : IInternalCommand
    {
        public MemberId MemberId { get; }

        public PromoteHeadmasterCommand(MemberId memberId)
        {
            MemberId = memberId;
        }
    }

    internal sealed class PromoteHeadmasterCommandHandler : IRequestHandler<PromoteHeadmasterCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;

        public PromoteHeadmasterCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }
        public async Task<Result> Handle(PromoteHeadmasterCommand request, CancellationToken token)
        {
            var memberOrNone = await _memberRepository.GetByIdAsync(request.MemberId, token);
            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member (Id:{request.MemberId}) not found!");

            var result = memberOrNone.Value.PromoteToHeadmaster();

            return result;
        }
    }
}