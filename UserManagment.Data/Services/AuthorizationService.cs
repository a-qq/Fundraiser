using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.Interfaces;
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

        public async Task VerifyAuthorizationAsync(Guid schoolId, Guid userId, Role role)
        {
            if (schoolId == Guid.Empty)
                throw new ArgumentNullException(nameof(schoolId));

            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            if (Administrator.FromId(userId) != null)
                return;

            Maybe<Member> memberOrNone = await _schoolRepository.GetSchoolMemberByIdAsync(schoolId, userId);
            if (memberOrNone.HasNoValue || !memberOrNone.Value.IsActive
                 || memberOrNone.Value.Role < role || memberOrNone.Value.IsArchived)
                throw new UnauthorizedAccessException($"SchoolId: {schoolId}, UserId: {userId}");

            return;
        }
    }
}
