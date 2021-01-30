using MediatR;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class SchoolRemovedEvent : INotification
    {
        public Guid SchoolId { get; }

        public SchoolRemovedEvent(Guid schoolId)
        {
            SchoolId = schoolId;
        }
    }
}
