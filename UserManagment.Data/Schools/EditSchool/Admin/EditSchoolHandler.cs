using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Data.Database;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.EditSchool.Admin
{
    public class EditSchoolHandler : IRequestHandler<EditSchoolCommand, Result<bool, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly SchoolContext _schoolContext;

        public EditSchoolHandler(
            ISchoolRepository schoolRepository,
            SchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(EditSchoolCommand request, CancellationToken cancellationToken)
        {
            if (Administrator.FromId(request.AuthId) == null)
                throw new UnauthorizedAccessException($"UserId: {request.AuthId}");

            Maybe<School> schoolOrNone = await _schoolRepository.GetByIdAsync(request.SchoolId);

            if (schoolOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedErrors.General.NotFound(request.SchoolId, nameof(School)));

            Name name = Name.Create(request.Name).Value;
            Description description = Description.Create(request.Description).Value;
            GroupMembersLimit limit = GroupMembersLimit.Create(request.GroupMembersLimit).Value;

            schoolOrNone.Value.Edit(name, description, limit);

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
