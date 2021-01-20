using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Members;
using System;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Services
{
    internal class AuthorizationService : IAuthorizationService
    {
        private readonly ISchoolRepository _schoolRepository;

        public AuthorizationService(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        public async Task<Result<School, RequestError>> VerifyAuthorizationAsync(Guid schoolId, Guid userId, Role role)
        {
            if (schoolId == Guid.Empty)
                throw new ArgumentNullException(nameof(schoolId));

            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            if (Administrator.FromId(userId) != null)
            {
                Maybe<School> schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId);
                if (schoolOrNone.HasNoValue)
                    return Result.Failure<School, RequestError>(SharedErrors.General.NotFound(schoolId, nameof(School)));

                return Result.Success<School, RequestError>(schoolOrNone.Value);
            }

            Maybe<Member> memberOrNone = await _schoolRepository.GetSchoolMemberByIdAsync(schoolId, userId);
            if (memberOrNone.HasNoValue || !memberOrNone.Value.IsActive || memberOrNone.Value.Role < role) 
                throw new UnauthorizedAccessException($"SchoolId: {schoolId}, UserId: {userId}");
           
            return Result.Success<School, RequestError>(memberOrNone.Value.School);
        }
    }
}
