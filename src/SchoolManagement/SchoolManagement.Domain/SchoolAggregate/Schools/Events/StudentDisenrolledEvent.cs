using System;
using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class StudentDisenrolledEvent : DomainEvent
    {
        internal StudentDisenrolledEvent(MemberId studentId)
        {
            StudentId = Guard.Against.Default(studentId, nameof(studentId));
        }

        public Guid StudentId { get; }
    }
}