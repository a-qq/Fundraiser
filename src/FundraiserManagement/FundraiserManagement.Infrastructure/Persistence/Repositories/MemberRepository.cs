using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Services;

namespace FundraiserManagement.Infrastructure.Persistence.Repositories
{
    internal sealed class MemberRepository : IMemberRepository
    {
        private readonly DbSet<Member> _members;

        public MemberRepository(FundraiserContext context)
        {
            _members = context.Members;
        }
        
        public async Task<Maybe<Member>> GetByIdAsync(MemberId memberId,
            SchoolId schoolId, CancellationToken token = default)
        {
            Guard.Against.Default(memberId, nameof(memberId));
            Guard.Against.Default(schoolId, nameof(schoolId));

            return await _members.SingleOrDefaultAsync(
                m => m.Id == memberId && m.SchoolId == schoolId, token);
        }

        public async Task<IReadOnlyCollection<Member>> GetByIdsAsync(IReadOnlyCollection<MemberId> memberIds,
            SchoolId schoolId, CancellationToken token = default)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));
            Guard.Against.NullOrEmpty(memberIds, nameof(memberIds));
            int i = 0;
            foreach (var memberId in memberIds)
            {
                Guard.Against.Default(memberId, $"{nameof(memberIds)}[{i}]");
                i++;
            }

            return await _members.Where(m => m.SchoolId == schoolId && memberIds.Contains(m.Id))
                .ToListAsync(token);
        }

        public async Task<Maybe<Member>> GetByIdAsync(MemberId memberId, CancellationToken token = default)
        {
            Guard.Against.Default(memberId, nameof(memberId));

            return await _members.SingleOrDefaultAsync(
                m => m.Id == memberId, token);
        }

        public async Task<IReadOnlyCollection<Member>> GetByIdsAsync(IReadOnlyCollection<MemberId> memberIds, CancellationToken token = default)
        {
            Guard.Against.NullOrEmpty(memberIds, nameof(memberIds));
            int i = 0;
            foreach (var memberId in memberIds)
            {
                Guard.Against.Default(memberId, $"{nameof(memberIds)}[{i}]");
                i++;
            }

            return await _members.Where(m => memberIds.Contains(m.Id))
                .ToListAsync(token);
        }

        public async Task<IReadOnlyCollection<Member>> GetBySchoolIdAsync(SchoolId schoolId, CancellationToken token = default)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));

            return await _members.Where(m => m.SchoolId == schoolId)
                .ToListAsync(token);
        }

        public void Add(Member member)
            => _members.Add(Guard.Against.Null(member, nameof(member)));

        public void Remove(Member member)
            => _members.Remove(Guard.Against.Null(member, nameof(member)));
    }
}
