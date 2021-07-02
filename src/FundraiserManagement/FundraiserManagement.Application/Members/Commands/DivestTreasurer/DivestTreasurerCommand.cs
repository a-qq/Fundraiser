using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Members.Commands.DivestTreasurer
{
    internal sealed class DivestTreasurerCommand : IInternalCommand
    {
        public MemberId MemberId { get; }

        public DivestTreasurerCommand(MemberId memberId)
        {
            MemberId = memberId;
        }
    }

    internal sealed class DivestTreasurerCommandHandler : IRequestHandler<DivestTreasurerCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;

        public DivestTreasurerCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<Result> Handle(DivestTreasurerCommand request, CancellationToken token)
        {
            var memberOrNone = await _memberRepository.GetByIdAsync(request.MemberId, token);
            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member (Id:{request.MemberId}) not found!");

            //check for any fundraiser with this treasurer, make it suspended

            var result = memberOrNone.Value.DivestTreasurer();

            return result;
        }
    }
}