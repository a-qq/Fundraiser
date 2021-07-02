using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Members.Commands.DivestTreasurer;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Members.Commands.DivestTreasurers
{
    internal sealed class DivestTreasurersCommand : IInternalCommand
    {
        public IReadOnlyCollection<MemberId> TreasurerIds { get; }

        public DivestTreasurersCommand(IReadOnlyCollection<MemberId> treasurerIds)
        {
            TreasurerIds = treasurerIds;
        }
    }

    internal sealed class DivestTreasurersCommandHandler : IRequestHandler<DivestTreasurersCommand, Result>
    {
        private readonly ISender _mediator;

        public DivestTreasurersCommandHandler(ISender mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Handle(DivestTreasurersCommand request, CancellationToken token)
        {
            var results = await Task.WhenAll(request.TreasurerIds.Select(
                x => _mediator.Send(new DivestTreasurerCommand(x), token)));

            return Result.Combine(results);
        }
    }
}