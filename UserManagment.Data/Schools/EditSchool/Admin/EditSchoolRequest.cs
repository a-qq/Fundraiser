using Microsoft.AspNetCore.Http;

namespace SchoolManagement.Data.Schools.EditSchool.Admin
{
    public class EditSchoolRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
