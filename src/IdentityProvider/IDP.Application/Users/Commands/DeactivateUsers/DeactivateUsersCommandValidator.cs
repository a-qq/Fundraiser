using FluentValidation;
using IDP.Application.Users.Commands.DeactivateUser;

namespace IDP.Application.Users.Commands.DeactivateUsers
{
    internal sealed class DeactivateUsersCommandValidator : AbstractValidator<DeactivateUsersCommand>
    {
        public DeactivateUsersCommandValidator()
        {
            RuleFor(p => p.UserDeactivationCommands).NotEmpty();

            RuleForEach(p => p.UserDeactivationCommands).NotNull()
                .SetValidator(new DeactivateUserCommandValidator());
        }
    }
}
