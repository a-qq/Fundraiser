using System;
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

namespace SchoolManagement.Application.Schools.Commands.PromoteFormTutor
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class PromoteFormTutorCommand : CommandRequest
    {
        public PromoteFormTutorCommand(Guid teacherId, Guid groupId, Guid schoolId)
        {
            TeacherId = teacherId;
            GroupId = groupId;
            SchoolId = schoolId;
        }

        public Guid TeacherId { get; }
        public Guid GroupId { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class
        PromoteFormTutorCommandHandler : IRequestHandler<PromoteFormTutorCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly ISchoolRepository _schoolRepository;

        public PromoteFormTutorCommandHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(PromoteFormTutorCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.SchoolId);
            var teacherId = new MemberId(request.TeacherId);

            var schoolOrNone = await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken);
            if (schoolOrNone.HasNoValue)
                SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (!schoolOrNone.Value.Groups.Any(g => g.Id == groupId))
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            if (!schoolOrNone.Value.Members.Any(g => g.Id == teacherId && g.Role == Role.Teacher))
                return SharedRequestError.General.NotFound(teacherId, "Teacher");

            var result = schoolOrNone.Value.PromoteFormTutor(groupId, teacherId);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}