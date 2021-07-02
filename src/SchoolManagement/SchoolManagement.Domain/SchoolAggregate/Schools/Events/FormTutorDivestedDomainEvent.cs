using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class FormTutorDivestedDomainEvent : DomainEvent
    {
        internal FormTutorDivestedDomainEvent(MemberId formTutorId, bool isActive)
        {
            FormTutorId = formTutorId;
            IsActive = isActive;
        }

        public MemberId FormTutorId { get; }
        public bool IsActive { get; }
    }
}