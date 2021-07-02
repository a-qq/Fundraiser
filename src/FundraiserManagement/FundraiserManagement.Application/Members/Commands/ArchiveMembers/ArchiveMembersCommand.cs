using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Members.Commands.ArchiveMember;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Members.Commands.ArchiveMembers
{
    internal sealed class ArchiveMembersCommand : IInternalCommand
    {
        public IReadOnlyCollection<MemberId> MemberIds { get; }

        public ArchiveMembersCommand(IReadOnlyCollection<MemberId> memberIds)
        {
            MemberIds = memberIds;
        }
    }
    internal sealed class ArchiveMembersCommandHandler : IRequestHandler<ArchiveMembersCommand, Result>
    {
        private readonly ISender _mediator;

        public ArchiveMembersCommandHandler(ISender mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Handle(ArchiveMembersCommand request, CancellationToken token)
        {
            var results = await Task.WhenAll(request.MemberIds.Select(id => 
                _mediator.Send(new ArchiveMemberCommand(id), token)));

            return Result.Combine(results);
        }
    }
}