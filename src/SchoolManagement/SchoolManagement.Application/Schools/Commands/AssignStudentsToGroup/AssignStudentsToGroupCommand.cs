using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Errors;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.AssignStudentsToGroup
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class AssignStudentsToGroupCommand : IUserCommand, ISchoolAuthorizationRequest
    {
        public AssignStudentsToGroupCommand(IEnumerable<Guid> studentIds, Guid schoolId, Guid groupId)
        {
            StudentIds = studentIds;
            SchoolId = schoolId;
            GroupId = groupId;
        }

        public IEnumerable<Guid> StudentIds { get; }
        public Guid SchoolId { get; }
        public Guid GroupId { get; }
    }

    internal sealed class
        AssignStudentsToGroupCommandHandler : IRequestHandler<AssignStudentsToGroupCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public AssignStudentsToGroupCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
        }

        public async Task<Result<Unit, RequestError>> Handle(AssignStudentsToGroupCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.GroupId);
            var studentIds = request.StudentIds
                .Select(id => new MemberId(id))
                .ToHashSet();

            var schoolOrNone =
                await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            Maybe<Group> groupOrNone = schoolOrNone.Value.Groups.SingleOrDefault(g => g.Id == groupId);
            if (groupOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            var membersToAdd = schoolOrNone.Value.Members
                .Where(m => studentIds.Contains(m.Id) && m.Role == Role.Student).ToList();

            if (studentIds.Count != membersToAdd.Count)
            {
                var missingMembersIds
                    = studentIds.Except(membersToAdd.Select(m => m.Id));

                return SharedRequestError.General.NotFound(missingMembersIds.Select(id => id.Value), "Student");
            }

            var validation = groupOrNone.Value.HaveSpaceFor(membersToAdd.Count);
            if (validation.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(validation.Error);

            var result = Result.Combine(studentIds
                .Select(id => schoolOrNone.Value.AssignStudentToGroup(id, groupOrNone.Value.Code)));

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            return Unit.Value;
        }
    }
}