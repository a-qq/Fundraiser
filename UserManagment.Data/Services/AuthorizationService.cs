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
        public AuthorizationService(
            ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }
        public async Task<Result<Tuple<School, User>, RequestError>> GetAuthorizationContextAsync(Guid schoolId, Guid userId)
        {
            if (schoolId == Guid.Empty)
                throw new ArgumentNullException(nameof(schoolId));

            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            Maybe<School> school = await _schoolRepository.GetByIdAsync(schoolId);
            User currentUser;

            if (userId == Admin.Id)
            {
                if (school.HasNoValue)
                    return Result.Failure<Tuple<School, User>, RequestError>(SharedErrors.General.NotFound(nameof(School), schoolId.ToString()));
                currentUser = Admin;
            }
            else
            {
                if (school.HasNoValue)
                    return Result.Failure<Tuple<School, User>, RequestError>(SharedErrors.General.Unauthorized(userId.ToString()));

                Maybe<User> currentUserOrNone = await _schoolRepository.GetSchoolMemberByIdAsync(school.Value.Id, userId);
                if (currentUserOrNone.HasNoValue)
                    return Result.Failure<Tuple<School, User>, RequestError>(SharedErrors.General.Unauthorized(userId.ToString()));

                currentUser = currentUserOrNone.Value;
            }

            return Result.Success<Tuple<School, User>, RequestError>(new Tuple<School, User>(school.Value, currentUser));
        }
    }
}
