using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.Commands.EditSchool.Headmaster
{
    [Authorize(Policy = "MustBeHeadmaster")]
    public sealed class EditSchoolInfoCommand : CommandRequest
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

    internal sealed class EditSchoolInfoHandler : IRequestHandler<EditSchoolInfoCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly ISchoolRepository _schoolRepository;

        public EditSchoolInfoHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(EditSchoolInfoCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var description = Description.Create(request.Description).Value;
            var limit = GroupMembersLimit.Create(request.GroupMembersLimit).Value;

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            schoolOrNone.Value.EditInfo(description, limit);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}