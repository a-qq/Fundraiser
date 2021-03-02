using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.EditSchoolLogo
{
    public class EditSchoolLogoCommandValidator : AbstractValidator<EditSchoolLogoCommand>
    {
        public EditSchoolLogoCommandValidator()
        {
            RuleFor(p => p.Logo).NotEmpty().WithMessage("{PropertyName} is required!")
                .Must(p => p.Length < 10485760).WithMessage("{PropertyName} must be under 10 MB!")
                .Must(p => p.ContentType.StartsWith("image/")).WithMessage("{PropertyName} must be an image!")
                .Must(p => p.ContentType.EndsWith("/png") || p.ContentType.EndsWith("/jpeg")
                    || p.ContentType.EndsWith("/bmp") || p.ContentType.EndsWith("/tga")).WithMessage("{PropertyName} must be in PNG, JPEG, BMP or TGA format!");
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}
