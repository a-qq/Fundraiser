using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class FormTutorAssignedIntegrationEvent : IntegrationEvent
    {
        public string FormTutorId { get; }
        public string GroupId { get; }
        public string AssignedRole { get; }
        public bool IsActive { get; }

        public FormTutorAssignedIntegrationEvent(string formTutorId, string groupId,
            string assignedRole, bool isActive)
        {
            FormTutorId = formTutorId;
            GroupId = groupId;
            AssignedRole = assignedRole;
            IsActive = isActive;
        }
    }
}