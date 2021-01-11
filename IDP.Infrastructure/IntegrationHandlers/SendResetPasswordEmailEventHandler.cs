using Fundraiser.SharedKernel.Managers;
using Fundraiser.SharedKernel.Settings;
using IDP.Core.UserAggregate.Events;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Infrastructure.IntegrationHandlers
{
    public class SendResetPasswordEmailEventHandler : INotificationHandler<SendResetPasswordEmailEvent>
    {
        private readonly IMailManager _mailManager;
        private readonly FrontendSettings _frontendSettings;

        public SendResetPasswordEmailEventHandler(
            IMailManager mailManager,
            IOptions<FrontendSettings> mailOptions)
        {
            _mailManager = mailManager;
            _frontendSettings = mailOptions.Value;
        }
        public async Task Handle(SendResetPasswordEmailEvent notification, CancellationToken cancellationToken)
        {
            await _mailManager.SendMailAsync(notification.Email, "Your new reset password link from your school's management system!",
                $"<hmtl><body><p>Please click the link below to reset your password!<br/>" +
                $"</p><p><a href='{_frontendSettings.IDPUrl}PasswordReset/ResetPassword/?securityCode={notification.SecurityCode.Value.Replace("+", "%2B")}' > Reset password</a></p>" +
                $"<p>This link will be valid until {notification.SecurityCode.ExpirationDate.Date.Value:MM/dd/yyyy HH:mm}.</p>"+
                $"<p>If you haven't requested password reset please ignore this message!</p></body></html>");
        }
    }
}
