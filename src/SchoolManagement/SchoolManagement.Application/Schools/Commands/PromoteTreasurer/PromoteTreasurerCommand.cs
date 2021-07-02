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

namespace SchoolManagement.Application.Schools.Commands.PromoteTreasurer
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastFormTutor)]
    public sealed class PromoteTreasurerCommand : IUserCommand, IGroupAuthorizationRequest
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

    internal sealed class PromoteTreasurerCommandHandler : IRequestHandler<PromoteTreasurerCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public PromoteTreasurerCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        public async Task<Result<Unit, RequestError>> Handle(PromoteTreasurerCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.GroupId);
            var studentId = new MemberId(request.StudentId);

            var schoolOrNone = await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken);
            if (schoolOrNone.HasNoValue)
                SharedRequestError.General.NotFound(schoolId, nameof(School));

            var groupOrNone = schoolOrNone.Value.Groups.TryFirst(g => g.Id == groupId);
            if (groupOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            if (groupOrNone.Value.Students.All(g => g.Id != studentId))
                return SharedRequestError.General.NotFound(studentId, "Student");

            var result = schoolOrNone.Value.PromoteTreasurer(groupId, studentId);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);
            
            return Unit.Value;
        }
    }
}