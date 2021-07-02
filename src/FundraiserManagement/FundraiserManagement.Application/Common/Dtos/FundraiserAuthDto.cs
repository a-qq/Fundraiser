using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using FMD = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;

namespace FundraiserManagement.Application.Common.Dtos
{
    internal class FundraiserAuthDto
    {
        public FMD.Type Type { get; private set; }
        public GroupId? GroupId { get; private set; }
        public MemberId? ManagerId { get; private set; }
        public SchoolId SchoolId { get; private set; }
        public FMD.Range Range { get; private set; }
    }
}