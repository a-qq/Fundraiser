using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure.Persistence.Repositories
{
    internal sealed class LocalCacheSchoolRepository : ILocalCacheSchoolRepository
    {
        private readonly LocalView<School> _schools;
        private readonly ISchoolRepository _schoolRepository;

        public LocalCacheSchoolRepository(
            SchoolContext schoolContext,
            ISchoolRepository schoolRepository)
        {
            _schools = schoolContext.Schools.Local;
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
        }


        public async ValueTask<Maybe<School>> GetByIdAsync(SchoolId schoolId, CancellationToken token = default)
        {
            var school = GetById(schoolId);
            return school ?? await _schoolRepository.GetByIdAsync(schoolId, token);
        }

        public async ValueTask<Maybe<School>> GetByIdWithMembersAsync(SchoolId schoolId, CancellationToken token = default)
        {
            var school = GetById(schoolId);
            return school ?? await _schoolRepository.GetByIdWithMembersAsync(schoolId, token);
        }

        public async ValueTask<Maybe<School>> GetByIdWithGroupsAsync(SchoolId schoolId, CancellationToken token = default)
        {
            var school = GetById(schoolId);
            return school ?? await _schoolRepository.GetByIdWithGroupsAsync(schoolId, token);
        }

        private School GetById(SchoolId schoolId)
            => _schools.SingleOrDefault(s => s.Id == schoolId);
    }
}