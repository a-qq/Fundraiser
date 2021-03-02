using Ardalis.GuardClauses;
using SharedKernel.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedKernel.Domain.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        public static IEnumerable<T> GuardEachAgainstDefault<T>(this IEnumerable<T> items, string parameterName)
            where T : struct
        {
            _ = Guard.Against.NullOrWhiteSpace(parameterName, nameof(parameterName));

            foreach (var item in items)
                _ = Guard.Against.Default(item, parameterName);

            return items;
        }

        public static IEnumerable<Guid> ConvertToGuid<T>(this IEnumerable<T> items)
            where T : ITypedId
        {
            return items.Select(x => x.Value);
        }
    }
}