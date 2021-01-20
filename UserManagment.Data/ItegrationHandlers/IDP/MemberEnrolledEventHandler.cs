using Dapper;
using Fundraiser.SharedKernel.Managers;
using Fundraiser.SharedKernel.Settings;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using Microsoft.Extensions.Options;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.IntegrationHandlers.IDP
{
    public class MemberEnrolledEventHandler : INotificationHandler<MemberEnrolledEvent>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMailManager _mailManager;
        private readonly FrontendSettings _frontendSettings;

        public MemberEnrolledEventHandler(
            ISqlConnectionFactory sqlConnectionFactory,
            IMailManager mailManager,
            IOptions<FrontendSettings> mailOptions)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mailManager = mailManager;
            _frontendSettings = mailOptions.Value;
        }

        public async Task Handle(MemberEnrolledEvent notification, CancellationToken cancellationToken)
        {
            var connection = this._sqlConnectionFactory.GetOpenConnection();

            const string sqlInsert1 = "INSERT INTO [auth].[Users] ([Subject], [Email], [IsActive], [SecurityCode], [SecurityCodeIssuedAt]) VALUES " +
                                     "(@Subject, @Email, @IsActive, @SecurityCode, @SecurityCodeIssuedAt)";
            var securityCode = GenerateSecurityCode();
            await connection.ExecuteAsync(sqlInsert1, new
            {
                Subject = notification.MemberId.ToString(),
                Email = notification.Email.Value,
                IsActive = false,
                SecurityCode = securityCode,
                SecurityCodeIssuedAt = DateTime.UtcNow
            });

            const string sqlInsert2 = "INSERT INTO[auth].[Claims]([UserSubject], [Type], [Value]) VALUES " +
                                     "(@UserId, @Type, @Value)";


            await connection.ExecuteAsync(sqlInsert2, new
            {
                UserId = notification.MemberId.ToString(),
                Type = "given_name",
                Value = notification.FirstName.Value
            });

            await connection.ExecuteAsync(sqlInsert2, new
            {
                UserId = notification.MemberId.ToString(),
                Type = "family_name",
                Value = notification.LastName.Value
            });

            await connection.ExecuteAsync(sqlInsert2, new
            {
                UserId = notification.MemberId.ToString(),
                Type = "role",
                Value = notification.Role.ToString()
            });

            await connection.ExecuteAsync(sqlInsert2, new
            {
                UserId = notification.MemberId.ToString(),
                Type = "school_id",
                Value = notification.SchoolId.ToString()
            });

            await connection.ExecuteAsync(sqlInsert2, new
            {
                UserId = notification.MemberId.ToString(),
                Type = "gender",
                Value = notification.Gender.ToString()
            });

            await _mailManager.SendMailAsync(
                notification.Email.Value,
                $"{notification.FirstName.Value}, welcome in your school's managment system!",
                $"<hmtl><body>Dear {notification.FirstName.Value}<br/>" +
                $"<p>Please click the link below to complete registration proccess!" +
                $"</p><p><a href='{_frontendSettings.IDPUrl}Registration/Register/?SecurityCode={securityCode.Replace("+", "%2B")}'>Register</a></p>" +
                $"<p>Have a great day!</p></body></html>");
            //&ReturnUrl=client_id=fundraiserclient&redirect_uri={_frontendSettings.ClientUrl}index.html
        }

        private string GenerateSecurityCode()
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var securityCodeData = new byte[128];
                randomNumberGenerator.GetBytes(securityCodeData);
                return Convert.ToBase64String(securityCodeData);
            }
        }
    }
}
