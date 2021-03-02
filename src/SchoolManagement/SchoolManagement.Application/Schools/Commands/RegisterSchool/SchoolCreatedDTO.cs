using System;

namespace SchoolManagement.Application.Schools.Commands.RegisterSchool
{
    public sealed class SchoolCreatedDTO
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public int YearsOfEducation { get; private set; }
        public MemberDTO Headmaster { get; private set; }
    }
}
