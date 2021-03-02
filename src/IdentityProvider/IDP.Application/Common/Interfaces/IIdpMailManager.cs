using System.Threading.Tasks;
using IDP.Domain.UserAggregate.ValueObjects;
using SharedKernel.Domain.ValueObjects;

namespace IDP.Application.Common.Interfaces
{
    public interface IIdpMailManager
    {
        Task SendResetPasswordEmail(Email email, SecurityCode securityCode);
    }
}