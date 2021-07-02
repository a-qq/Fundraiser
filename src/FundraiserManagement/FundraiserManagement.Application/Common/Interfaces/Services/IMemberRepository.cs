using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;

namespace FundraiserManagement.Application.Common.Interfaces.Services
{
    public interface IMemberRepository
    {
        Task<Maybe<Member>> GetByIdAsync(MemberId memberId, SchoolId schoolId, CancellationToken token = default);
        Task<IReadOnlyCollection<Member>> GetByIdsAsync(IReadOnlyCollection<MemberId> memberIds, SchoolId schoolId,
            CancellationToken token = default);
        Task<Maybe<Member>> GetByIdAsync(MemberId memberId, CancellationToken token = default);
        Task<IReadOnlyCollection<Member>> GetByIdsAsync(IReadOnlyCollection<MemberId> memberIds, CancellationToken token = default);
        Task<IReadOnlyCollection<Member>> GetBySchoolIdAsync(SchoolId schoolId, CancellationToken token = default);
        void Add(Member member);
        void Remove(Member member);
    }
}
