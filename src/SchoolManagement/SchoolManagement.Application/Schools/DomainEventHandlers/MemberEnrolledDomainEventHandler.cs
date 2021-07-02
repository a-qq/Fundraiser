using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.IntegrationEvents.Events;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.DomainEventHandlers
{
    internal sealed class MemberEnrolledDomainEventHandler
        : INotificationHandler<DomainEventNotification<MemberEnrolledDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly ILocalCacheSchoolRepository _schoolRepository;
        private readonly IIntegrationEventService _integrationEventService;

        public MemberEnrolledDomainEventHandler(ILoggerFactory logger,
            ILocalCacheSchoolRepository schoolRepository,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _schoolRepository = schoolRepository;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<MemberEnrolledDomainEvent> notification,
            CancellationToken cancellationToken)
        {

            var member = (await _schoolRepository.GetByIdWithMembersAsync(
                    notification.DomainEvent.SchoolId, cancellationToken))
                .Value.Members.Single(m => m.Id == notification.DomainEvent.MemberId);

            _logger.CreateLogger<MemberEnrolledDomainEvent>()
                .LogTrace("Member with Id: {MemberId} has been successfully enrolled to school {SchoolName} ({Id})!",
                member.Id, member.School.Name, member.School.Id);

            await _integrationEventService.AddAndSaveEventAsync(new MemberEnrolledIntegrationEvent(
                member.Id, member.Email, member.Role, member.School.Id, member.FirstName, member.LastName, member.Gender));
        }
    }
}