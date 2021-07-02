using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Auth;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Application.Common.Security;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using MediatR;
using SharedKernel.Infrastructure.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Fundraisers.Commands.RequestFundraiserOpening
{
    [Authorize(Policy = PolicyNames.CanManageFundraiser)]
    public sealed class RequestFundraiserOpeningCommand : IUserCommand, IFundraiserAuthorizationRequest
    {
        public Guid FundraiserId { get; }
        public Guid SchoolId { get; }

        public RequestFundraiserOpeningCommand(Guid fundraiserId, Guid schoolId)
        {
            FundraiserId = fundraiserId;
            SchoolId = schoolId;
        }
    }

    internal sealed class RequestFundraiserOpeningCommandHandler : IRequestHandler<RequestFundraiserOpeningCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IFundraiserRepository _fundraiserRepository;
        private readonly IPaymentGateway _paymentGateway;

        public RequestFundraiserOpeningCommandHandler(
            ISchoolRepository schoolRepository,
            IFundraiserRepository fundraiserRepository,
            IPaymentGateway paymentGateway)
        {
            _schoolRepository = schoolRepository;
            _fundraiserRepository = fundraiserRepository;
            _paymentGateway = paymentGateway;
        }

        public async Task<Result<Unit, RequestError>> Handle(RequestFundraiserOpeningCommand request, CancellationToken token)
        {
            //TODO: return 200 Accepted
            var fundraiserId = new FundraiserId(request.FundraiserId);
            var schoolId = new SchoolId(request.SchoolId);
            
            var fundraiserOrNone = await _fundraiserRepository.GetByIdWithManagerAsync(schoolId, fundraiserId, token);
            if (fundraiserOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(fundraiserId, nameof(Fundraiser));

            var preValidation = fundraiserOrNone.Value.RequestOpening();
            if (preValidation.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(preValidation.Error);

            return Unit.Value;
        }
    }
}