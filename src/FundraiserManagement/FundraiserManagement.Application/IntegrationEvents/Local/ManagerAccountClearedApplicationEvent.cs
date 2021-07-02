using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using SharedKernel.Infrastructure.Concretes.Models;

namespace FundraiserManagement.Application.IntegrationEvents.Local
{
    internal sealed class ManagerAccountClearedApplicationEvent : IntegrationEvent
    {
        public SchoolId SchoolId { get; }
        public MemberId ManagerId { get; }

        public ManagerAccountClearedApplicationEvent(
            SchoolId schoolId, MemberId managerId)
        {
            SchoolId = schoolId;
            ManagerId = managerId;
        }
    }
}