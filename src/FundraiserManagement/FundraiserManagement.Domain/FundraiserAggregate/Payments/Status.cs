namespace FundraiserManagement.Domain.FundraiserAggregate.Payments
{
    public enum Status
    {
        Processing = 1,
        Succeeded = 2,
        Failed = 3,
        Cancelled = 4
    }
}