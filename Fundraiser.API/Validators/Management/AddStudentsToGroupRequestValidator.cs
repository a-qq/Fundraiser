using FluentValidation;
using SchoolManagement.Data.Schools.AddStudentsToGroup;
using System.Collections.Generic;
using System.Linq;

namespace Fundraiser.API.Validators.Management
{
    public class AddStudentsToGroupRequestValidator : AbstractValidator<AddStudentsToGroupRequest>
    {
        public AddStudentsToGroupRequestValidator()
        {
            RuleFor(p => p.StudentIds).NotNull().NotEmpty().Must(p => p.Count() == p.Distinct().Count()).WithMessage("{PropertyName} cannot contain duplicates!");
            RuleForEach(p => p.StudentIds).NotEmpty();
        }
    }
}
