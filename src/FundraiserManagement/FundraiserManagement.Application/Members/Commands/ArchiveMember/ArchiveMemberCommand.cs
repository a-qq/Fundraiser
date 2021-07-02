using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Members.Commands.ArchiveMember
{
    internal sealed class ArchiveMemberCommand : IInternalCommand
    {
        public MemberId MemberId { get; }

        public ArchiveMemberCommand(MemberId memberId)
        {
            MemberId = memberId;
        }
    }
    internal sealed class ArchiveMemberCommandHandler : IRequestHandler<ArchiveMemberCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;

        public ArchiveMemberCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<Result> Handle(ArchiveMemberCommand request, CancellationToken token)
        {
            var memberOrNone = await _memberRepository.GetByIdAsync(request.MemberId, token);
            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member (Id:{request.MemberId}) not found!");

            var result = memberOrNone.Value.Archive();

            return result;
        }
    }
}