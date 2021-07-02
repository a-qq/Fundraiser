using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class FormTutorAssignedIntegrationEvent : IntegrationEvent
    {
        public MemberId FormTutorId { get; }
        public GroupId GroupId { get; }
        public string AssignedRole { get; }
        public bool IsActive { get; }

        public FormTutorAssignedIntegrationEvent(MemberId formTutorId, GroupId groupId, bool isActive)
        {
            FormTutorId = formTutorId;
            GroupId = groupId;
            AssignedRole = GroupRoles.FormTutor;
            IsActive = isActive;
        }
    }
}