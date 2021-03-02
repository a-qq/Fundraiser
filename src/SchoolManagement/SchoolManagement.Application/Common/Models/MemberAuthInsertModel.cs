using SchoolManagement.Domain.SchoolAggregate.Members;
using System;

namespace SchoolManagement.Application.Common.Models
{
    internal sealed class MemberAuthInsertModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public RoleEnum Role { get; set; }
        public GenderEnum Gender { get; set; }
        public Guid SchoolId { get; set; }
        public Guid? GroupId { get; set; }
        public string SecurityCode { get; set; }
    }
}
