using System;
using FluentValidation;
using SchoolManagement.Application.Common.ValidationRules;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.EnrollMembersFromCsv
{
    internal sealed class RawMemberFromCsvModelValidator : AbstractValidator<RawMemberFromCsvModel>
    {
        public RawMemberFromCsvModelValidator()
        {
            RuleFor(p => p.FirstName).FirstNameMustBeValid();
            RuleFor(p => p.LastName).LastNameMustBeValid();
            RuleFor(p => p.Email).EmailMustBeValid();
            RuleFor(p => p.Role).RoleMustBeValid();
            RuleFor(p => p.Gender).GenderMustBeValid();
            When(
                p => !string.IsNullOrWhiteSpace(p.Role) &&
                     p.Role.Trim().Equals(Role.Student.ToString(), StringComparison.OrdinalIgnoreCase), () =>
                {
                    RuleFor(p => p.Group).Custom((property, context) =>
                    {
                        if (!string.IsNullOrWhiteSpace(property))
                        {
                            var numberValidation = Number.Validate(
                                int.TryParse(property.Substring(0, 1), out var number) ? number : 0, "Group's number");

                            if (numberValidation.IsFailure)
                                context.AddFailure(numberValidation.Error);

                            var signValidation = Sign.Validate(property.Length > 0
                                ? property.Substring(1)
                                : string.Empty, "Group's sign");

                            if (signValidation.IsFailure)
                                foreach (var error in signValidation.Error.Errors)
                                    context.AddFailure(error);
                        }
                    });
                }).Otherwise(() =>
            {
                RuleFor(p => p.Group).Empty()
                    .WithMessage("Field '{PropertyName}' must be empty, if role is other then student!");
            });
        }
    }
}