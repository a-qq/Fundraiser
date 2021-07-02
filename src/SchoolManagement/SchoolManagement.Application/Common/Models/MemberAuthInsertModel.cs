using SharedKernel.Domain.Constants;
using System;
using Gender = SharedKernel.Domain.Constants.Gender;

namespace SchoolManagement.Application.Common.Models
{
    internal sealed class MemberAuthInsertModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public SchoolRole Role { get; set; }
        public Gender Gender { get; set; }
        public Guid SchoolId { get; set; }
        public Guid? GroupId { get; set; }
        public string SecurityCode { get; set; }
    }
}