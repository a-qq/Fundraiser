using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.DivestTreasurer
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastFormTutor)]
    public sealed class DivestTreasurerCommand : IUserCommand, IGroupAuthorizationRequest
    {
        public DivestTreasurerCommand(Guid groupId, Guid schoolId)
        {
            GroupId = groupId;
            SchoolId = schoolId;
        }

        public Guid GroupId { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class DivestTreasurerCommandHandler : IRequestHandler<DivestTreasurerCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public DivestTreasurerCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
        }

        public async Task<Result<Unit, RequestError>> Handle(DivestTreasurerCommand request,
            CancellationToken cancellationToken)
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

            return Unit.Value;
        }
    }
}