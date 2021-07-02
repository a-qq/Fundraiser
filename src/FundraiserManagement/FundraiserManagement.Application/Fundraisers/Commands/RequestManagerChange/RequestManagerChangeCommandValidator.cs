using FluentValidation;

namespace FundraiserManagement.Application.Fundraisers.Commands.RequestManagerChange
{
    internal sealed class RequestManagerChangeCommandValidator : AbstractValidator<RequestManagerChangeCommand>
    {
        public RequestManagerChangeCommandValidator()
        {
            RuleFor(p => p.ManagerId).NotEmpty();
            RuleFor(p => p.SchoolId).NotEmpty();
            RuleFor(p => p.FundraiserId).NotEmpty();
        }
    }
}