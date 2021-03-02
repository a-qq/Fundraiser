using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.Commands.AddStudentsToGroup
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class AddStudentsToGroupCommand : CommandRequest
    {
        public AddStudentsToGroupCommand(IEnumerable<Guid> studentIds, Guid schoolId, Guid groupId)
        {
            StudentIds = studentIds.Distinct();
            SchoolId = schoolId;
            GroupId = groupId;
        }

        public IEnumerable<Guid> StudentIds { get; }
        public Guid SchoolId { get; }
        public Guid GroupId { get; }
    }

    internal sealed class
        AddStudentsToGroupHandler : IRequestHandler<AddStudentsToGroupCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly ISchoolRepository _schoolRepository;

        public AddStudentsToGroupHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(AddStudentsToGroupCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.GroupId);
            var studentIds = request.StudentIds.Select(id => new MemberId(id));

            var schoolOrNone =
                await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (!schoolOrNone.Value.Groups.Any(g => g.Id == groupId))
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            var membersToAdd =
                schoolOrNone.Value.Members.TakeWhile(m => studentIds.Contains(m.Id) && m.Role == Role.Student);

            if (studentIds.Count() != membersToAdd.Count())
            {
                var missingMembersIds
                    = studentIds.Except(membersToAdd.Select(m => m.Id));

                return SharedRequestError.General.NotFound(missingMembersIds.Select(id => id.Value), "Student");
            }

            var result = schoolOrNone.Value.AssignStudentsToGroup(groupId, studentIds);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}