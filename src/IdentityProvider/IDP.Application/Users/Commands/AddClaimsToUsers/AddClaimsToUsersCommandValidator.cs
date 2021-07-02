using FluentValidation;
using IDP.Application.Users.Commands.AddClaimsToUser;

namespace IDP.Application.Users.Commands.AddClaimsToUsers
{
    internal sealed class AddClaimsToUsersCommandValidator : AbstractValidator<AddClaimsToUsersCommand>
    {
        public AddClaimsToUsersCommandValidator()
        {
            RuleFor(p => p.AddClaimsToUserCommands).NotEmpty()
                .ForEach(p => p.NotNull().SetValidator(new AddClaimsToUserCommandValidator()));
        }
    }
}
