using SharedKernel.Domain.ValueObjects;

namespace SchoolManagement.Domain.SchoolAggregate.Members
{
    public sealed class MemberEnrollmentData
    {
        public MemberEnrollmentData(FirstName firstName, LastName lastName, Email email, Role role, Gender gender)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            Gender = gender;
        }

        public FirstName FirstName { get; }
        public LastName LastName { get; }
        public Email Email { get; }
        public Role Role { get; }
        public Gender Gender { get; }
    }
}