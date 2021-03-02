using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.Commands.DivestFormTutor
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class DivestFormTutorCommand : CommandRequest
    {
        public DivestFormTutorCommand(Guid groupId, Guid schoolId)
        {
            GroupId = groupId;
            SchoolId = schoolId;
        }

        public Guid GroupId { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class DivestFormTutorHandler : IRequestHandler<DivestFormTutorCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly ISchoolRepository _schoolRepository;

        public DivestFormTutorHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(DivestFormTutorCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            var groupOrNone = schoolOrNone.Value.Groups.TryFirst(g => g.Id == groupId);
            if (groupOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            Maybe<Member> formTutorOrNone = groupOrNone.Value.FormTutor;
            if (formTutorOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(nameof(GroupRoles.FormTutor));

            schoolOrNone.Value.DivestFormTutorFromGroup(groupId);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}