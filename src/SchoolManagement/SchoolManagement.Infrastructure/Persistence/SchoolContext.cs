using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SchoolManagement.Infrastructure.Persistence.Configuration;
using SchoolManagement.Infrastructure.Services;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure.Persistence
{
    public sealed class SchoolContext : DbContext, ISchoolContext
    {
        private readonly DomainEventService _domainEventService;
        private IDbContextTransaction _currentTransaction;

        public SchoolContext(DbContextOptions<SchoolContext> options,
            DomainEventService domainEventService)
            : base(options)
        {
            _domainEventService = Guard.Against.Null(domainEventService, nameof(domainEventService));
        }

        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

        public DbSet<School> Schools { get; set; }

        public DbConnection GetDbConnection() => Database.GetDbConnection();

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
            builder.ApplyConfigurationsFromAssembly(typeof(SchoolConfiguration).GetTypeInfo().Assembly);

            base.OnModelCreating(builder);
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