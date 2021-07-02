using SchoolManagement.Domain.Common.Models;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Common;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.EventReducers
{
    public sealed class StudentAssignedDomainEventReducer : IEventReducer
    {
        public void ReduceEvents(ICollection<DomainEvent> domainEvents)
        {
            var assignedStudentsEventsGroups = domainEvents.OfType<StudentAssignedDomainEvent>()
                .ToLookup(de => de.GroupId).Where(g => g.Count() > 1).ToList();

            if (!assignedStudentsEventsGroups.Any())
                return;

            foreach (var eventGroup in assignedStudentsEventsGroups)
            {
                foreach (var @event in eventGroup)
                    domainEvents.Remove(@event);

                var collectionEvent = new StudentsAssignedDomainEvent(
                    eventGroup.Key,
                    eventGroup.Select(de => new MemberIsActiveModel(de.StudentId, de.IsActive)));

                domainEvents.Add(collectionEvent);
            }
        }
    }
}