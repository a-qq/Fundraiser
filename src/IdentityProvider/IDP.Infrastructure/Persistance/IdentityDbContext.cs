﻿using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IDP.Application.Common.Interfaces;
using IDP.Domain.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Common;
using SharedKernel.Infrastructure.Interfaces;

namespace IDP.Infrastructure.Persistance
{
    public sealed class IdentityDbContext : DbContext, IIdentityContext
    {
        private readonly IDomainEventService _domainEventService;

        public IdentityDbContext(DbContextOptions options,
            IDomainEventService domainEventService)
            : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<User> Users { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            await DispatchEvents();

            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        private async Task DispatchEvents()
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker
                    .Entries<AggregateRoot>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
                    .Where(domainEvent => !domainEvent.IsPublished)
                    .FirstOrDefault();

                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity);
            }
        }
    }
}