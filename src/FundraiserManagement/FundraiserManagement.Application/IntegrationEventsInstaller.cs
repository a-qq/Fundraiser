using FundraiserManagement.Application.IntegrationEvents.Incoming;
using SharedKernel.Infrastructure.Abstractions.EventBus;

namespace FundraiserManagement.Application
{
    public static class IntegrationEventsInstaller
    {
        public static void SetFundraiserManagementIntegrationEventSubscriptions(this IEventBus eventBus)
        {
            eventBus.Subscribe<MemberActivatedIntegrationEvent, IIntegrationEventHandler<MemberActivatedIntegrationEvent>>();
            eventBus.Subscribe<SchoolCreatedIntegrationEvent, IIntegrationEventHandler<SchoolCreatedIntegrationEvent>>();
            eventBus.Subscribe<FormTutorAssignedIntegrationEvent, IIntegrationEventHandler<FormTutorAssignedIntegrationEvent>>();
            eventBus.Subscribe<FormTutorDivestedIntegrationEvent, IIntegrationEventHandler<FormTutorDivestedIntegrationEvent>>();
            eventBus.Subscribe<FormTutorsDivestedIntegrationEvent, IIntegrationEventHandler<FormTutorsDivestedIntegrationEvent>>();
            eventBus.Subscribe<HeadmasterDivestedIntegrationEvent, IIntegrationEventHandler<HeadmasterDivestedIntegrationEvent>>();
            eventBus.Subscribe<HeadmasterPromotedIntegrationEvent, IIntegrationEventHandler<HeadmasterPromotedIntegrationEvent>>();
            eventBus.Subscribe<MemberArchivedIntegrationEvent, IIntegrationEventHandler<MemberArchivedIntegrationEvent>>();
            eventBus.Subscribe<MemberExpelledIntegrationEvent, IIntegrationEventHandler<MemberExpelledIntegrationEvent>>();
            eventBus.Subscribe<MemberRestoredIntegrationEvent, IIntegrationEventHandler<MemberRestoredIntegrationEvent>>();
            eventBus.Subscribe<MembersArchivedIntegrationEvent, IIntegrationEventHandler<MembersArchivedIntegrationEvent>>();
            eventBus.Subscribe<SchoolRemovedIntegrationEvent, IIntegrationEventHandler<SchoolRemovedIntegrationEvent>>();
            eventBus.Subscribe<StudentAssignedIntegrationEvent, IIntegrationEventHandler<StudentAssignedIntegrationEvent>>();
            eventBus.Subscribe<StudentDisenrolledIntegrationEvent, IIntegrationEventHandler<StudentDisenrolledIntegrationEvent>>();
            eventBus.Subscribe<StudentsDisenrolledIntegrationEvent, IIntegrationEventHandler<StudentsDisenrolledIntegrationEvent>>();
            eventBus.Subscribe<StudentsAssignedIntegrationEvent, IIntegrationEventHandler<StudentsAssignedIntegrationEvent>>();
            eventBus.Subscribe<TreasurerDivestedIntegrationEvent, IIntegrationEventHandler<TreasurerDivestedIntegrationEvent>>();
            eventBus.Subscribe<TreasurersDivestedIntegrationEvent, IIntegrationEventHandler<TreasurersDivestedIntegrationEvent>>();
            eventBus.Subscribe<TreasurerPromotedIntegrationEvent, IIntegrationEventHandler<TreasurerPromotedIntegrationEvent>>();
        }
    }
}