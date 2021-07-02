using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Application.IntegrationEvents.Events;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.DeleteSchool
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class DeleteSchoolCommand : IUserCommand, ISchoolAuthorizationRequest
    {
        public DeleteSchoolCommand(Guid schoolId)
        {
            SchoolId = schoolId;
        }

        public Guid SchoolId { get; }
    }

    internal sealed class DeleteSchoolCommandHandler : IRequestHandler<DeleteSchoolCommand, Result<Unit, RequestError>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;
        private readonly ISchoolRepository _schoolRepository;

        public DeleteSchoolCommandHandler(
            ISchoolRepository schoolRepository,
            ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
            _logger = Guard.Against.Null(logger, nameof(logger));
            _integrationEventService = Guard.Against.Null(integrationEventService, nameof(integrationEventService));

        }

        public async Task<Result<Unit, RequestError>> Handle(DeleteSchoolCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            _schoolRepository.Remove(schoolOrNone.Value);

            _logger.CreateLogger<DeleteSchoolCommandHandler>()
                .LogTrace("School with Id: {SchoolId} has been successfully removed!",
                    schoolOrNone.Value.Id);

            await _integrationEventService.AddAndSaveEventAsync(
                new SchoolRemovedIntegrationEvent(schoolOrNone.Value.Id));

            return Unit.Value;
        }
    }
}