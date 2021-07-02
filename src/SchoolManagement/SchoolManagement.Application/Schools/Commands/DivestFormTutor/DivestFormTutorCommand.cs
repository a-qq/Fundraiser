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

namespace SchoolManagement.Application.Schools.Commands.DivestFormTutor
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class DivestFormTutorCommand : IUserCommand, ISchoolAuthorizationRequest
    {
        public DivestFormTutorCommand(Guid groupId, Guid schoolId)
        {
            GroupId = groupId;
            SchoolId = schoolId;
        }

        public Guid GroupId { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class DivestFormTutorCommandHandler : IRequestHandler<DivestFormTutorCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public DivestFormTutorCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
        }

        public async Task<Result<Unit, RequestError>> Handle(DivestFormTutorCommand request,
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

            Maybe<Member> formTutorOrNone = groupOrNone.Value.FormTutor;
            if (formTutorOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(nameof(GroupRoles.FormTutor));

            schoolOrNone.Value.DivestFormTutorFromGroup(groupId);

            return Unit.Value;
        }
    }
}