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
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.DivestTreasurer
{
    [Authorize(Policy = "MustBeAtLeastFormTutor")]
    public sealed class DivestTreasurerCommand : CommandRequest
    {
        public Guid GroupId { get; }
        public Guid SchoolId { get; }

        public DivestTreasurerCommand(Guid groupId, Guid schoolId)
        {
            GroupId = groupId;
            SchoolId = schoolId;
        }
    }

    internal sealed class DivestTreasurerHandler : IRequestHandler<DivestTreasurerCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISchoolContext _schoolContext;

        public DivestTreasurerHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _schoolContext = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(DivestTreasurerCommand request, CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            var groupOrNone = schoolOrNone.Value.Groups.TryFirst(g => g.Id == groupId);
            if (groupOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            Maybe<Member> treasurerOrNone = groupOrNone.Value.Treasurer;
            if (treasurerOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(GroupRoles.Treasurer);

            schoolOrNone.Value.DivestTreasurerFromGroup(groupId);

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
