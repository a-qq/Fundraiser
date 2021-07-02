using SchoolManagement.Domain.Common.Models;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Common;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.EventReducers
{
    public sealed class StudentDisenrolledDomainEventReducer : IEventReducer
    {
        public void ReduceEvents(ICollection<DomainEvent> domainEvents)
        {
            var studentDisenrolledEvents = domainEvents.OfType<StudentDisenrolledDomainEvent>().ToList();
            if (studentDisenrolledEvents.Count < 2)
                return;

            var collectionEvent = new StudentsDisenrolledDomainEvent(
                studentDisenrolledEvents.Select(de => new StudentDisenrollmentData(de.StudentId, de.RemovedRole, de.IsActive)));

            foreach (var studentDisenrolledEvent in studentDisenrolledEvents)
                domainEvents.Remove(studentDisenrolledEvent);

            domainEvents.Add(collectionEvent);
        }
    }
}