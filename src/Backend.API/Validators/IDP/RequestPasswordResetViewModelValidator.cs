using Backend.API.IdentityServer.PasswordReset;
using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace Backend.API.Validators.IDP
{
    public class RequestPasswordResetViewModelValidator : AbstractValidator<RequestPasswordResetViewModel>
    {
        public RequestPasswordResetViewModelValidator()
        {
            RuleFor(p => p.Email).EmailMustBeValid();
        }
    }
}