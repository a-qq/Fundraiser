using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Application.Members.Commands.SetPaymentAccount;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Fundraisers.Commands.OpenFundraiser
{
    internal sealed class OpenFundraiserCommand : IInternalCommand
    {
        public FundraiserId FundraiserId { get; }
        public SchoolId SchoolId { get; }

        public OpenFundraiserCommand(FundraiserId fundraiserId, SchoolId schoolId)
        {
            FundraiserId = fundraiserId;
            SchoolId = schoolId;
        }
    }

    internal sealed class OpenFundraiserCommandHandler : IRequestHandler<OpenFundraiserCommand, Result>
    {
        private readonly IFundraiserRepository _fundraiserRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISender _mediator;

        public OpenFundraiserCommandHandler(
            IFundraiserRepository fundraiserRepository,
            ISender mediator,
            ISchoolRepository schoolRepository)
        {
            _fundraiserRepository = fundraiserRepository;
            _mediator = mediator;
            _schoolRepository = schoolRepository;
        }


        public async Task<Result> Handle(OpenFundraiserCommand request, CancellationToken token)
        {
            if(!await _schoolRepository.ExistByIdAsync(request.SchoolId, token))
                return Result.Failure($"School with Id: '{request.SchoolId}' not found!");

            var fundraiserOrNone = await _fundraiserRepository
                .GetByIdWithManagerAsync(request.SchoolId, request.FundraiserId, token);

            if (fundraiserOrNone.HasNoValue)
                return Result.Failure($"Fundraiser with Id '{request.FundraiserId}' not found!");

            Result result;

            if (fundraiserOrNone.Value.Manager.CanSetAccountId().IsSuccess)
            {
               result = await _mediator.Send(new SetPaymentAccountCommand(
                   fundraiserOrNone.Value.Manager.Id, fundraiserOrNone.Value.Manager.SchoolId), token);

               if (result.IsFailure)
                   return result;
            }

            result = fundraiserOrNone.Value.Open();

            return result;
        }
    }
}