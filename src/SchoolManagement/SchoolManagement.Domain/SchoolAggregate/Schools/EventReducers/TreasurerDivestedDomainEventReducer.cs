using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Common;
using System.Collections.Generic;
using System.Linq;
using SchoolManagement.Domain.Common.Models;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.EventReducers
{
    public sealed class TreasurerDivestedDomainEventReducer : IEventReducer
    {
        public void ReduceEvents(ICollection<DomainEvent> domainEvents)
        {
            var formTutorDivestedEvents = domainEvents.OfType<TreasurerDivestedDomainEvent>().ToList();
            if (formTutorDivestedEvents.Count < 2)
                return;

            var collectionEvent = new TreasurersDivestedDomainEvent(
                formTutorDivestedEvents.Select(de => new MemberIsActiveModel(de.TreasurerId, de.IsActive)));

            foreach (var formTutorDivestedEvent in formTutorDivestedEvents)
                domainEvents.Remove(formTutorDivestedEvent);

            domainEvents.Add(collectionEvent);
        }
    }
}