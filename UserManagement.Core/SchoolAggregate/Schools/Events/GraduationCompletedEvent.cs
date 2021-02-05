using MediatR;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class GraduationCompletedEvent : INotification
    {
        public IReadOnlyList<Guid> IdsOfArchivedStudents { get; }
        public IReadOnlyList<Guid> IdsOfDivestedFormTutors { get; }
        public IReadOnlyList<Guid> IdsOfDivestedTreasurers { get; }

        public GraduationCompletedEvent(List<Guid> idsOfMembersToArchive, List<Guid> idsOfFormTutorsToDivest, List<Guid> idsOfDivestedTreasurers)
        {
            IdsOfArchivedStudents = idsOfMembersToArchive.AsReadOnly();
            IdsOfDivestedFormTutors = idsOfFormTutorsToDivest.AsReadOnly();
            IdsOfDivestedTreasurers = idsOfDivestedTreasurers.AsReadOnly();
        }
    }
}
