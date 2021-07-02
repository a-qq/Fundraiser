using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.EditSchool.Headmaster
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class EditSchoolInfoCommand : IUserCommand, ISchoolAuthorizationRequest
    {
        public EditSchoolInfoCommand(string description, int? groupMembersLimit, Guid schoolId)
        {
            Description = description;
            GroupMembersLimit = groupMembersLimit;
            SchoolId = schoolId;
        }

        public string Description { get; }
        public int? GroupMembersLimit { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class EditSchoolInfoCommandHandler : IRequestHandler<EditSchoolInfoCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public EditSchoolInfoCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
        }

        public async Task<Result<Unit, RequestError>> Handle(EditSchoolInfoCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var description = Description.Create(request.Description).Value;
            var limit = request.GroupMembersLimit.HasValue
                ? GroupMembersLimit.Create(request.GroupMembersLimit.Value).Value
                : null;

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            var result = schoolOrNone.Value.EditInfo(description, limit);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            return Unit.Value;
        }
    }
}