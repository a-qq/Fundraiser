using Microsoft.AspNetCore.Http;

namespace SchoolManagement.Application.Schools.Commands.EditSchoolLogo
{
    public sealed class EditSchoolLogoRequest
    {
        public IFormFile Logo { get; set; }
    }
}
