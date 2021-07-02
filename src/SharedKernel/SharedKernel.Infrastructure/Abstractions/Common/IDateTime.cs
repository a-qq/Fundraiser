using System;

namespace SharedKernel.Infrastructure.Abstractions.Common
{
    public interface IDateTime
    {
        DateTime Now { get; }
    }
}