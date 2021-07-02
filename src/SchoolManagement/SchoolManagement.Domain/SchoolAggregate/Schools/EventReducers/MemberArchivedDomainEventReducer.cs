using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Common;
using System.Collections.Generic;
using System.Linq;
using SchoolManagement.Domain.Common.Models;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.EventReducers
{
    public sealed class MemberArchivedDomainEventReducer : IEventReducer
    {
        public void ReduceEvents(ICollection<DomainEvent> domainEvents)
        {
            var memberArchivedEvents = domainEvents.OfType<MemberArchivedDomainEvent>().ToList();
            if (memberArchivedEvents.Count < 2)
                return;

            var collectionEvent = new MembersArchivedDomainEvent(
                memberArchivedEvents.Select(e => new MemberArchivisationData(e.MemberId, e.GroupRole)));

            foreach (var memberArchivedEvent in memberArchivedEvents)
                domainEvents.Remove(memberArchivedEvent);

            domainEvents.Add(collectionEvent);
        }
    }
}