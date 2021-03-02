using SchoolManagement.Domain.SchoolAggregate.Members;

namespace SchoolManagement.Application.Schools.Queries.GetMember
{
    public sealed class AuthorizationDto
    {
        public RoleEnum Role { get; private set; }
        //public GenderEnum Gender { get; private set; }
        public long? GroupId { get; private set; }
        public bool IsTreasurer { get; private set; }
    }
}
