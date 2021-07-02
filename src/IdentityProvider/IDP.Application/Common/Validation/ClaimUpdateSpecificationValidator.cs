using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using IDP.Application.Common.Models;

namespace IDP.Application.Common.Validation
{
    internal sealed class ClaimUpdateSpecificationValidator : AbstractValidator<ClaimUpdateSpecification>
    {
        public ClaimUpdateSpecificationValidator()
        {
            RuleFor(p => p.Type).NotEmpty();
            RuleFor(p => p.NewValue).NotEmpty();
            When(p => p.OldValue.HasValue,
                () => RuleFor(p => p.OldValue.Value).NotEmpty().WithName(p => nameof(p.OldValue)));
        }
    }
}
