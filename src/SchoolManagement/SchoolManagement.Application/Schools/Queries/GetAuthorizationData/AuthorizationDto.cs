using SharedKernel.Domain.Constants;

namespace SchoolManagement.Application.Schools.Queries.GetAuthorizationData
{
    public sealed class AuthorizationDto
    {
        public SchoolRole Role { get; private set; }

        //public GenderEnum Gender { get; private set; }
        public long? GroupId { get; private set; }
        public bool IsTreasurer { get; private set; }
    }
}