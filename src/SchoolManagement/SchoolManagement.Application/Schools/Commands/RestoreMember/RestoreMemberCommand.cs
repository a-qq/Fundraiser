using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.RestoreMember
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class RestoreMemberCommand : IUserCommand, ISchoolAuthorizationRequest
    {
        public RestoreMemberCommand(Guid schoolId, Guid memberId)
        {
            SchoolId = schoolId;
            MemberId = memberId;
        }

        public Guid SchoolId { get; }
        public Guid MemberId { get; }
    }

    internal sealed class RestoreMemberCommandHandler : IRequestHandler<RestoreMemberCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public RestoreMemberCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        public async Task<Result<Unit, RequestError>> Handle(RestoreMemberCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var memberId = new MemberId(request.MemberId);

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken, true);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (schoolOrNone.Value.Members.All(m => m.Id != memberId))
                return SharedRequestError.General.NotFound(memberId, nameof(Member));

            var result = schoolOrNone.Value.RestoreMember(memberId);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            return Unit.Value;
        }
    }
}