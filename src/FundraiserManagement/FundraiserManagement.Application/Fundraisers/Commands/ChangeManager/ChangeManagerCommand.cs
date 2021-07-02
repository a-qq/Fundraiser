using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Application.Members.Commands.SetPaymentAccount;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.FundraiserAggregate.Payments;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharedKernel.Domain.Constants;

namespace FundraiserManagement.Application.Fundraisers.Commands.ChangeManager
{
    public sealed class ChangeManagerCommand : IInternalCommand
    {
        public SchoolId SchoolId { get; }
        public FundraiserId FundraiserId { get; }
        public MemberId ManagerId { get; }
        public string IdempotencyKey { get; }

        public ChangeManagerCommand(
            SchoolId schoolId, FundraiserId fundraiserId,
            MemberId managerId, string idempotencyKey)
        {
            SchoolId = schoolId;
            FundraiserId = fundraiserId;
            ManagerId = managerId;
            IdempotencyKey = idempotencyKey;
        }
    }

    internal sealed class
        ChangeManagerCommandHandler : IRequestHandler<ChangeManagerCommand, Result>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISender _mediator;
        private readonly IFundraiserRepository _fundraiserRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IPaymentGateway _paymentGateway;

        public ChangeManagerCommandHandler(
            IFundraiserRepository fundraiserRepository,
            ISchoolRepository schoolRepository,
            IMemberRepository memberRepository,
            IPaymentGateway paymentGateway,
            ISender mediator)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository)); 
            _mediator = Guard.Against.Null(mediator, nameof(mediator)); 
            _paymentGateway = Guard.Against.Null(paymentGateway, nameof(paymentGateway));
            _fundraiserRepository = Guard.Against.Null(fundraiserRepository, nameof(fundraiserRepository));
            _memberRepository = Guard.Against.Null(memberRepository, nameof(memberRepository));
        }

        public async Task<Result> Handle(ChangeManagerCommand request, CancellationToken token)
        {
            var schoolOrNone = await _schoolRepository.GetByIdAsync(request.SchoolId, token);
            if(schoolOrNone.HasNoValue)
                return Result.Failure($"School with Id: '{request.SchoolId}' not found!");

            var fundraiserOrNone = await _fundraiserRepository
                .GetByIdWithPaymentsAsync(request.SchoolId, request.FundraiserId, token);

            if (fundraiserOrNone.HasNoValue)
                return Result.Failure($"Fundraiser with Id '{request.FundraiserId}' not found!");

            var memberOrNone = await _memberRepository
                .GetByIdAsync(request.ManagerId, request.SchoolId, token);

            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member with Id '{request.ManagerId}' not found!");

            var preValidation = fundraiserOrNone.Value.CanChangeManagerTo(memberOrNone.Value);
            if (preValidation.IsFailure)
                return Result.Failure(string.Join("\n", preValidation.Error.Errors));

            Result result;

            var newAccountSet = false;
            if (memberOrNone.Value.CanSetAccountId().IsSuccess)
            {
                result = await _mediator.Send(new SetPaymentAccountCommand(
                    memberOrNone.Value.Id, memberOrNone.Value.SchoolId), token);

                if (result.IsFailure)
                    return result;

                newAccountSet = true;
            }

            if (fundraiserOrNone.Value.Manager.Id != memberOrNone.Value.Id)
            {
                var amount = fundraiserOrNone.Value.Participations
                    .Select(p => p.Payments
                        .Where(pp => !pp.InCash && pp.Status == Status.Succeeded)
                        .Sum(pp => pp.Amount))
                    .Sum(x => x);

                if (amount > 0)
                {
                    result = await _paymentGateway.MakeATransfer(fundraiserOrNone.Value.Manager.AccountId,
                        memberOrNone.Value.AccountId, amount, request.IdempotencyKey, fundraiserOrNone.Value.Name,
                        fundraiserOrNone.Value.Id, token);

                    if (result.IsFailure)
                    {
                        if(newAccountSet)
                            result = Result.Combine(result, await _paymentGateway.DeleteAccount(
                            memberOrNone.Value.AccountId, request.IdempotencyKey, token));

                        return result;
                    }
                }
            }

            result = fundraiserOrNone.Value.ChangeManager(memberOrNone.Value);
            return result;
        }
    }
}