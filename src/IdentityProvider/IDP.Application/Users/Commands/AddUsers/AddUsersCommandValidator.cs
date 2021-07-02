using FluentValidation;
using IDP.Application.Common.Validation;
using SharedKernel.Infrastructure.Extensions;

namespace IDP.Application.Users.Commands.AddUsers
{
    internal sealed class AddUsersCommandValidator : AbstractValidator<AddUsersCommand>
    {
        public AddUsersCommandValidator()
        {
            RuleFor(p => p.HoursToExpire).HoursToExpireMustBeValid();
            RuleForEach(p => p.Users).ChildRules(users =>
            {
                users.RuleFor(x => x.Subject).SubjectMustBeValid();
                users.RuleFor(p => p.Email).EmailMustBeValid();
                users.RuleForEach(p => p.Claims).NotNull().SetValidator(new ClaimInsertModelValidator());
            });
        }
    }
}