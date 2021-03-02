using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.DisenrollStudentFromGroup
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class DisenrollStudentFromGroupCommand : CommandRequest
    {
        public Guid GroupId { get; }
        public Guid StudentId { get; }
        public Guid SchoolId { get; }

        public DisenrollStudentFromGroupCommand(Guid groupId, Guid studentId, Guid schoolId)
        {
            GroupId = groupId;
            StudentId = studentId;
            SchoolId = schoolId;
        }
    }

    internal sealed class DisenrollStudentFromGroupHandler : IRequestHandler<DisenrollStudentFromGroupCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISchoolContext _context;

        public DisenrollStudentFromGroupHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(DisenrollStudentFromGroupCommand request, CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.SchoolId);
            var studentId = new MemberId(request.StudentId);

            var schoolOrNone = await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            var groupOrNone = schoolOrNone.Value.Groups.TryFirst(g => g.Id == groupId);
            if (groupOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            if (!groupOrNone.Value.Students.Any(s => s.Id == studentId))
                return SharedRequestError.General.NotFound(studentId, "Student");

            schoolOrNone.Value.DisenrollStudentFromGroup(groupId, studentId);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
