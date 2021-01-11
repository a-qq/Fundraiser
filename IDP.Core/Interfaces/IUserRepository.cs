using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using IDP.Core.UserAggregate.Entities;
using System.Threading.Tasks;

namespace IDP.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<Maybe<User>> GetUserByEmailAsync(Email email);
        Task<Maybe<User>> GetUserBySubjectAsync(string subject);
        Task<Maybe<User>> GetUserBySecurityCodeAsync(string securityCode);
    }
}
