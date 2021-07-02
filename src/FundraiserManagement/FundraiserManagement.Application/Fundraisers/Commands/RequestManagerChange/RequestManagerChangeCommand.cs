using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Auth;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Application.Common.Security;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using SharedKernel.Infrastructure.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Fundraisers.Commands.RequestManagerChange
{
    [Authorize(Policy = PolicyNames.CanManageFundraiser)]
    public sealed class RequestManagerChangeCommand : IUserCommand, IFundraiserAuthorizationRequest
    {
        public Guid SchoolId { get; }
        public Guid FundraiserId { get; }
        public Guid ManagerId { get; }

        public RequestManagerChangeCommand(Guid schoolId, Guid fundraiserId, Guid managerId)
        {
            SchoolId = schoolId;
            FundraiserId = fundraiserId;
            ManagerId = managerId;
        }

        internal sealed class
            RequestManagerChangeCommandHandler : IRequestHandler<RequestManagerChangeCommand, Result<Unit, RequestError>
            >
        {
            private readonly IFundraiserRepository _fundraiserRepository;
            private readonly IMemberRepository _memberRepository;

            public RequestManagerChangeCommandHandler(IFundraiserRepository fundraiserRepository,
                IMemberRepository memberRepository)
            {
                _fundraiserRepository = Guard.Against.Null(fundraiserRepository, nameof(fundraiserRepository));
                _memberRepository = Guard.Against.Null(memberRepository, nameof(memberRepository));
            }

            public async Task<Result<Unit, RequestError>> Handle(RequestManagerChangeCommand request,
                CancellationToken token)
            {
                var fundraiserId = new FundraiserId(request.FundraiserId);
                var managerId = new MemberId(request.ManagerId);
                var schoolId = new SchoolId(request.SchoolId);

                var fundraiserOrNone =
                    await _fundraiserRepository.GetByIdWithManagerAsync(schoolId, fundraiserId, token);
                if (fundraiserOrNone.HasNoValue)
                    return SharedRequestError.General.NotFound(fundraiserId, nameof(Fundraiser));

                if (fundraiserOrNone.Value.Manager?.Id != managerId)
                {
                    var managerOrNone = await _memberRepository.GetByIdAsync(managerId, schoolId, token);
                    if (managerOrNone.HasNoValue)
                        return SharedRequestError.General.NotFound(managerId, nameof(Member));

                    var result = fundraiserOrNone.Value.RequestManagerChange(managerOrNone.Value);
                    if (result.IsFailure)
                        return SharedRequestError.General.BusinessRuleViolation(result.Error);
                }

                return Unit.Value;
            }
        }
    }
}