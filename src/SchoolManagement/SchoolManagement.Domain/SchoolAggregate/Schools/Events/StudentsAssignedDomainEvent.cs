using Ardalis.GuardClauses;
using SchoolManagement.Domain.Common.Models;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SharedKernel.Domain.Common;
using System.Collections.Generic;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class StudentsAssignedDomainEvent : DomainEvent
    {
        internal StudentsAssignedDomainEvent(GroupId groupId, IEnumerable<MemberIsActiveModel> studentData)
        {
            GroupId = groupId;
            StudentData = Guard.Against.NullOrEmpty(studentData, nameof(studentData));
        }

        public GroupId GroupId { get; }
        public IEnumerable<MemberIsActiveModel> StudentData { get; }
    }
}