using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.EditSchool.Headmaster
{
    public class EditSchoolInfoHandler : IRequestHandler<EditSchoolInfoCommand, Result<bool, RequestError>>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly SchoolContext _schoolContext;

        public EditSchoolInfoHandler(
            IAuthorizationService authorizationService,
            SchoolContext schoolContext)
        {
            _authorizationService = authorizationService;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(EditSchoolInfoCommand request, CancellationToken cancellationToken)
        {
            Result<School, RequestError> schoolOrError = 
                await _authorizationService.VerifyAuthorizationAsync(request.SchoolId, request.AuthId, Role.Headmaster);

            if (schoolOrError.IsFailure)
                return schoolOrError.ConvertFailure<bool>();

            Description description = Description.Create(request.Description).Value;

            schoolOrError.Value.EditInfo(description);

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
