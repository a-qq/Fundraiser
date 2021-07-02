using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Common;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.EventReducers
{
    public sealed class MemberEnrolledDomainEventReducer : IEventReducer
    {
        public void ReduceEvents(ICollection<DomainEvent> domainEvents)
        {
            var enrolledMembersEvents = domainEvents.OfType<MemberEnrolledDomainEvent>().ToList();
            if (enrolledMembersEvents.Count < 2)
                return;

            var collectionEvent = new MembersEnrolledDomainEvent(
                enrolledMembersEvents.First().SchoolId,
                enrolledMembersEvents.Select(de => de.MemberId).ToList());

            foreach (var enrolledMembersEvent in enrolledMembersEvents)
                domainEvents.Remove(enrolledMembersEvent);

            domainEvents.Add(collectionEvent);
        }
    }
}