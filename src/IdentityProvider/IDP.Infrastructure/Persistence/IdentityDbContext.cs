using Ardalis.GuardClauses;
using IDP.Application.Common.Interfaces;
using IDP.Domain.UserAggregate.Entities;
using IDP.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.Infrastructure.Abstractions.Common;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Infrastructure.Persistence
{
    public sealed class IdentityDbContext : DbContext, IIdentityContext
    {
        private readonly IDomainEventService _domainEventService;
        private IDbContextTransaction _currentTransaction;

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options,
            IDomainEventService domainEventService)
            : base(options)
        {
            _domainEventService = Guard.Against.Null(domainEventService, nameof(domainEventService));
        }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

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
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).GetTypeInfo().Assembly);
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

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