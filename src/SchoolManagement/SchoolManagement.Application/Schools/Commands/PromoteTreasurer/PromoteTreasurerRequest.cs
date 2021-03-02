using System;

namespace SchoolManagement.Application.Schools.Commands.PromoteTreasurer
{
    public sealed class PromoteTreasurerRequest
    {
        public Guid StudentId { get; set; }
    }
}