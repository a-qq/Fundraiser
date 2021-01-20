using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Data.Database;
using System;
using System.Collections.Generic;
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

        public async Task<Maybe<Member>> GetSchoolMemberByIdAsync(Guid schoolId, Guid memberId)
        {
            if (schoolId == Guid.Empty)
                throw new ArgumentNullException(nameof(schoolId));

            if (memberId == Guid.Empty)
                throw new ArgumentNullException(nameof(memberId));

            var memberOrNone = Maybe<Member>.From(_cache.Get<Member>(memberId));
            if (memberOrNone.HasNoValue)
            {
                memberOrNone = Maybe<Member>.From(
                      await _dbSet
                          .Where(s => s.Id == schoolId)
                          .SelectMany(s => s.Members)
                          .Include(u => u.School)
                          .FirstOrDefaultAsync(m => m.Id == memberId));

                if (memberOrNone.HasValue)
                    _cache.Set(memberId, memberOrNone.Value, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(new TimeSpan(0, 0, 5))
                        .SetSlidingExpiration(new TimeSpan(0, 0, 3)));
            }

            return memberOrNone;
        }

        public void Add(School school)
        {
            if (school == null)
                throw new ArgumentNullException(nameof(school));
            _dbSet.Add(school);
        }

        public async Task<List<Member>> GetSchoolMembersByIdAsync(Guid schoolId, IEnumerable<Guid> userIds)
        {
            if (schoolId == Guid.Empty)
                throw new ArgumentNullException(nameof(schoolId));

            if (userIds == null || !userIds.Any())
                throw new ArgumentNullException(nameof(userIds));

            return await _dbSet
                .Where(s => s.Id == schoolId)
                .SelectMany(s => s.Members)
                .Include(u => u.School)
                .Include(u => u.Group)
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<Maybe<Group>> GetGroupByIdAsync(Guid schoolId, long groupId)
        {
            if (schoolId == Guid.Empty)
                throw new ArgumentNullException(nameof(schoolId));

            if (groupId < 1)
                throw new ArgumentOutOfRangeException(nameof(groupId));

            return Maybe<Group>.From(
                      await _dbSet
                          .Where(s => s.Id == schoolId)
                          .SelectMany(s => s.Groups)
                          .Include(g => g.School)
                          .Include(g => g.Members)
                          .FirstOrDefaultAsync(g => g.Id == groupId));
        }
    }
}
