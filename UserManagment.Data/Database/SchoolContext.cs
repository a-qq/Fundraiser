using Fundraiser.SharedKernel.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.SchoolAggregate.Schools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Database
{
    public sealed class SchoolContext : DbContext
    {
        private readonly IMediator _eventPublisher;

        public DbSet<School> Schools { get; set; }

        public SchoolContext(DbContextOptions<SchoolContext> options, IMediator mediator)
            : base(options)
        {
            _eventPublisher = mediator;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //IEnumerable<EntityEntry> enumerationEntries = ChangeTracker.Entries()
            //    .Where(x => EnumerationTypes.Contains(x.Entity.GetType()));

            //foreach (EntityEntry enumerationEntry in enumerationEntries)
            //{
            //    enumerationEntry.State = EntityState.Unchanged;
            //}

            List<AggregateRoot<Guid>> roots = ChangeTracker
                .Entries()
                .Where(x => x.Entity is AggregateRoot<Guid>)
                .Select(x => (AggregateRoot<Guid>)x.Entity)
                .ToList();

            int result = await base.SaveChangesAsync(cancellationToken);
             
            foreach (AggregateRoot<Guid> root in roots)
            {
                foreach(INotification ev in root.DomainEvents)
                {
                    await _eventPublisher.Publish(ev);
                }
                root.ClearEvents();
            }

            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
