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

namespace SchoolManagement.Application.Schools.Commands.PromoteTreasurer
{
    [Authorize(Policy = "MustBeFormTutorOrHeadmasterOrAdmin")]
    public sealed class PromoteTreasurerCommand : CommandRequest
    {
        public PromoteTreasurerCommand(Guid groupId, Guid studentId, Guid schoolId)
        {
            GroupId = groupId;
            StudentId = studentId;
            SchoolId = schoolId;
        }

        public Guid GroupId { get; }
        public Guid StudentId { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class PromoteTreasurerHandler : IRequestHandler<PromoteTreasurerCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolContext _schoolContext;
        private readonly ISchoolRepository _schoolRepository;

        public PromoteTreasurerHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _schoolContext = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(PromoteTreasurerCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.SchoolId);
            var studentId = new MemberId(request.StudentId);

            var schoolOrNone = await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken);
            if (schoolOrNone.HasNoValue)
                SharedRequestError.General.NotFound(schoolId, nameof(School));

            var groupOrNone = schoolOrNone.Value.Groups.TryFirst(g => g.Id == groupId);
            if (groupOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            if (!groupOrNone.Value.Students.Any(g => g.Id == studentId))
                return SharedRequestError.General.NotFound(studentId, "Student");

            var result = schoolOrNone.Value.PromoteTreasurer(groupId, studentId);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}