using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharedKernel.Domain.Common;

namespace SharedKernel.Infrastructure.Implementations
{
    public class TypedIdValueConverter<TTypedIdValue> : ValueConverter<TTypedIdValue, Guid>
        where TTypedIdValue : ITypedId
    {
        public TypedIdValueConverter(ConverterMappingHints mappingHints = null)
            : base(id => id.Value, value => Create(value), mappingHints)
        {
        }

        private static TTypedIdValue Create(Guid id)
        {
            return (TTypedIdValue) Activator.CreateInstance(typeof(TTypedIdValue), id);
        }
    }
}