using FluentValidation;
using IDP.Application.Common.Validation;

namespace IDP.Application.Users.Commands.AddClaimsToUser
{
    internal sealed class AddClaimsToUserCommandValidator : AbstractValidator<AddClaimsToUserCommand>
    {
        public AddClaimsToUserCommandValidator()
        {
            RuleFor(p => p.Subject).SubjectMustBeValid();
            RuleForEach(p => p.Claims).NotNull().SetValidator(new ClaimInsertModelValidator());
        }   
    }
}
