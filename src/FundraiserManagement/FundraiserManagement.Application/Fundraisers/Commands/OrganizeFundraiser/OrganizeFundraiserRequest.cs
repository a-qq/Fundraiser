using System;
using Range = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.Range;
using Type = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.Type;

namespace FundraiserManagement.Application.Fundraisers.Commands.OrganizeFundraiser
{
    public class OrganizeFundraiserRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? GroupId { get; set; }
        public Range Range { get; set; }
        public Type Type { get; set; }
        public decimal Goal { get; set; }
        public bool IsShared { get; set; }
        public Guid ManagerId { get; set; }
    }
}
