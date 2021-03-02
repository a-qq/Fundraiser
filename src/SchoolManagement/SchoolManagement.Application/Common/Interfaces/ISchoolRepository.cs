using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Schools;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface ISchoolRepository
    {
        Task<Maybe<School>> GetByIdAsync(SchoolId schoolId, CancellationToken cancellationToken = default,
            bool disableFilters = false);

        Task<Maybe<School>> GetByIdWithMembersAsync(SchoolId schoolId, CancellationToken cancellationToken = default,
            bool disableFilters = false);

        Task<Maybe<School>> GetByIdWithGroupsAsync(SchoolId schoolId, CancellationToken cancellationToken = default,
            bool disableFilters = false);

        void Add(School school);
        void Remove(School school);
    }
}