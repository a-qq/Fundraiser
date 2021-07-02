using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class FormTutorDivestedIntegrationEvent : IntegrationEvent
    {
        public FormTutorDivestedIntegrationEvent(MemberId formTutorId, bool isActive)
        {
            FormTutorId = formTutorId;
            IsActive = isActive;
            RemovedRole = GroupRoles.FormTutor;
        }

        public MemberId FormTutorId { get; }
        public string RemovedRole { get; }
        public bool IsActive { get; }
    }
}