using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using System;
using System.Threading.Tasks;


namespace SchoolManagement.Data.Services
{
    public interface IAuthorizationService
    {
        Task<Result<Tuple<School, User>, RequestError>> GetAuthorizationContextAsync(Guid schoolId, Guid userId);
    }
}
