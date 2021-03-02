using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.DivestHeadmaster
{
    [Authorize(Policy = "MustBeAdmin")]
    public sealed class DivestHeadmasterCommand : CommandRequest
    {
        public Guid SchoolId { get; }

        public DivestHeadmasterCommand(Guid schoolId)
        {
            SchoolId = schoolId;
        }
    }

    internal sealed class DivestHeadmasterHandler : IRequestHandler<DivestHeadmasterCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISchoolContext _context;

        public DivestHeadmasterHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(DivestHeadmasterCommand request, CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (!schoolOrNone.Value.Members.Any(m => m.Role == Role.Headmaster))
                return SharedRequestError.General.NotFound(nameof(Role.Headmaster));

            schoolOrNone.Value.DivestHeadmaster();

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
