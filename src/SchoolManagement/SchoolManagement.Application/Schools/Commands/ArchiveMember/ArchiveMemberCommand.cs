using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.Commands.ArchiveMember
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class ArchiveMemberCommand : CommandRequest
    {
        public ArchiveMemberCommand(Guid memberId, Guid schoolId)
        {
            MemberId = memberId;
            SchoolId = schoolId;
        }

        public Guid MemberId { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class ArchiveMemberHandler : IRequestHandler<ArchiveMemberCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly ISchoolRepository _schoolRepository;

        public ArchiveMemberHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(ArchiveMemberCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var memberId = new MemberId(request.MemberId);

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (!schoolOrNone.Value.Members.Any(m => m.Id == memberId))
                return SharedRequestError.General.NotFound(memberId, nameof(Member));

            var result = schoolOrNone.Value.ArchiveMember(memberId);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}