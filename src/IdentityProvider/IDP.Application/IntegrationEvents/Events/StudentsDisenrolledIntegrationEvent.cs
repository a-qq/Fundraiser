using IDP.Application.Common.Models;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Collections.Generic;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class StudentsDisenrolledIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<StudentDisenrollmentData> DisenrolledStudentsData { get; }

        public StudentsDisenrolledIntegrationEvent(IEnumerable<StudentDisenrollmentData> disenrolledStudentsData)
        {
            DisenrolledStudentsData = disenrolledStudentsData;
        }
    }
}
