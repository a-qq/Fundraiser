using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.Commands.PassOnHeadmaster
{
    [Authorize(Policy = "MustBeHeadmaster")]
    public sealed class PassOnHeadmasterCommand : CommandRequest
    {
        public PassOnHeadmasterCommand(Guid schoolId, Guid teacherId)
        {
            SchoolId = schoolId;
            TeacherId = teacherId;
        }

        public Guid SchoolId { get; }
        public Guid TeacherId { get; }
    }

    internal sealed class PassOnHeadmasterHandler : IRequestHandler<PassOnHeadmasterCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolContext _context;

        private readonly ISchoolRepository _schoolRepository;

        public PassOnHeadmasterHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(PassOnHeadmasterCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var teacherId = new MemberId(request.TeacherId);

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (!schoolOrNone.Value.Members.Any(m => m.Id == teacherId && m.Role == Role.Teacher))
                return SharedRequestError.General.NotFound(teacherId, nameof(Role.Teacher));

            if (!schoolOrNone.Value.Members.Any(m => m.Role == Role.Headmaster))
                return SharedRequestError.General.NotFound(nameof(Role.Headmaster));

            schoolOrNone.Value.PassOnHeadmaster(teacherId);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}