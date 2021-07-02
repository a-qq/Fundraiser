using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.SchoolAggregate;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Schools.CreateSchoolWithPaymentsAccount
{
    internal sealed class CreateSchoolWithPaymentsAccountCommand : IInternalCommand
    {
        public SchoolId SchoolId { get; }

        public CreateSchoolWithPaymentsAccountCommand(SchoolId schoolId)
        {
            SchoolId = schoolId;
        }    
    }

    internal sealed class CreateSchoolWithPaymentsAccountCommandHandler : IRequestHandler<CreateSchoolWithPaymentsAccountCommand, Result>
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly ISchoolRepository _schoolRepository;

        public CreateSchoolWithPaymentsAccountCommandHandler(
            IPaymentGateway paymentGateway,
            ISchoolRepository schoolRepository)
        {
            _paymentGateway = paymentGateway;
            _schoolRepository = schoolRepository;
        }

        public async Task<Result> Handle(CreateSchoolWithPaymentsAccountCommand request, CancellationToken token)
        {
            if ((await _schoolRepository.GetByIdAsync(request.SchoolId, token)).HasValue)
                return Result.Failure($"School with id {request.SchoolId} already exists!");

            var result = await _paymentGateway.CreateAccountAsync(request.SchoolId.ToString(), token);
            if (result.IsFailure)
                return result;

            _schoolRepository.Add(new School(request.SchoolId, result.Value));

            return Result.Success();
        }
    }
}