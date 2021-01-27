using Microsoft.AspNetCore.Http;

namespace SchoolManagement.Data.Schools.EnrollMembersFromCsv
{
    public sealed class EnrollMembersFromCsvRequest
    {
        public IFormFile File { get; set; }
        public DelimiterEnum Delimiter { get; set; }
    }
}
