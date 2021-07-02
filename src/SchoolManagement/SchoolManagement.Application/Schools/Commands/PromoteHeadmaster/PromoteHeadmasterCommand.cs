using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.PromoteHeadmaster
{
    [Authorize(Policy = PolicyNames.MustBeAdmin)]
    public sealed class PromoteHeadmasterCommand : IUserCommand
    {
        public PromoteHeadmasterCommand(Guid schoolId, Guid teacherId)
        {
            SchoolId = schoolId;
            TeacherId = teacherId;
        }

        public Guid SchoolId { get; }
        public Guid TeacherId { get; }
    }

    internal sealed class
        PromoteHeadmasterCommandHandler : IRequestHandler<PromoteHeadmasterCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public PromoteHeadmasterCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        public async Task<Result<Unit, RequestError>> Handle(PromoteHeadmasterCommand request,
            CancellationToken cancellationToken)
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

            return Unit.Value;
        }
    }
}