using Fundraiser.SharedKernel.Utils;
using IDP.Core.UserAggregate.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Infrastructure.Database
{
    public sealed class IdentityDbContext : DbContext
    {
        private readonly IMediator _eventPublisher;

        public DbSet<User> Users { get; set; }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IMediator mediator)
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

            List<AggregateRoot> roots = ChangeTracker
                .Entries()
                .Where(x => x.Entity is AggregateRoot)
                .Select(x => (AggregateRoot)x.Entity)
                .ToList();

            int result = await base.SaveChangesAsync(cancellationToken);

            foreach (AggregateRoot root in roots)
            {
                foreach (INotification ev in root.DomainEvents)
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
