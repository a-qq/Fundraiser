using FluentValidation;
using Fundraiser.API.Validators.Rules;
using IdentityServerHost.Quickstart.UI;

namespace Fundraiser.API.Validators.IDP
{
    public class RequestPasswordResetViewModelValidator : AbstractValidator<RequestPasswordResetViewModel>
    {
        public RequestPasswordResetViewModelValidator()
        {
            RuleFor(p => p.Email).EmailMustBeValid();
        }
    }
}
