using System;
using SharedKernel.Infrastructure.Interfaces;

namespace SharedKernel.Infrastructure.Implementations
{
    public sealed class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}