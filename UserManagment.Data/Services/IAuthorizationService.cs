using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Members;
using System;
using System.Threading.Tasks;


namespace SchoolManagement.Data.Services
{
    public interface IAuthorizationService
    {
        Task<Result<School, RequestError>> VerifyAuthorizationAsync(Guid schoolId, Guid userId, Role atLeastInRole);
    }
}
