using FluentValidation;

namespace FundraiserManagement.Application.Fundraisers.Commands.ChangeManager
{
    internal sealed class ChangeManagerCommandValidator : AbstractValidator<ChangeManagerCommand>
    {
        public ChangeManagerCommandValidator()
        {
            RuleFor(p => p.ManagerId).NotEmpty();
            RuleFor(p => p.SchoolId).NotEmpty();
            RuleFor(p => p.FundraiserId).NotEmpty();
        }
    }
}