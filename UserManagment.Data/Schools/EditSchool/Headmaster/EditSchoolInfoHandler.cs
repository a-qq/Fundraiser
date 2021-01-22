using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System.Threading;
using System.Threading.Tasks;
using SchoolManagement.Core.Interfaces;

namespace SchoolManagement.Data.Schools.EditSchool.Headmaster
{
    public class EditSchoolInfoHandler : IRequestHandler<EditSchoolInfoCommand, Result<bool, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly SchoolContext _schoolContext;

        public EditSchoolInfoHandler(
            ISchoolRepository schoolRepository,
            IAuthorizationService authorizationService,
            SchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _authorizationService = authorizationService;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(EditSchoolInfoCommand request, CancellationToken cancellationToken)
        {
            await _authorizationService.VerifyAuthorizationAsync(request.SchoolId, request.AuthId, Role.Headmaster);

            Maybe<School> schoolOrNone = await _schoolRepository.GetByIdAsync(request.SchoolId);

            if (schoolOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedErrors.General.NotFound(request.SchoolId, nameof(School)));

            Description description = Description.Create(request.Description).Value;
            GroupMembersLimit limit = GroupMembersLimit.Create(request.GroupMembersLimit).Value;

            schoolOrNone.Value.EditInfo(description, limit);

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
