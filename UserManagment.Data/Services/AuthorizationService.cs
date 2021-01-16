using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using System;
using System.Threading.Tasks;
using static SchoolManagement.Core.SchoolAggregate.Users.User;

namespace SchoolManagement.Data.Services
{
    internal class AuthorizationService : IAuthorizationService
    {
        private readonly ISchoolRepository _schoolRepository;


        public AuthorizationService(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;

        }
        public async Task<Result<Tuple<School, User>, RequestError>> GetAuthorizationContextAsync(Guid schoolId, Guid userId)
        {
            if (schoolId == Guid.Empty)
                throw new ArgumentNullException(nameof(schoolId));

            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            if (userId == Admin.Id)
            {
                Maybe<School> schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId);
                if (schoolOrNone.HasNoValue)
                    return Result.Failure<Tuple<School, User>, RequestError>(SharedErrors.General.NotFound(nameof(School), schoolId.ToString()));

                return Result.Success<Tuple<School, User>, RequestError>(new Tuple<School, User>(schoolOrNone.Value, Admin));
            }

            Maybe<User> userOrNone = await _schoolRepository.GetSchoolMemberByIdAsync(schoolId, userId);
            if (userOrNone.HasNoValue || !userOrNone.Value.IsActive) 
                throw new UnauthorizedAccessException($"SchoolId: {schoolId}, UserId: {userId}");
            
            //only if Admin would be loaded from cache
            if (userOrNone.Value.School == null)
                throw new InvalidOperationException(nameof(GetAuthorizationContextAsync));


            return Result.Success<Tuple<School, User>, RequestError>(new Tuple<School, User>(userOrNone.Value.School, userOrNone.Value));
        }
    }
}
