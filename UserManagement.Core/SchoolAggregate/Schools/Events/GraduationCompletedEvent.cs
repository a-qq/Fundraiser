using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class GraduationCompletedEvent : INotification
    {
        public IReadOnlyList<Guid> IdsOfArchivedStudents { get; }
        public IReadOnlyList<Guid> IdsOfDivestedFormTutors { get; }
        public IReadOnlyList<Guid> IdsOfDivestedTreasurers { get; }

        public GraduationCompletedEvent(List<Guid> idsOfMembersToArchive, List<Guid> idsOfFormTutorsToDivest, List<Guid> idsOfDivestedTreasurers)
        {
            if (idsOfMembersToArchive == null || idsOfMembersToArchive.Any(c => c == Guid.Empty))
                throw new ArgumentException(nameof(idsOfMembersToArchive));

            IdsOfArchivedStudents = idsOfMembersToArchive.AsReadOnly();

            if (idsOfFormTutorsToDivest == null || idsOfFormTutorsToDivest.Any(c => c == Guid.Empty))
                throw new ArgumentException(nameof(idsOfFormTutorsToDivest));

            IdsOfDivestedFormTutors = idsOfFormTutorsToDivest.AsReadOnly();

            if (idsOfDivestedTreasurers == null || idsOfDivestedTreasurers.Any(c => c == Guid.Empty))
                throw new ArgumentException(nameof(idsOfFormTutorsToDivest));

            IdsOfDivestedTreasurers = idsOfDivestedTreasurers.AsReadOnly();
        }
    }
}
