using CSharpFunctionalExtensions;
using FluentValidation;
using Fundraiser.API.Validators.Rules;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Data.Schools.EnrollMembersFromCsv;
using System;

namespace Fundraiser.API.Validators.Management
{
    public class RawMemberFromCsvModelValidator : AbstractValidator<RawMemberFromCsvModel>
    {
        public RawMemberFromCsvModelValidator()
        {
            RuleFor(p => p.FirstName).FirstNameMustBeValid();
            RuleFor(p => p.LastName).LastNameMustBeValid();
            RuleFor(p => p.Email).EmailMustBeValid();
            RuleFor(p => p.Role).RoleMustBeValid();
            RuleFor(p => p.Gender).GenderMustBeValid();
            When(p => !string.IsNullOrWhiteSpace(p.Role) && p.Role.Trim().Equals(Role.Student.ToString(), StringComparison.OrdinalIgnoreCase), () =>
            {
                RuleFor(p => p.Group).Custom((property, context) =>
                {
                    Result numberValidation = Number.Validate(!string.IsNullOrWhiteSpace(property)
                         && int.TryParse(property.Substring(0, 1), out int number) ? number : 0, "Group's number");

                    if (numberValidation.IsFailure)
                        context.AddFailure(numberValidation.Error);

                    Result<bool, Error> signValidation = Sign.Validate(property.Length > 0
                        ? property.Substring(1) : string.Empty, "Group's sign");

                    if (signValidation.IsFailure)
                        foreach (var error in signValidation.Error.Errors)
                            context.AddFailure(error);
                });
            }).Otherwise(() =>
            {
                RuleFor(p => p.Group).Empty().WithMessage("Field '{PropertyName}' must be empty, if role is other then student!");
            });
        }
    }
}
