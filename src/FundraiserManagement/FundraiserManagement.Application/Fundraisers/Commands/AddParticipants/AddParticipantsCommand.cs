using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Common.Security;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using SharedKernel.Domain.Errors;
using SharedKernel.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Auth;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;

namespace FundraiserManagement.Application.Fundraisers.Commands.AddParticipants
{
    [Authorize(Policy = PolicyNames.CanManageFundraiser)]
    public sealed class AddParticipantsCommand : IUserCommand, IFundraiserAuthorizationRequest
    {
        public Guid FundraiserId { get; }
        public Guid SchoolId { get; }
        public IEnumerable<Guid> ParticipantsIds { get; }

        public AddParticipantsCommand(Guid fundraiserId, Guid schoolId, IEnumerable<Guid> participantsIds)
        {
            FundraiserId = fundraiserId;
            ParticipantsIds = participantsIds;
            SchoolId = schoolId;
        }
    }

    internal sealed class AddParticipantsCommandHandler : IRequestHandler<AddParticipantsCommand, Result<Unit, RequestError>>
    {
        private readonly IFundraiserRepository _fundraiserRepository;
        private readonly IMemberRepository _memberRepository;

        public AddParticipantsCommandHandler(IFundraiserRepository fundraiserRepository, IMemberRepository memberRepository)
        {
            _fundraiserRepository = fundraiserRepository;
            _memberRepository = memberRepository;
        }

        public async Task<Result<Unit, RequestError>> Handle(AddParticipantsCommand request, CancellationToken token)
        {
            var fundraiserId = new FundraiserId(Guard.Against.Default(request.FundraiserId, nameof(request.FundraiserId)));
            var schoolId = new SchoolId(Guard.Against.Default(request.SchoolId, nameof(request.SchoolId)));
            var participantsIds = request.ParticipantsIds
                .Select(id => new MemberId(Guard.Against.Default(id, nameof(request.ParticipantsIds))))
                .ToHashSet();

            var fundraiserOrNone = await _fundraiserRepository.GetByIdWithParticipantsAsync(schoolId, fundraiserId, token);
            if (fundraiserOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(fundraiserId, nameof(Fundraiser));

            var participantsToAdd = await _memberRepository.GetByIdsAsync(participantsIds, schoolId, token);
            if (participantsIds.Count != participantsToAdd.Count)
            {
                var missingMembersIds
                    = participantsIds.Except(participantsToAdd.Select(m => m.Id));

                return SharedRequestError.General.NotFound(missingMembersIds.Select(id => id.Value), nameof(Member));
            }

            var result = Result.Success<bool, Error>(true);

            foreach (var participant in participantsToAdd)
                result = Result.Combine(fundraiserOrNone.Value.AddParticipant(participant), result);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            return Unit.Value;
        }
    }
}