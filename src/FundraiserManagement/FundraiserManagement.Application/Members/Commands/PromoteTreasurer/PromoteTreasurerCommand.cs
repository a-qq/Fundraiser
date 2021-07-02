using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Members.Commands.PromoteTreasurer
{
    internal sealed class PromoteTreasurerCommand : IInternalCommand
    {
        public MemberId MemberId { get; }

        public PromoteTreasurerCommand(MemberId memberId)
        {
            MemberId = memberId;
        }
    }

    internal sealed class PromoteTreasurerCommandHandler : IRequestHandler<PromoteTreasurerCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;

        public PromoteTreasurerCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }
        public async Task<Result> Handle(PromoteTreasurerCommand request, CancellationToken token)
        {
            var memberOrNone = await _memberRepository.GetByIdAsync(request.MemberId, token);
            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member (Id:{request.MemberId}) not found!");

            var result = memberOrNone.Value.PromoteToTreasurer();

            return result;
        }
    }
}