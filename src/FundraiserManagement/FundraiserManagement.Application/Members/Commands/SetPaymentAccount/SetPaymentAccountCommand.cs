using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;

namespace FundraiserManagement.Application.Members.Commands.SetPaymentAccount
{
    internal sealed class SetPaymentAccountCommand : IInternalCommand
    {
        public MemberId MemberId { get; }
        public SchoolId SchoolId { get; }

        public SetPaymentAccountCommand(MemberId memberId, SchoolId schoolId)
        {
            MemberId = memberId;
            SchoolId = schoolId;
        }
    }

    internal sealed class SetPaymentAccountHandler : IRequestHandler<SetPaymentAccountCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IPaymentGateway _paymentGateway;

        public SetPaymentAccountHandler(
            IMemberRepository memberRepository,
            ISchoolRepository schoolRepository,
            IPaymentGateway paymentGateway)
        {
            _memberRepository = memberRepository;
            _schoolRepository = schoolRepository;
            _paymentGateway = paymentGateway;
        }
        public async Task<Result> Handle(SetPaymentAccountCommand request, CancellationToken token)
        {
            if (!await _schoolRepository.ExistByIdAsync(request.SchoolId, token))
                return Result.Failure($"School with Id: '{request.SchoolId}' not found!");


            var memberOrNone = await _memberRepository
                .GetByIdAsync(request.MemberId, request.SchoolId, token);
            
            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member with Id '{request.MemberId}' not found!");

            var accountIdOrError = await _paymentGateway.CreateAccountAsync(
                memberOrNone.Value.Id.ToString(), token, memberOrNone.Value.Email);

            if (accountIdOrError.IsFailure)
                return accountIdOrError;

            var result = memberOrNone.Value.SetAccountId(accountIdOrError.Value);

            return result;
        }
    }
}