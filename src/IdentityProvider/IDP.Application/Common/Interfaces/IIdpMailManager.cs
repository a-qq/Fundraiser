using IDP.Domain.UserAggregate.ValueObjects;
using SharedKernel.Domain.ValueObjects;
using System.Threading.Tasks;

namespace IDP.Application.Common.Interfaces
{
    public interface IIdpMailManager
    {
        Task SendResetPasswordEmail(Email email, SecurityCode securityCode);
    }
}
