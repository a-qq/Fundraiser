using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Members;

namespace SchoolManagement.Data.Schools.EnrollMembersFromCsv
{
    public class MemberFromCsvModel
    {
        public FirstName FirstName { get; set; }
        public LastName LastName { get; set; }
        public Email Email { get; set; }
        public Role Role { get; set; }
        public Gender Gender { get; set; }
        public Maybe<Number> GroupNumber { get; set; }
        public Maybe<Sign> GroupSign { get; set; }
        protected MemberFromCsvModel() { }
    }
}
