using System;

namespace SharedKernel.Domain.Common
{
    public interface ITypedId
    {
        public Guid Value { get; }
    }
}