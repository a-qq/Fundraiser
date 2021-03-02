using Microsoft.AspNetCore.Http;

namespace SchoolManagement.Application.Schools.Commands.EnrollMembersFromCsv
{
    public sealed class EnrollMembersFromCsvRequest
    {
        public IFormFile File { get; set; }
        public DelimiterEnum Delimiter { get; set; }
    }
}