using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.FundraiserAggregate.Payments;
using FundraiserManagement.Domain.MemberAggregate;

namespace FundraiserManagement.Application.Common.Interfaces.Services
{
    public interface IFundraiserRepository
    {
        Task<Maybe<Fundraiser>> GetByIdAsync(SchoolId schoolId, FundraiserId fundraiserId, CancellationToken token = default);
        Task<Maybe<Fundraiser>> GetByIdWithManagerAsync(SchoolId schoolId, FundraiserId fundraiserId, CancellationToken token = default);
        Task<Maybe<Fundraiser>> GetByIdWithPaymentsAsync(SchoolId schoolId, FundraiserId fundraiserId, CancellationToken token = default);
        Task<Maybe<Fundraiser>> GetByIdWithParticipantsAsync(SchoolId schoolId, FundraiserId fundraiserId, CancellationToken token = default);
        Task<IReadOnlyCollection<Fundraiser>> GetBySchoolIdAsync(SchoolId schoolId, CancellationToken token = default);
        Task<IReadOnlyCollection<Fundraiser>> GetByManagerIdAsync(SchoolId schoolId, MemberId managerId, CancellationToken token = default);
        Task<IReadOnlyCollection<Fundraiser>> GetByParticipantIdAsync(SchoolId schoolId, MemberId participantId, CancellationToken token = default);
        Task<Maybe<Fundraiser>> GetByPaymentIdAsync(PaymentId paymentId, CancellationToken token = default);
        void Add(Fundraiser member);
        void Remove(Fundraiser member);
    }
}