using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.MemberAggregate;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.FundraiserAggregate.Payments;

namespace FundraiserManagement.Infrastructure.Persistence.Repositories
{
    internal sealed class FundraiserRepository : IFundraiserRepository
    {
        private readonly DbSet<Fundraiser> _set;

        public FundraiserRepository(FundraiserContext context)
        {
            _set = context.Fundraisers;
        }

        public async Task<Maybe<Fundraiser>> GetByIdAsync(SchoolId schoolId,
            FundraiserId fundraiserId, CancellationToken token)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));
            Guard.Against.Default(fundraiserId, nameof(fundraiserId));

            return await _set.SingleOrDefaultAsync(f => f.Id == fundraiserId &&
                                                        f.SchoolId == schoolId, token);
        }

        public async Task<Maybe<Fundraiser>> GetByIdWithManagerAsync(SchoolId schoolId,
            FundraiserId fundraiserId, CancellationToken token = default)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));
            Guard.Against.Default(fundraiserId, nameof(fundraiserId));

            return await _set.Include(f => f.Manager)
                .ThenInclude(m => m.Card)
                .SingleOrDefaultAsync(f => f.Id == fundraiserId &&
                                                        f.SchoolId == schoolId, token);
        }

        public async Task<Maybe<Fundraiser>> GetByIdWithPaymentsAsync(
            SchoolId schoolId, FundraiserId fundraiserId, CancellationToken token)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));
            Guard.Against.Default(fundraiserId, nameof(fundraiserId));

            return await _set
                .Include(f => f.Manager)
                    .ThenInclude(f => f.Card)
                .Include(f => f.Participations)
                    .ThenInclude(f => f.Payments)
                .SingleOrDefaultAsync(f => f.Id == fundraiserId &&
                                           f.SchoolId == schoolId, token);
        }

        public async Task<Maybe<Fundraiser>> GetByIdWithParticipantsAsync(
            SchoolId schoolId, FundraiserId fundraiserId, CancellationToken token)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));
            Guard.Against.Default(fundraiserId, nameof(fundraiserId));

            return await _set
                .Include(f => f.Manager)
                    .ThenInclude(m => m.Card)
                .Include(f => f.Participations)
                    .ThenInclude(p => p.Payments)
                .Include(f => f.Participations)
                    .ThenInclude(p => p.Participant)
                .SingleOrDefaultAsync(f => f.Id == fundraiserId &&
                                           f.SchoolId == schoolId, token);
        }

        public async Task<IReadOnlyCollection<Fundraiser>> GetBySchoolIdAsync(SchoolId schoolId, CancellationToken token = default)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));

            return await _set
                .Include(f => f.Manager)
                    .ThenInclude(f => f.Card)
                .Include(f => f.Participations)
                    .ThenInclude(f => f.Payments)
                .Where(f => f.SchoolId == schoolId)
                .ToListAsync(token);
        }

        public async Task<IReadOnlyCollection<Fundraiser>> GetByManagerIdAsync(
            SchoolId schoolId, MemberId managerId, CancellationToken token)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));
            Guard.Against.Default(managerId, nameof(managerId));

            return await _set
                .Include(f => f.Manager)
                    .ThenInclude(m => m.Card)
                .Where(f => f.SchoolId == schoolId && f.Manager.Id == managerId)
                .ToListAsync(token);
        }

        public async Task<IReadOnlyCollection<Fundraiser>> GetByParticipantIdAsync(
            SchoolId schoolId, MemberId participantId, CancellationToken token)
        {
            Guard.Against.Default(schoolId, nameof(schoolId));
            Guard.Against.Default(participantId, nameof(participantId));

            return await _set
                .Include(f => f.Manager)
                    .ThenInclude(m => m.Card)
                .Include(f => f.Participations)
                    .ThenInclude(p => p.Participant)
                .Where(f => f.SchoolId == schoolId && f.Participations
                    .Any(p => p.Participant.Id == participantId))
                .ToListAsync(token);
        }

        public async Task<Maybe<Fundraiser>> GetByPaymentIdAsync(PaymentId paymentId, CancellationToken token = default)
        {
            Guard.Against.Default(paymentId, nameof(paymentId));

            return await _set
                .Include(f => f.Manager)
                    .ThenInclude(f => f.Card)
                .Include(f => f.Participations)
                    .ThenInclude(f => f.Payments)
                .SingleOrDefaultAsync(f => f.Participations.Any(
                    p => p.Payments.Any(pp => pp.Id == paymentId)), token);
        }

        public void Add(Fundraiser fundraising)
            => _set.Add(Guard.Against.Null(fundraising, nameof(fundraising)));
        
        public void Remove(Fundraiser fundraising)
            => _set.Remove(Guard.Against.Null(fundraising, nameof(fundraising)));
    }
}