using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using SchoolManagement.Data.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Repositories
{
    internal sealed class SchoolRepository : ISchoolRepository
    {
        private readonly DbSet<School> _dbSet;
        private readonly IMemoryCache _cache;

        public SchoolRepository(
            SchoolContext schoolContext,
            IMemoryCache memoryCache)
        {
            _dbSet = schoolContext.Schools;
            _cache = memoryCache;
        }
        public async Task<Maybe<School>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));

            return Maybe<School>.From(await _dbSet.FindAsync(id));
        }

        public async Task<bool> ExistByIdAsync(Guid id)
        {
            return (await GetByIdAsync(id)).HasValue;
        }

        public async Task<Maybe<User>> GetSchoolMemberByIdAsync(Guid id, Guid memberId)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));

            if (memberId == Guid.Empty)
                throw new ArgumentNullException(nameof(memberId));

            var userOrNone = Maybe<User>.From(_cache.Get<User>(memberId));
            if (userOrNone.HasNoValue)
            {
                userOrNone = Maybe<User>.From(
                      await _dbSet
                          .Where(s => s.Id == id)
                          .SelectMany(s => s.Members)
                          .Include(u => u.School)
                          .FirstOrDefaultAsync(m => m.Id == memberId));

                if (userOrNone.HasValue)
                    _cache.Set(memberId, userOrNone.Value, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(new TimeSpan(0, 0, 5))
                        .SetSlidingExpiration(new TimeSpan(0, 0, 3)));
            }

            return userOrNone;
        }

        public void Add(School school)
        {
            if (school == null)
                throw new ArgumentNullException(nameof(school));
            _dbSet.Add(school);
        }

    }
}
