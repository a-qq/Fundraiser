using FluentValidation;
using IDP.Application.Common.Validation;
using SharedKernel.Infrastructure.Extensions;

namespace IDP.Application.Users.Commands.AddUser
{
    internal sealed class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator()
        {
            RuleFor(p => p.Email).EmailMustBeValid();
            RuleFor(p => p.Subject).SubjectMustBeValid();
            RuleFor(p => p.HoursToExpire).HoursToExpireMustBeValid();
            RuleForEach(p => p.Claims).NotNull().SetValidator(new ClaimInsertModelValidator());
        }
    }
}
