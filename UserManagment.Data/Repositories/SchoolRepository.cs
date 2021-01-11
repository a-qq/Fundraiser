using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
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
        public SchoolRepository(SchoolContext schoolContext)
        {
            _dbSet = schoolContext.Schools;
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

            return Maybe<User>.From(
                await _dbSet.Include(m => m.Members)
                    .Where(s => s.Id == id)
                    .SelectMany(s => s.Members)
                    .SingleOrDefaultAsync(m => m.Id == memberId));
        }

        public void Add(School school)
        {
            if (school == null)
                throw new ArgumentNullException(nameof(school));
            _dbSet.Add(school);
        }
    }
}
