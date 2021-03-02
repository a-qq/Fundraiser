using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.PromoteHeadmaster
{
    [Authorize(Policy = "MustBeAdmin")]
    public sealed class PromoteHeadmasterCommand : CommandRequest
    {
        public Guid SchoolId { get; }
        public Guid TeacherId { get; }

        public PromoteHeadmasterCommand(Guid schoolId, Guid teacherId)
        {
            SchoolId = schoolId;
            TeacherId = teacherId;
        }
    }

    internal sealed class PromoteHeadmasterHandler : IRequestHandler<PromoteHeadmasterCommand, Result<Unit, RequestError>>
    {

        private readonly ISchoolRepository _schoolRepository;
        private readonly ISchoolContext _context;

        public PromoteHeadmasterHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(PromoteHeadmasterCommand request, CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var teacherId = new MemberId(request.TeacherId);

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (!schoolOrNone.Value.Members.Any(m => m.Id == teacherId && m.Role == Role.Teacher))
                return SharedRequestError.General.NotFound(teacherId, nameof(Role.Teacher));

            var result = schoolOrNone.Value.PromoteHeadmaster(teacherId);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
