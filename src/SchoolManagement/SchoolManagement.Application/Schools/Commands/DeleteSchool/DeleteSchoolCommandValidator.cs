using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.DeleteSchool
{
    internal sealed class DeleteSchoolCommandValidator : AbstractValidator<DeleteSchoolCommand>
    {
        public DeleteSchoolCommandValidator()
        {
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}