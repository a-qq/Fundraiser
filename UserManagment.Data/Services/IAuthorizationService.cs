using SchoolManagement.Core.SchoolAggregate.Members;
using System;
using System.Threading.Tasks;


namespace SchoolManagement.Data.Services
{
    public interface IAuthorizationService
    {
        Task VerifyAuthorizationAsync(Guid schoolId, Guid userId, Role atLeastInRole);
        Task VerifyFormTutorAuthorizationAsync(Guid schoolId, Guid userId, long groupId);
    }
}
