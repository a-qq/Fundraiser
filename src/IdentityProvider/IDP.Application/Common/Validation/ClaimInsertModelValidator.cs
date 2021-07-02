using FluentValidation;
using IDP.Application.Common.Models;

namespace IDP.Application.Common.Validation
{
    internal sealed class ClaimInsertModelValidator : AbstractValidator<ClaimInsertModel>
    {
        public ClaimInsertModelValidator()
        {
            RuleFor(p => p.Type).NotEmpty();
            RuleFor(p => p.Value).NotEmpty();
        }
    }
}
