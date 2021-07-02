using Ardalis.GuardClauses;
using FundraiserManagement.Domain.Common.Models;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.SchoolAggregate
{
    public class School : AggregateRoot<SchoolId>
    {
        public string AccountId { get; }

        public School(SchoolId id, string accountId)
            : base(Guard.Against.Default(id, nameof(id)))
        {
            AccountId = Guard.Against.Null(accountId, nameof(accountId)); ;
        }
    }
}