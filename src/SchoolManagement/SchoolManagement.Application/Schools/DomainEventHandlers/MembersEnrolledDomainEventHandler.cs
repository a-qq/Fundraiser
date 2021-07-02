using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.IntegrationEvents.Events;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Concretes.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.DomainEventHandlers
{
    internal sealed class MembersEnrolledDomainEventHandler : INotificationHandler<DomainEventNotification<MembersEnrolledDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly ILocalCacheSchoolRepository _schoolRepository;
        private readonly IIntegrationEventService _integrationEventService;

        public MembersEnrolledDomainEventHandler(ILoggerFactory logger,
            ILocalCacheSchoolRepository schoolRepository,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _schoolRepository = schoolRepository;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<MembersEnrolledDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            var school = (await _schoolRepository.GetByIdWithMembersAsync(
                    notification.DomainEvent.SchoolId, cancellationToken)).Value;

            var membersData = school.Members
                .Where(m => notification.DomainEvent.MemberIds.Contains(m.Id))
                .Select(m => new MemberEnrollmentData(m.Id, m.Email, m.Role, m.Group?.Id, m.FirstName, m.LastName, m.Gender))
                .ToList();

            if (membersData.Count != notification.DomainEvent.MemberIds.Count())
                throw new InvalidOperationException(nameof(MembersEnrolledDomainEventHandler));

            _logger.CreateLogger<MemberArchivedDomainEvent>()
                .LogTrace("Members with Ids: {MemberIds} has been successfully enrolled to school {SchoolName} ({Id})!",
                    string.Join(", ", notification.DomainEvent.MemberIds), school.Name, school.Id);

            await _integrationEventService.AddAndSaveEventAsync(
                new MembersEnrolledIntegrationEvent(notification.DomainEvent.SchoolId, membersData));
        }
    }
}