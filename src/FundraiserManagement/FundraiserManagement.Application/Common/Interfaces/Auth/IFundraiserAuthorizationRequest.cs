using System;
using SharedKernel.Infrastructure.Abstractions.Requests;

namespace FundraiserManagement.Application.Common.Interfaces.Auth
{
    public interface IFundraiserAuthorizationRequest : ISchoolAuthorizationRequest
    {
        public Guid FundraiserId { get; }
    }
}
