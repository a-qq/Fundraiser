using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.EditSchool.Admin
{
    [Authorize(Policy = PolicyNames.MustBeAdmin)]
    public sealed class EditSchoolCommand : IUserCommand
    {
        public EditSchoolCommand(string name, string description, int? groupMembersLimit, Guid schoolId)
        {
            Name = name;
            GroupMembersLimit = groupMembersLimit;
            Description = description;
            SchoolId = schoolId;
        }

        public string Name { get; }
        public string Description { get; }
        public int? GroupMembersLimit { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class EditSchoolCommandHandler : IRequestHandler<EditSchoolCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public EditSchoolCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
        }

        public async Task<Result<Unit, RequestError>> Handle(EditSchoolCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var name = Name.Create(request.Name).Value;
            var description = Description.Create(request.Description).Value;
            var limit = request.GroupMembersLimit.HasValue
                ? GroupMembersLimit.Create(request.GroupMembersLimit.Value).Value
                : null;

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            var result = schoolOrNone.Value.Edit(name, description, limit);
            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            return Unit.Value;
        }
    }
}