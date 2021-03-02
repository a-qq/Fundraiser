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

namespace SchoolManagement.Application.Schools.Commands.ChangeGroupAssignment
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class ChangeGroupAssignmentCommand : CommandRequest
    {
        public Guid StudentId { get; }
        public Guid GroupId { get; }
        public Guid SchoolId { get; }

        public ChangeGroupAssignmentCommand(Guid studentId, Guid groupId, Guid schoolId)
        {
            StudentId = studentId;
            GroupId = groupId;
            SchoolId = schoolId;
        }
    }

    internal sealed class ChangeGroupAssignmentHandler : IRequestHandler<ChangeGroupAssignmentCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISchoolContext _schoolContext;

        public ChangeGroupAssignmentHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _schoolContext = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(ChangeGroupAssignmentCommand request, CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.SchoolId);
            var studentId = new MemberId(request.StudentId);

            var schoolOrNone = await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken);
           
            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (!schoolOrNone.Value.Groups.Any(g => g.Id == groupId))
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            if (!schoolOrNone.Value.Members.Any(m => m.Id == studentId && m.Role == Role.Student))
                return SharedRequestError.General.NotFound(studentId, "Student");

            var result = schoolOrNone.Value.ReassignStudentToGroup(groupId, studentId);
            
            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
