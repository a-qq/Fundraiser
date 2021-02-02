using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
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

        public async Task VerifyFormTutorAuthorizationAsync(Guid schoolId, Guid userId, long groupId)
        {
            if (schoolId == Guid.Empty)
                throw new ArgumentNullException(nameof(schoolId));

            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            if (groupId < 1)
                throw new ArgumentOutOfRangeException(nameof(groupId));


            if (Administrator.FromId(userId) != null)
                return;

            Maybe<Member> memberOrNone = await _schoolRepository.GetSchoolMemberByIdAsync(schoolId, userId);
            if (memberOrNone.HasNoValue || !memberOrNone.Value.IsActive || memberOrNone.Value.IsArchived)
                throw new UnauthorizedAccessException($"SchoolId: {schoolId}, UserId: {userId}");

            if (memberOrNone.Value.Role == Role.Headmaster)
                return;

            if (memberOrNone.Value.Role != Role.Teacher)
                throw new UnauthorizedAccessException($"SchoolId: {schoolId}, UserId: {userId}");

            Maybe<Group> groupOrNone = await _schoolRepository.GetGroupWithFormTutorByIdAsync(schoolId, groupId);
            if (groupOrNone.HasNoValue || groupOrNone.Value?.FormTutor?.Id != userId)
                throw new UnauthorizedAccessException($"SchoolId: {schoolId}, UserId: {userId}");

            return;
        }
    }
}
