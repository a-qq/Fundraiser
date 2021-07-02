using CSharpFunctionalExtensions;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using SharedKernel.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Common.Interfaces.Services
{
    public interface IPaymentGateway
    {
        Task<Result<string>> CreateAccountAsync(string idempotencyKey, CancellationToken token = default, Email email = null);
        Task<Result> DeleteAccount(string accountId, string idempotencyKey, CancellationToken token);
        Task<Result> MakeATransfer(string sourceAccountId, string targetAccountId, decimal amount,
            string idempotencyKey, Name name, FundraiserId fundraiserId, CancellationToken token);
    }
}