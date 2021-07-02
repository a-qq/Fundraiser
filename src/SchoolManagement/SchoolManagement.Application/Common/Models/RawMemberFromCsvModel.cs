namespace SchoolManagement.Application.Common.Models
{
    internal class RawMemberFromCsvModel
    {
        protected RawMemberFromCsvModel()
        {
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
        public string Group { get; set; }
        public int RowNumber { get; set; }
    }
}