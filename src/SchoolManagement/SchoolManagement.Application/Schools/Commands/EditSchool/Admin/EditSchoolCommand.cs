using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.EditSchool.Admin
{
    [Authorize(Policy = "MustBeAdmin")]
    public sealed class EditSchoolCommand : CommandRequest
    {
        public string Name { get; }
        public string Description { get; }
        public int? GroupMembersLimit { get; }
        public Guid SchoolId { get; }

        public EditSchoolCommand(string name, string description, int? groupMembersLimit, Guid schoolId)
        {
            Name = name;
            GroupMembersLimit = groupMembersLimit;
            Description = description;
            SchoolId = schoolId;
        }
    }

    internal sealed class EditSchoolHandler : IRequestHandler<EditSchoolCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISchoolContext _context;

        public EditSchoolHandler(
            ISchoolRepository schoolRepository,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(EditSchoolCommand request, CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var name = Name.Create(request.Name).Value;
            var description = Description.Create(request.Description).Value;
            var limit = GroupMembersLimit.Create(request.GroupMembersLimit).Value;

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            schoolOrNone.Value.Edit(name, description, limit);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
