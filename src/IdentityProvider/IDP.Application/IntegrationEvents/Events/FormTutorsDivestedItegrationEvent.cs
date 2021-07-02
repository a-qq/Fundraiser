using SharedKernel.Infrastructure.Concretes.Models;
using System.Collections.Generic;
using IDP.Application.Common.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class FormTutorsDivestedIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<MemberIsActiveModel> FormTutorsData { get; }
        public string RemovedRole { get; }

        public FormTutorsDivestedIntegrationEvent(
            IEnumerable<MemberIsActiveModel> formTutorsData, string removedRole)
        {
            FormTutorsData = formTutorsData;
            RemovedRole = removedRole;
        }
    }
}