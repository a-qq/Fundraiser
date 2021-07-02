using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.IntegrationEvents.Events;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.DomainEventHandlers
{
    internal sealed class MemberActivatedDomainEventHandler : INotificationHandler<DomainEventNotification<MemberActivatedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly ILocalCacheSchoolRepository _schoolRepository;
        private readonly IIntegrationEventService _integrationEventService;

        public MemberActivatedDomainEventHandler(ILoggerFactory logger,
            ILocalCacheSchoolRepository schoolRepository,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _schoolRepository = schoolRepository;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<MemberActivatedDomainEvent> notification,
            CancellationToken cancellationToken)
        {

            var member = (await _schoolRepository.GetByIdWithMembersAsync(
                    notification.DomainEvent.SchoolId, cancellationToken))
                .Value.Members.Single(m => m.Id == notification.DomainEvent.MemberId);

            _logger.CreateLogger<MemberActivatedDomainEvent>()
                .LogTrace("Member with Id: {MemberId} from school {SchoolName} ({Id}) has been successfully activated!",
                    member.Id, member.School.Name, member.School.Id);

            GroupId? groupId = null;
            bool isFormTutor = false;
            bool isTreasurer = false;
            if (member.Role == Role.Student)
            {
                groupId = member.Group?.Id;
                isTreasurer = member.Group?.Treasurer == member;
            }
            else
            {
                var group = member.School.CurrentGroupOfFormTutor(member);
                if (group.HasValue)
                {
                    groupId = group.Value.Id;
                    isFormTutor = true;
                }
            }

            await _integrationEventService.AddAndSaveEventAsync(new MemberActivatedIntegrationEvent(member.Id,
                member.School.Id, member.Role.Value, member.Gender.Value, member.Email, groupId, isFormTutor, isTreasurer));
        }
    }
}