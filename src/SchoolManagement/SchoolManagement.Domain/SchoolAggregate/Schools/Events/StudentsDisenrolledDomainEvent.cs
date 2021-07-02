using Ardalis.GuardClauses;
using SchoolManagement.Domain.Common.Models;
using SharedKernel.Domain.Common;
using System.Collections.Generic;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class StudentsDisenrolledDomainEvent : DomainEvent
    {
        public IEnumerable<StudentDisenrollmentData> DisenrolledStudentsData { get; }

        public StudentsDisenrolledDomainEvent(IEnumerable<StudentDisenrollmentData> disenrolledStudentsData)
        {
            DisenrolledStudentsData = Guard.Against.NullOrEmpty(disenrolledStudentsData, nameof(disenrolledStudentsData));
        }
    }
}
