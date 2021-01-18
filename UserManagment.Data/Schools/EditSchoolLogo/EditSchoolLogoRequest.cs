using Microsoft.AspNetCore.Http;

namespace SchoolManagement.Data.Schools.EditSchoolLogo
{
    public class EditSchoolLogoRequest
    {
        public IFormFile Logo { get; set; }
    }
}
