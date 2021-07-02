using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Application.IntegrationEvents.Local;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.FundraiserAggregate.Payments;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Errors;
using IIntegrationEventService = FundraiserManagement.Application.Common.Interfaces.Services.IIntegrationEventService;
using Range = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.Range;

namespace FundraiserManagement.Application.Fundraisers.Commands.ResumeIntraschoolFundraisersManagedByHeadmaster
{
    internal sealed class ResumeIntraschoolFundraiserManagedByPromotedHeadmasterCommand : IInternalCommand
    {
        public MemberId HeadmasterId { get; }
        public SchoolId SchoolId { get; }
        public FundraiserId FundraiserId { get; }
        public string IdempotencyKey { get; }

        public ResumeIntraschoolFundraiserManagedByPromotedHeadmasterCommand(
            MemberId headmasterId, SchoolId schoolId, FundraiserId fundraiserId, string idempotencyKey)
        {
            HeadmasterId = headmasterId;
            SchoolId = schoolId;
            IdempotencyKey = idempotencyKey;
            FundraiserId = fundraiserId;
        }

        internal sealed class ResumeIntraschoolFundraisersManagedByHeadmasterCommandHandler : IRequestHandler<ResumeIntraschoolFundraiserManagedByPromotedHeadmasterCommand, Result>
        {
            private readonly IFundraiserRepository _fundraiserRepository;
            private readonly IMemberRepository _memberRepository;
            private readonly ISchoolRepository _schoolRepository;
            private readonly IDateTime _dateTime;
            private readonly IPaymentGateway _paymentGateway;
            private readonly IIntegrationEventService _integrationEventService;

            public ResumeIntraschoolFundraisersManagedByHeadmasterCommandHandler(
                IFundraiserRepository fundraiserRepository,
                IMemberRepository memberRepository,
                ISchoolRepository schoolRepository,
                IPaymentGateway paymentGateway,
                IDateTime dateTime, IIntegrationEventService integrationEventService)
            {
                _fundraiserRepository = fundraiserRepository;
                _memberRepository = memberRepository;
                _schoolRepository = schoolRepository;
                _paymentGateway = paymentGateway;
                _dateTime = dateTime;
                _integrationEventService = integrationEventService;
            }

            public async Task<Result> Handle(ResumeIntraschoolFundraiserManagedByPromotedHeadmasterCommand request, CancellationToken token)
            {
                var schoolOrNone = await _schoolRepository.GetByIdAsync(request.SchoolId, token);
                if (schoolOrNone.HasNoValue)
                    return Result.Failure($"School with Id: '{request.SchoolId}' not found!");

                var memberOrNone = await _memberRepository.GetByIdAsync(request.HeadmasterId, request.SchoolId, token);
                if (memberOrNone.HasNoValue || memberOrNone.Value.Role != SchoolRole.Headmaster)
                    return Result.Failure($"Headmaster with Id: '{request.HeadmasterId}' not found!");

                var fundraiserOrNone = await _fundraiserRepository.GetByIdWithPaymentsAsync(request.SchoolId, request.FundraiserId, token);
                if (fundraiserOrNone.HasNoValue)
                    return Result.Failure($"Fundraiser with Id: '{request.FundraiserId}' not found!");

                if (fundraiserOrNone.Value.Manager != memberOrNone.Value)
                    return Result.Failure($"Headmaster (Id: '{request.HeadmasterId}' is not a manager of fundraiser (Id: '{request.FundraiserId}'!");


                fundraiserOrNone.Value.CancelAllProcessingPayments(_dateTime.Now);

                var transferAmount = fundraiserOrNone.Value.Participations
                    .Select(p => p.Payments
                        .Where(x => !x.InCash && x.Status == Status.Succeeded)
                        .Sum(x => x.Amount))
                    .Sum();

                Result result = Result.Success();

                if (transferAmount > 0)
                {
                    result = await _paymentGateway.MakeATransfer(fundraiserOrNone.Value.Manager.AccountId, schoolOrNone.Value.AccountId,
                        transferAmount, request.IdempotencyKey, fundraiserOrNone.Value.Name, fundraiserOrNone.Value.Id, token);

                    if (result.IsFailure)
                        return result;
                }

                result = fundraiserOrNone.Value.Resume();
                //if all fundraisers finalized, delete payment account
                var fundraisers =
                  await  _fundraiserRepository.GetByManagerIdAsync(request.SchoolId, request.HeadmasterId, token);

                if (!fundraisers.Any(f => f.Range != Range.Intraschool && f.State != State.ResourcesPayedOut))
                {
                    await _integrationEventService.AddAndSaveEventAsync(
                        new ManagerAccountClearedApplicationEvent(request.SchoolId, request.HeadmasterId));
                }

                return result;
            }
        }
    }
}