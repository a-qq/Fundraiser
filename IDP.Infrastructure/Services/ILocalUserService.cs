using CSharpFunctionalExtensions;
using IDP.Infrastructure.DTOs;
using System.Threading.Tasks;

namespace IDP.Infrastructure.Services
{
    public interface ILocalUserService
    {
        Task<Result<UserDTO>> Login(string email, string password);
        Task<Result> CompleteRegistration(string securityCode, string password);
        Task<Result> ResetPassword(string securityCode, string password);
        Task<Result> ChangePassword(string email, string oldPassword, string newPassword);
        Task SendResetPasswordEmail(string email);
    }
}
