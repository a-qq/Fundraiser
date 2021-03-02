using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Infrastructure.Extensions
{
    public static class DbSetExtensions
    {
        public static async Task<T> FindAsync<T, TKey>(this DbSet<T> source, TKey id,
            CancellationToken cancellationToken = default, bool disableFilters = false)
            where T : Entity<TKey>
            where TKey : IEquatable<TKey>
        {
            if (disableFilters)
                //return source.Local.SingleOrDefault(e => e.Id.Equals(id)) ?? 
                return await source.IgnoreQueryFilters().SingleOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);

            return await source.FindAsync(new object[] {id}, cancellationToken);
        }
    }
}