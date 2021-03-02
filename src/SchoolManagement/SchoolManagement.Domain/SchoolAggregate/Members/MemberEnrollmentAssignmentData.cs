using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SharedKernel.Domain.ValueObjects;

namespace SchoolManagement.Domain.SchoolAggregate.Members
{
    public sealed class MemberEnrollmentAssignmentData
    {
        public FirstName FirstName { get; private set; }
        public LastName LastName { get; private set; }
        public Email Email { get; private set; }
        public Role Role { get; private set; }
        public Gender Gender { get; private set; }
        public Maybe<Code> GroupCode { get; private set; }
    }
}