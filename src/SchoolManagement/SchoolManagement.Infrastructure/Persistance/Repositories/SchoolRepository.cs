using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure.Persistance.Repositories
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

            var schoolOrNone = Maybe<School>.None;
            if (disableFilters)
            {
                schoolOrNone = await _context.Schools
                        .IgnoreQueryFilters()
                        .Include(c => c.Members)
                        .SingleOrDefaultAsync(s => s.Id == schoolId);
            }
            else
            {
                schoolOrNone = await GetByIdAsync(schoolId, cancellationToken, disableFilters);

                if (schoolOrNone.HasValue)
                    await _context.Entry(schoolOrNone.Value).Collection(s => s.Members).LoadAsync(cancellationToken);

            }
            return schoolOrNone;
        }

        public async Task<Maybe<School>> GetByIdWithGroupsAsync(
            SchoolId schoolId, CancellationToken cancellationToken = default, bool disableFilters = false)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));

            var schoolOrNone = Maybe<School>.None;
            if (disableFilters)
            {
                schoolOrNone = await _context.Schools
                        .IgnoreQueryFilters()
                        .Include(c => c.Groups)
                        .SingleOrDefaultAsync(s => s.Id == schoolId);
            }
            else
            {
                schoolOrNone = await GetByIdAsync(schoolId, cancellationToken, disableFilters);

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
