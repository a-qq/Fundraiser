using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.SchoolAggregate;
using FundraiserManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Infrastructure.Services
{
    internal sealed class SchoolRepository : ISchoolRepository
    {
        private readonly DbSet<School> _schools;

        public SchoolRepository(FundraiserContext context)
        {
            _schools = context.Schools;
        }

        public async Task<bool> ExistByIdAsync(SchoolId schoolId, CancellationToken token)
            => (await GetByIdAsync(schoolId, token)).HasValue;

        public async Task<Maybe<School>> GetByIdAsync(SchoolId schoolId, CancellationToken token)
        => await _schools.FindAsync(new[]
        {
                Guard.Against.Default(schoolId, nameof(schoolId))
        }, token);

        public void Add(School school)
            => _schools.Add(Guard.Against.Null(school, nameof(school)));

    }
}