using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Application.Common.Models;
using SharedKernel.Domain.ValueObjects;
using Stripe;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.FundraiserAggregate.Payments;

namespace FundraiserManagement.Infrastructure.Services
{
    internal sealed class PaymentGateway : IPaymentGateway
    {
        private readonly StripeClient _client;

        public PaymentGateway(StripeOptions stripeOptions)
        {
            _client = new StripeClient(stripeOptions.SecretKey);
        }
        

        public async Task<Result<string>> CreateAccountAsync(string idempotencyKey, CancellationToken token = default, Email email = null)
        {
            var service = new AccountService(_client);
            var accountCreateOptions = new AccountCreateOptions
            {
                Type = "express",
                Email = email,
                DefaultCurrency = "PLN"
            };

            var requestOptions = new RequestOptions
            {
                IdempotencyKey = Guard.Against.NullOrWhiteSpace(idempotencyKey, nameof(idempotencyKey))
            };

            Account account;
            try
            {
                account = await service.CreateAsync(accountCreateOptions, requestOptions, token);
            }
            catch (StripeException ex)
            {
                return Result.Failure<string>(ex.Message);
            }

            return account.Id;
        }

        public async Task<Result> DeleteAccount(string accountId, string idempotencyKey, CancellationToken token)
        {
            Guard.Against.NullOrWhiteSpace(accountId, nameof(accountId));

            var requestOptions = new RequestOptions
            {
                IdempotencyKey = Guard.Against.NullOrWhiteSpace(idempotencyKey, nameof(idempotencyKey))
            };

            var service = new AccountService(_client);

            try
            {
                await service.DeleteAsync(accountId, null, requestOptions, token);
            }
            catch (StripeException ex)
            {
                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }

        public async Task<Result> MakeATransfer(string sourceAccountId, string targetAccountId, decimal amount, string idempotencyKey, Name name, FundraiserId fundraiserId, CancellationToken token)
        {
            var service = new TransferService(_client);

            var createOptions = new TransferCreateOptions
            {
                Amount = (long) amount * 100,
                Currency = "PLN",
                Destination = Guard.Against.NullOrWhiteSpace(targetAccountId, nameof(targetAccountId)),
                Description = $"{fundraiserId} - {name}"
            };
        
            var requestOptions = new RequestOptions
            {
                IdempotencyKey = Guard.Against.NullOrWhiteSpace(idempotencyKey, nameof(idempotencyKey)),
                StripeAccount = Guard.Against.NullOrWhiteSpace(sourceAccountId, nameof(sourceAccountId))
            };

            try
            {
                await service.CreateAsync(createOptions, requestOptions, token);
                //increase fundraiser goal by stripe fee
            }
            catch (StripeException ex)
            {
                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }


    }
}