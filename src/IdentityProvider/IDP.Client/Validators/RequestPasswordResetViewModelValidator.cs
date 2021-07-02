using FluentValidation;
using IDP.Client.Controllers.PasswordReset;
using SharedKernel.Infrastructure.Extensions;

namespace IDP.Client.Validators
{
    public class RequestPasswordResetViewModelValidator : AbstractValidator<RequestPasswordResetViewModel>
    {
        public RequestPasswordResetViewModelValidator()
        {
            RuleFor(p => p.Email).EmailMustBeValid();
        }
    }
}