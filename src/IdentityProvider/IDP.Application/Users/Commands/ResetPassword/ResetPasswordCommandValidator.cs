using FluentValidation;
using IDP.Application.Common.Validation;

namespace IDP.Application.Users.Commands.ResetPassword
{
    internal sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(p => p.SecurityCode).SecurityCodeMustBeValid();
            RuleFor(p => p.NewPassword).PasswordMustBeValid();
        }
    }
}
