using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure.Persistence.Repositories
{
    internal sealed class SchoolRepository : ISchoolRepository
    {
        private readonly SchoolContext _context;

        public SchoolRepository(SchoolContext context)
        {
            _context = context;
        }

        public async Task<Maybe<School>> GetByIdAsync(
            SchoolId schoolId, CancellationToken cancellationToken = default, bool disableFilters = false)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));

            return await _context.Schools.FindAsync<School, SchoolId>(schoolId, cancellationToken, disableFilters);
        }

        public async Task<Maybe<School>> GetByIdWithMembersAsync(
            SchoolId schoolId, CancellationToken cancellationToken = default, bool disableFilters = false)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));

            Maybe<School> schoolOrNone;
            if (disableFilters)
            {
                schoolOrNone = await _context.Schools
                    .IgnoreQueryFilters()
                    .Include(c => c.Members)
                    .SingleOrDefaultAsync(s => s.Id == schoolId, cancellationToken);
            }
            else
            {
                schoolOrNone = await GetByIdAsync(schoolId, cancellationToken);

                if (schoolOrNone.HasValue)
                    await _context.Entry(schoolOrNone.Value).Collection(s => s.Members).LoadAsync(cancellationToken);
            }

            return schoolOrNone;
        }

        public async Task<Maybe<School>> GetByIdWithGroupsAsync(
            SchoolId schoolId, CancellationToken cancellationToken = default, bool disableFilters = false)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));

            Maybe<School> schoolOrNone;
            if (disableFilters)
            {
                //includes are necessary as ignore query filters do not apply for further lazy loading
                schoolOrNone = await _context.Schools
                    .IgnoreQueryFilters()
                    .Include(c => c.Groups)
                        .ThenInclude(c => c.Students)
                    .Include(c => c.Groups)
                        .ThenInclude(c => c.FormTutor)
                    .SingleOrDefaultAsync(s => s.Id == schoolId, cancellationToken);
            }
            else
            {
                schoolOrNone = await GetByIdAsync(schoolId, cancellationToken);

                if (schoolOrNone.HasValue)
                    await _context.Entry(schoolOrNone.Value).Collection(s => s.Groups).LoadAsync(cancellationToken);
            }

            return schoolOrNone;
        }

        public void Add(School school)
        {
            Guard.Against.Null(school, nameof(school));

            _context.Schools.Add(school);
        }

        public void Remove(School school)
        {
            Guard.Against.Null(school, nameof(school));

            _context.Schools.Remove(school);
        }
    }
}