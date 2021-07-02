using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace IDP.Application.Users.Commands.RequestPasswordReset
{
    internal sealed class RequestPasswordResetCommandValidator : AbstractValidator<RequestPasswordResetCommand>
    {
        public RequestPasswordResetCommandValidator()
        {
            RuleFor(p => p.Email).EmailMustBeValid();
        }
    }
}
