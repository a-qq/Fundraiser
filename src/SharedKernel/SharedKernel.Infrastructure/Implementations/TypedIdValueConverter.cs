using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharedKernel.Domain.Common;
using System;

namespace SharedKernel.Infrastructure.Implementations
{
    public class TypedIdValueConverter<TTypedIdValue> : ValueConverter<TTypedIdValue, Guid>
        where TTypedIdValue : ITypedId
    {
        public TypedIdValueConverter(ConverterMappingHints mappingHints = null)
            : base(id => id.Value, value => Create(value), mappingHints)
        {
        }

        private static TTypedIdValue Create(Guid id) => (TTypedIdValue)Activator.CreateInstance(typeof(TTypedIdValue), id);
    }
}
