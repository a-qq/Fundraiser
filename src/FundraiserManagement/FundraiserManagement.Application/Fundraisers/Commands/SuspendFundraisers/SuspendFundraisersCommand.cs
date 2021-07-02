using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Fundraisers.Commands.SuspendFundraisers
{
    internal sealed class SuspendFundraisersCommand : IInternalCommand
    {
        public MemberId ManagerId { get; }
        public SchoolId SchoolId { get; }
        public bool WasManagerHeadmaster { get; }

        public SuspendFundraisersCommand(
            MemberId managerId, SchoolId schoolId, bool wasManagerHeadmaster)
        {
            ManagerId = managerId;
            SchoolId = schoolId;
            WasManagerHeadmaster = wasManagerHeadmaster;
        }
    }

    internal sealed class SuspendFundraisersCommandHandler : IRequestHandler<SuspendFundraisersCommand, Result>
    {
        private readonly IFundraiserRepository _fundraiserRepository;

        public SuspendFundraisersCommandHandler(IFundraiserRepository fundraiserRepository)
        {
            _fundraiserRepository = fundraiserRepository;
        }

        public async Task<Result> Handle(SuspendFundraisersCommand request, CancellationToken token)
        {
            var fundraisers = await _fundraiserRepository.GetByManagerIdAsync(request.SchoolId, request.ManagerId, token);

            if (!fundraisers.Any())
                return Result.Success();

            foreach (var fundraiser in fundraisers
                .Where(f => f.State == State.Open || f.State == State.Stopped))
            {
                fundraiser.Suspend(request.WasManagerHeadmaster);
            }

            return Result.Success();
        }
    }
}
