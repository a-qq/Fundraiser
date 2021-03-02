using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.RestoreMember
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class RestoreMemberCommand : CommandRequest
    {
        public Guid SchoolId { get; }
        public Guid MemberId { get; }

        public RestoreMemberCommand(Guid schoolId, Guid memberId)
        {
            SchoolId = schoolId;
            MemberId = memberId;
        }
    }

    internal sealed class RestoreMemberHandler : IRequestHandler<RestoreMemberCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISchoolContext _context;

        public RestoreMemberHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(RestoreMemberCommand request, CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var memberId = new MemberId(request.MemberId);

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken, true);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (!schoolOrNone.Value.Members.Any(m => m.Id == memberId))
                return SharedRequestError.General.NotFound(memberId, nameof(Member));

            var result = schoolOrNone.Value.RestoreMember(memberId);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}