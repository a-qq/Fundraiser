using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class StudentDisenrolledEvent : DomainEvent
    {
        public Guid StudentId { get; }

        internal StudentDisenrolledEvent(MemberId studentId)
        {
            StudentId = Guard.Against.Default(studentId, nameof(studentId));
        }
    }
}
