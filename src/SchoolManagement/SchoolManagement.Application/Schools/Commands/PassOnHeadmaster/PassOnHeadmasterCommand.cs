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

namespace SchoolManagement.Application.Schools.Commands.PassOnHeadmaster
{
    [Authorize(Policy = PolicyNames.MustBeHeadmaster)]
    public sealed class PassOnHeadmasterCommand : IUserCommand, ISchoolAuthorizationRequest
    {
        public PassOnHeadmasterCommand(Guid schoolId, Guid teacherId)
        {
            SchoolId = schoolId;
            TeacherId = teacherId;
        }

        public Guid SchoolId { get; }
        public Guid TeacherId { get; }
    }

    internal sealed class PassOnHeadmasterCommandHandler : IRequestHandler<PassOnHeadmasterCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;

        public PassOnHeadmasterCommandHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        public async Task<Result<Unit, RequestError>> Handle(PassOnHeadmasterCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var teacherId = new MemberId(request.TeacherId);

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (!schoolOrNone.Value.Members.Any(m => m.Id == teacherId && m.Role == Role.Teacher))
                return SharedRequestError.General.NotFound(teacherId, nameof(Role.Teacher));

            if (schoolOrNone.Value.Members.All(m => m.Role != Role.Headmaster))
                return SharedRequestError.General.NotFound(nameof(Role.Headmaster));

            schoolOrNone.Value.PassOnHeadmaster(teacherId);

            return Unit.Value;
        }
    }
}