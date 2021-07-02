using System;
using SharedKernel.Infrastructure.Abstractions.Common;

namespace SharedKernel.Infrastructure.Concretes.Services
{
    public sealed class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}