using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.Commands.DeleteGroup
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class DeleteGroupCommand : CommandRequest
    {
        public DeleteGroupCommand(Guid groupId, Guid schoolId)
        {
            GroupId = groupId;
            SchoolId = schoolId;
        }

        public Guid GroupId { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly ISchoolRepository _schoolRepository;

        public DeleteGroupHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(DeleteGroupCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var groupId = new GroupId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdWithGroupsAsync(schoolId, cancellationToken, true);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            if (!schoolOrNone.Value.Groups.Any(g => g.Id == groupId))
                return SharedRequestError.General.NotFound(groupId, nameof(Group));

            schoolOrNone.Value.DeleteGroup(groupId);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}