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

namespace SchoolManagement.Application.Schools.Commands.Graduate
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class GraduateCommand : IUserCommand, ISchoolAuthorizationRequest
    {
        public GraduateCommand(Guid schoolId)
        {
            SchoolId = schoolId;
        }

        public Guid SchoolId { get; }
    }

    internal sealed class GraduateCommandHandler : IRequestHandler<GraduateCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public GraduateCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
        }

        public async Task<Result<Unit, RequestError>> Handle(GraduateCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken);
            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            var result = schoolOrNone.Value.Graduate();

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            return Unit.Value;
        }
    }
}