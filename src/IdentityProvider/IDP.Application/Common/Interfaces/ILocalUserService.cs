using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using IDP.Application.DTOs;
using SharedKernel.Domain.Errors;

namespace IDP.Application.Common.Interfaces
{
    public interface ILocalUserService
    {
        Task<Result<UserDTO, Error>> Login(string email, string password);
        Task<Result<bool, Error>> CompleteRegistration(string securityCode, string password);
        Task<Result<bool, Error>> ResetPassword(string securityCode, string password);
        Task<Result<bool, Error>> ChangePassword(string email, string oldPassword, string newPassword);
        Task RequestPasswordReset(string email);
    }
}