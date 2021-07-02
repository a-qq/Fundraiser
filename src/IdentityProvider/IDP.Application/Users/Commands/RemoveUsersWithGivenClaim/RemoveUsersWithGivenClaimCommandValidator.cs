using FluentValidation;

namespace IDP.Application.Users.Commands.RemoveUsersWithGivenClaim
{
    internal sealed class RemoveUsersWithGivenClaimCommandValidator : AbstractValidator<RemoveUsersWithGivenClaimCommand>
    {
        public RemoveUsersWithGivenClaimCommandValidator()
        {
            RuleFor(p => p.Type).NotEmpty();
            RuleFor(p => p.Value).NotEmpty();
        }
    }
}
