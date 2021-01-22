using FluentValidation;
using SchoolManagement.Data.Schools.MakeTeacherFormTutor;

namespace Fundraiser.API.Validators.Management
{
    public sealed class MakeTeacherFormTutorRequestValidator : AbstractValidator<MakeTeacherFormTutorRequest>
    {
        public MakeTeacherFormTutorRequestValidator()
        {
            RuleFor(p => p.TeacherId).NotEmpty();
        }
    }
}
