using Ardalis.GuardClauses;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.MemberAggregate;
using FundraiserManagement.Infrastructure.Persistence.Configuration;
using FundraiserManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.SchoolAggregate;

namespace FundraiserManagement.Infrastructure.Persistence
{
    public sealed class FundraiserContext : DbContext, IFundraiserContext
    {
        private readonly DomainEventService _domainEventService;
        private IDbContextTransaction _currentTransaction;

        public FundraiserContext(
            DbContextOptions<FundraiserContext> options,
            DomainEventService domainEventService)
            : base(options)
        {
            _domainEventService = Guard.Against.Null(domainEventService, nameof(domainEventService));
        }

        public FundraiserContext(DbContextOptions<FundraiserContext> options) : base(options) { }

        public DbSet<Fundraiser> Fundraisers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<School> Schools { get; set; } 

        public DbConnection GetDbConnection()
            => Database.GetDbConnection();

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public IExecutionStrategy CreateExecutionStrategy() => Database.CreateExecutionStrategy();

        public bool HasActiveTransaction => _currentTransaction != null;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _domainEventService.DispatchDomainEvents(this);

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(FundraisingConfiguration).GetTypeInfo().Assembly);

            base.OnModelCreating(builder);
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            Guard.Against.Null(transaction, nameof(transaction));

            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}