using FluentValidation;
using IDP.Application.Users.Commands.RemoveClaimsFromUser;

namespace IDP.Application.Users.Commands.RemoveClaimsFromUsers
{
    internal sealed class RemoveClaimsFromUsersCommandValidator : AbstractValidator<RemoveClaimsFromUsersCommand>
    {
        public RemoveClaimsFromUsersCommandValidator()
        {
            RuleFor(p => p.RemoveClaimsFromUserCommands).NotEmpty()
                .ForEach(p => p.NotNull().SetValidator(new RemoveClaimsFromUserCommandValidator()));
        }
    }
}
