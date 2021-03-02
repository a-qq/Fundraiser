using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Common;
using SharedKernel.Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure.Persistance
{
    public sealed class SchoolContext : DbContext, ISchoolContext
    {
        private readonly IDomainEventService _domainEventService;

        public SchoolContext(DbContextOptions<SchoolContext> options,
               IDomainEventService domainEventService)
               : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<School> Schools { get; set; }


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
                  .Entries<AggregateRoot<SchoolId>>()
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
