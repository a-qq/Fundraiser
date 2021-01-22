namespace SchoolManagement.Data.Schools.Commands.RegisterSchool
{
    public sealed class RegisterSchoolRequest
    {
        public string Name { get; set; }
        public int YearsOfEducation { get; set; }
        public string HeadmasterFirstName { get; set; }
        public string HeadmasterLastName { get; set; }
        public string HeadmasterEmail { get; set; }
        public string HeadmasterGender { get; set; }
    }
}
