using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Extensions
{
    public static class MediatrExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx)
        {
            //TODO: check if works
            var aggregates = ctx.ChangeTracker
                .Entries<AggregateRoot<ITypedId>>()
                //.Where(x => x.GetType()
                //    .GetGenericTypeDefinition() == typeof(AggregateRoot<>))
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = aggregates
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            aggregates.ToList()
                .ForEach(entity => entity.Entity.ClearEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}
