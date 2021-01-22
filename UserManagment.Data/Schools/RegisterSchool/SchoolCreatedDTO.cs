using System;

namespace SchoolManagement.Data.Schools.RegisterSchool
{
    public sealed class SchoolCreatedDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int YearsOfEdcuation { get; set; }
        public MemberDTO Headmaster { get; set; }
    }
}
