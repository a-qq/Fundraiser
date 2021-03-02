using SharedKernel.Infrastructure.Interfaces;
using System;

namespace SharedKernel.Infrastructure.Implementations
{
    public sealed class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
