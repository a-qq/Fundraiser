namespace SchoolManagement.Application.Schools.Commands.EditSchool.Admin
{
    public sealed class EditSchoolRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? MaxNumberOfMembersInGroup { get; set; }
    }
}
