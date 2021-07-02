using FluentValidation;

namespace FundraiserManagement.Application.Fundraisers.Commands.AddParticipants
{
    internal sealed class AddParticipantsCommandValidator : AbstractValidator<AddParticipantsCommand>
    {
        public AddParticipantsCommandValidator()
        {
            RuleFor(p => p.SchoolId).NotEmpty();
            RuleFor(p => p.FundraiserId).NotEmpty();
            RuleFor(p => p.ParticipantsIds).NotEmpty()
                .ForEach(x => x.NotEmpty());
        }
    }
}
