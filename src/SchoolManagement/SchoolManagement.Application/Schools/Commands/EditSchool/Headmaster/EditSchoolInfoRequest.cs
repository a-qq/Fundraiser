namespace SchoolManagement.Application.Schools.Commands.EditSchool.Headmaster
{
    public sealed class EditSchoolInfoRequest
    {
        public string Description { get; set; }
        public int? MaxNumberOfMembersInGroup { get; set; }
    }
}
