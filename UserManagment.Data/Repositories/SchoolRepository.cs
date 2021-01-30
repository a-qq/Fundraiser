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
using Fundraiser.SharedKernel.Utils;

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

            var memberOrNone = Maybe<Member>.From(_cache.Get<Member>(SchemaNames.Management + memberId));

            if (memberOrNone.HasNoValue)
            {
                memberOrNone = Maybe<Member>.From(
                      await _dbSet
                          .Where(s => s.Id == schoolId)
                          .SelectMany(s => s.Members)
                          .Include(m => m.School) // without this context won't cache school to retrive with find
                          .FirstOrDefaultAsync(m => m.Id == memberId));
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
                .Include(u => u.Group)
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<Maybe<Group>> GetGroupWithStudentsByIdAsync(Guid schoolId, long groupId)
        {
            if (schoolId == Guid.Empty)
                throw new ArgumentNullException(nameof(schoolId));

            if (groupId < 1)
                throw new ArgumentOutOfRangeException(nameof(groupId));

            return Maybe<Group>.From(
                      await _dbSet
                          .Where(s => s.Id == schoolId)
                          .SelectMany(s => s.Groups)
                          .Include(g => g.Students)
                          .FirstOrDefaultAsync(g => g.Id == groupId));
        }

        public async Task<Maybe<Group>> GetGroupWithFormTutorByIdAsync(Guid schoolId, long groupId)
        {
            if (schoolId == Guid.Empty)
                throw new ArgumentNullException(nameof(schoolId));

            if (groupId < 1)
                throw new ArgumentOutOfRangeException(nameof(groupId));

            return Maybe<Group>.From(
                      await _dbSet
                          .Where(s => s.Id == schoolId)
                          .SelectMany(s => s.Groups)
                          .Include(g => g.FormTutor)
                          .FirstOrDefaultAsync(g => g.Id == groupId));
        }

        public async Task<Maybe<School>> GetSchoolWithGroupAndFormTutors(Guid schoolId)
        {
            if (schoolId == Guid.Empty)
                throw new ArgumentNullException(nameof(schoolId));

            return Maybe<School>.From(
                      await _dbSet
                          .Include(s => s.Groups)
                            .ThenInclude(g => g.FormTutor)
                          .FirstOrDefaultAsync(s => s.Id == schoolId));
        }

        public void Remove(School school)
        {
            if (school == null)
                throw new ArgumentNullException(nameof(school));

            _dbSet.Remove(school);
        }
    }
}
