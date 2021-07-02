using SchoolManagement.Domain.Common.Models;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Common;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.EventReducers
{
    public sealed class FormTutorDivestedDomainEventReducer : IEventReducer
    {
        public void ReduceEvents(ICollection<DomainEvent> domainEvents)
        {
            var formTutorDivestedEvents = domainEvents.OfType<FormTutorDivestedDomainEvent>().ToList();
            if (formTutorDivestedEvents.Count < 2)
                return;

            var collectionEvent = new FormTutorsDivestedDomainEvent(
                formTutorDivestedEvents.Select(de => new MemberIsActiveModel(de.FormTutorId, de.IsActive)));

            foreach (var formTutorDivestedEvent in formTutorDivestedEvents)
                domainEvents.Remove(formTutorDivestedEvent);

            domainEvents.Add(collectionEvent);
        }
    }
}
