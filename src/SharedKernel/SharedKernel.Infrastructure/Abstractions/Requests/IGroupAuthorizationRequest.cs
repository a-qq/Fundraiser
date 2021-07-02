using System;

namespace SharedKernel.Infrastructure.Abstractions.Requests
{
    public interface IGroupAuthorizationRequest : ISchoolAuthorizationRequest
    {
        public Guid GroupId { get; }
    }
}