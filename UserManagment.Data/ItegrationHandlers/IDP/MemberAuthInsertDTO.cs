using SchoolManagement.Core.SchoolAggregate.Members;
using System;

namespace SchoolManagement.Data.ItegrationHandlers.IDP
{
    public sealed class MemberAuthInsertDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public RoleEnum Role { get; set; }
        public GenderEnum Gender { get; set; }
        public Guid SchoolId { get; set; }
        public string SecurityCode { get; set; } 
    }
}
