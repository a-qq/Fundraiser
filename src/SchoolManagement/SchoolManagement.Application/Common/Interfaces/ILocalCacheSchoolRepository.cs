using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface ILocalCacheSchoolRepository
    {
        ValueTask<Maybe<School>> GetByIdAsync(SchoolId schoolId, CancellationToken token = default);


        ValueTask<Maybe<School>> GetByIdWithMembersAsync(SchoolId schoolId, CancellationToken token = default);


        ValueTask<Maybe<School>> GetByIdWithGroupsAsync(SchoolId schoolId, CancellationToken token = default);
    }
}