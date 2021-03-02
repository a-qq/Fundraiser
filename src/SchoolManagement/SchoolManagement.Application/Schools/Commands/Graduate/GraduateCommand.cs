using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.Commands.Graduate
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class GraduateCommand : CommandRequest
    {
        public GraduateCommand(Guid schoolId)
        {
            SchoolId = schoolId;
        }

        public Guid SchoolId { get; }
    }

    internal sealed class GraduateHandler : IRequestHandler<GraduateCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly ISchoolRepository _schoolRepository;

        public GraduateHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(GraduateCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken);
            if (schoolOrNone.HasNoValue)
                SharedRequestError.General.NotFound(schoolId, nameof(School));

            var result = schoolOrNone.Value.Graduate();

            if (result.IsFailure)
                SharedRequestError.General.BusinessRuleViolation(result.Error);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}