using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.ValueObjects;

namespace SchoolManagement.Domain.Common.Models
{
    public class MemberEnrollmentAssignmentData
    {
        public FirstName FirstName { get; set; }
        public LastName LastName { get; set; }
        public Email Email { get; set; }
        public Role Role { get; set; }
        public Gender Gender { get; set; }
        public Maybe<Code> GroupCode { get; set; }
    }
}