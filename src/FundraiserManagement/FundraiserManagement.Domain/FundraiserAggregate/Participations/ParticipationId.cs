using System;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.FundraiserAggregate.Participations
{
    [StronglyTypedId]
    public partial struct ParticipationId : ITypedId
    {
        public static implicit operator Guid(ParticipationId participationId)
        {
            return participationId.Value;
        }
    }
}
