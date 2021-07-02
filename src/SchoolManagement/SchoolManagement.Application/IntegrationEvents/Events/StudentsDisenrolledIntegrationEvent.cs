using Ardalis.GuardClauses;
using SchoolManagement.Domain.Common.Models;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Collections.Generic;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class StudentsDisenrolledIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<StudentDisenrollmentData> DisenrolledStudentsData { get; }

        public StudentsDisenrolledIntegrationEvent(IEnumerable<StudentDisenrollmentData> disenrolledStudentsData)
        {
            DisenrolledStudentsData = Guard.Against.NullOrEmpty(disenrolledStudentsData, nameof(disenrolledStudentsData));
        }
    }
}
