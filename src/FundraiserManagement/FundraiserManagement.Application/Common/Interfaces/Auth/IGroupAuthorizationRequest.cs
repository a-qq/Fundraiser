using System;
using SharedKernel.Infrastructure.Abstractions.Requests;

namespace FundraiserManagement.Application.Common.Interfaces.Auth
{
    internal interface IGroupAuthorizationRequest : ISchoolAuthorizationRequest
    {
        public Guid? GroupId { get; }
    }
}
