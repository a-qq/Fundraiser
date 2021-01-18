using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System;
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
            Result<Tuple<School, User>, RequestError> authContext = 
                await _authorizationService.GetAuthorizationContextAsync(request.SchoolId, request.AuthId);

            if (authContext.IsFailure)
                return authContext.ConvertFailure<bool>();

            User currentUser = authContext.Value.Item2;
            School school = authContext.Value.Item1;
            Description description = Description.Create(request.Description).Value;

            currentUser.EditSchoolInfo(description, school);

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
