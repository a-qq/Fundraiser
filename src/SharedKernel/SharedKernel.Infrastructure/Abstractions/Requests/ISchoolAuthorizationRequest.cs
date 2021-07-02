using System;

namespace SharedKernel.Infrastructure.Abstractions.Requests
{
    public interface ISchoolAuthorizationRequest
    {
        public Guid SchoolId { get; }
    }
}