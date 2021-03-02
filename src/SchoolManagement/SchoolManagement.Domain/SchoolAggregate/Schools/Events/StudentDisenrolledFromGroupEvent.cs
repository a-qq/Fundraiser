using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class StudentDisenrolledFromGroupEvent : DomainEvent
    {
        public Guid StudentId { get; }

        internal StudentDisenrolledFromGroupEvent(MemberId studentId)
        {
            StudentId = Guard.Against.Default(studentId, nameof(studentId));
        }
    }
}