using FluentValidation;
using IDP.Application.Common.Validation;
using SharedKernel.Infrastructure.Extensions;

namespace IDP.Application.Users.Commands.ChangePassword
{
    internal sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(p => p.Email).EmailMustBeValid();
            RuleFor(p => p.OldPassword).NotEmpty();
            RuleFor(p => p.NewPassword).PasswordMustBeValid();
        }
    }
}
