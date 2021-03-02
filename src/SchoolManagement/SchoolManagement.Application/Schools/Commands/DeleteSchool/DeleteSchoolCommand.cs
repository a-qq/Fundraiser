using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.Commands.DeleteSchool
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class DeleteSchoolCommand : CommandRequest
    {
        public DeleteSchoolCommand(Guid schoolId)
        {
            SchoolId = schoolId;
        }

        public Guid SchoolId { get; }
    }

    internal sealed class DeleteSchoolHandler : IRequestHandler<DeleteSchoolCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly IDomainEventService _domainEventService;
        private readonly ISchoolRepository _schoolRepository;

        public DeleteSchoolHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext,
            IDomainEventService domainEventService)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
            _domainEventService = domainEventService;
        }

        public async Task<Result<Unit, RequestError>> Handle(DeleteSchoolCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            _schoolRepository.Remove(schoolOrNone.Value);

            await _context.SaveChangesAsync(cancellationToken);

            await _domainEventService.Publish(new SchoolRemovedEvent(schoolId));

            return Unit.Value;
        }
    }
}