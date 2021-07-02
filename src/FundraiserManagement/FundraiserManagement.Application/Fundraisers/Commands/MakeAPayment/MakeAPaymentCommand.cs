using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.FundraiserAggregate.Payments;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Concretes.Services;
using SharedKernel.Infrastructure.Errors;

namespace FundraiserManagement.Application.Fundraisers.Commands.MakeAPayment
{
    public sealed class MakeAPaymentCommand : IUserCommand<PaymentId>
    {
        public Guid SchoolId { get; }
        public Guid FundraiserId { get; }
        public Guid ParticipantId { get; }
        public decimal Amount { get; }
        public bool InCash { get; }

        public MakeAPaymentCommand(Guid schoolId, Guid fundraiserId, Guid participantId, decimal amount, bool inCash)
        {
            SchoolId = schoolId;
            FundraiserId = fundraiserId;
            Amount = amount;
            InCash = inCash;
            ParticipantId = participantId;
        }
    }

    internal sealed class MakeAPaymentCommandHandler : IRequestHandler<MakeAPaymentCommand, Result<PaymentId, RequestError>>
    {
        private readonly IFundraiserRepository _fundraiserRepository;
        private readonly IDateTime _dateTime;

        public MakeAPaymentCommandHandler(IFundraiserRepository fundraiserRepository, IDateTime dateTime)
        {
            _fundraiserRepository = Guard.Against.Null(fundraiserRepository, nameof(fundraiserRepository));
            _dateTime = Guard.Against.Null(dateTime, nameof(dateTime)); 
        }

        public async Task<Result<PaymentId, RequestError>> Handle(MakeAPaymentCommand request, CancellationToken token)
        {
            var amount = Amount.Create(request.Amount).Value;
            var schoolId = new SchoolId(Guard.Against.Default(request.SchoolId, nameof(request.SchoolId)));
            var fundraiserId = new FundraiserId(Guard.Against.Default(request.FundraiserId, nameof(request.FundraiserId)));
            var participantId = new MemberId(Guard.Against.Default(request.ParticipantId, nameof(request.ParticipantId)));

            var fundraiserOrNone =
                await _fundraiserRepository.GetByIdWithParticipantsAsync(schoolId, fundraiserId, token);

            if (fundraiserOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(fundraiserId, nameof(Fundraiser));

            var participation = fundraiserOrNone.Value.Participations
                .SingleOrDefault(p => p.Participant.Id != participantId);

            if (participation is null)
                return SharedRequestError.General.NotFound(participantId, "Participant");

            var result = fundraiserOrNone.Value.SavePayment(participation, amount, request.InCash, _dateTime.Now);
            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            return result.Value;
        }
    }
}
