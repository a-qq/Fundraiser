using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class FormTutorDivestedIntegrationEvent : IntegrationEvent
    {
        public FormTutorDivestedIntegrationEvent(string formTutorId, string removedRole, bool isActive)
        {
            FormTutorId = formTutorId;
            RemovedRole = removedRole;
            IsActive = isActive;
        }

        public string FormTutorId { get; }
        public string RemovedRole { get; }
        public bool IsActive { get; }
    }
}