using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.DivestHeadmaster
{
    [Authorize(Policy = PolicyNames.MustBeAdmin)]
    public sealed class DivestHeadmasterCommand : IUserCommand
    {
        public DivestHeadmasterCommand(Guid schoolId)
        {
            SchoolId = schoolId;
        }

        public Guid SchoolId { get; }
    }

    internal sealed class DivestHeadmasterCommandHandler : IRequestHandler<DivestHeadmasterCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public DivestHeadmasterCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
        }

        public async Task<Result<Unit, RequestError>> Handle(DivestHeadmasterCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (schoolOrNone.Value.Members.All(m => m.Role != Role.Headmaster))
                return SharedRequestError.General.NotFound(nameof(Role.Headmaster));

            schoolOrNone.Value.DivestHeadmaster();

            return Unit.Value;
        }
    }
}