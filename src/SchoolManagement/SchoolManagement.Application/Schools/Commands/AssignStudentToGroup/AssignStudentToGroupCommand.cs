using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.AssignStudentToGroup
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class AssignStudentToGroupCommand : IUserCommand, ISchoolAuthorizationRequest
    {
        public AssignStudentToGroupCommand(Guid studentId, Guid groupId, Guid schoolId)
        {
            StudentId = studentId;
            GroupId = groupId;
            SchoolId = schoolId;
        }

        public Guid StudentId { get; }
        public Guid GroupId { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class
        AssignStudentToGroupCommandHandler : IRequestHandler<AssignStudentToGroupCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public AssignStudentToGroupCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
        }

        public async Task<Result<Unit, RequestError>> Handle(AssignStudentToGroupCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.GroupId);
            var studentId = new MemberId(request.StudentId);

            var schoolOrNone = await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            Maybe<Group> groupOrNone = schoolOrNone.Value.Groups.FirstOrDefault(g => g.Id == groupId);
            if(groupOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            if (!schoolOrNone.Value.Members.Any(m => m.Id == studentId && m.Role == Role.Student))
                return SharedRequestError.General.NotFound(studentId, "Student");

            var result = schoolOrNone.Value.AssignStudentToGroup(studentId, groupOrNone.Value.Code);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            return Unit.Value;
        }
    }
}