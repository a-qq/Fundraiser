using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.DeleteGroup
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class DeleteGroupCommand : IUserCommand, ISchoolAuthorizationRequest
    {
        public DeleteGroupCommand(Guid groupId, Guid schoolId)
        {
            GroupId = groupId;
            SchoolId = schoolId;
        }

        public Guid GroupId { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public DeleteGroupCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
        }

        public async Task<Result<Unit, RequestError>> Handle(DeleteGroupCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.GroupId);

            var schoolOrNone = await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken, true);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (schoolOrNone.Value.Groups.All(g => g.Id != groupId))
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            schoolOrNone.Value.DeleteGroup(groupId);

            return Unit.Value;
        }
    }
}