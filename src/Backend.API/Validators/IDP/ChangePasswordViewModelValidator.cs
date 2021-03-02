using Backend.API.IdentityServer.ChangePassword;
using Backend.API.Validators.Rules;
using FluentValidation;

namespace Backend.API.Validators.IDP
{
    public class ChangePasswordViewModelValidator : AbstractValidator<ChangePasswordViewModel>
    {
        public ChangePasswordViewModelValidator()
        {
            RuleFor(p => p.OldPassword).NotNull().WithMessage("Old password is required!").NotEmpty()
                .WithMessage("Old password is required!");
            RuleFor(p => p.NewPassword).PasswordMustBeValid();
            RuleFor(p => p.ConfirmPassword).Equal(p => p.NewPassword).WithMessage("Passwords do not match!");
        }
    }
}