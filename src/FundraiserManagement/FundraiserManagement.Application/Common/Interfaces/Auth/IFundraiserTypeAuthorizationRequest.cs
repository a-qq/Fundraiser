using FMD = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;

namespace FundraiserManagement.Application.Common.Interfaces.Auth
{
    internal interface IFundraiserTypeAuthorizationRequest : IGroupAuthorizationRequest
    {
        public FMD.Type Type { get; }
        public FMD.Range Range { get; }
    }
}