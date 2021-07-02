using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.MemberAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FundraiserManagement.Application.IntegrationEvents.Local;
using Range = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.Range;

namespace FundraiserManagement.Application.Members.DomainEventHandlers
{
    internal sealed class HeadmasterPromotedDomainEventHandler : INotificationHandler<DomainEventNotification<HeadmasterPromotedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IFundraiserRepository _fundraiserRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IIntegrationEventService _integrationEventService;

        public HeadmasterPromotedDomainEventHandler(
            ILoggerFactory logger,
            ISchoolRepository schoolRepository,
            IFundraiserRepository fundraiserRepository,
            IMemberRepository memberRepository,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _schoolRepository = schoolRepository;
            _fundraiserRepository = fundraiserRepository;
            _memberRepository = memberRepository;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<HeadmasterPromotedDomainEvent> notification,
            CancellationToken token)
        {
            var logger = _logger.CreateLogger<HeadmasterPromotedDomainEvent>();

            logger.LogTrace("Member with Id: {MemberId} has been successfully promoted to headmaster!", notification.DomainEvent.MemberId);

            var memberOrNone = await _memberRepository.GetByIdAsync(notification.DomainEvent.MemberId, token);
            if (memberOrNone.HasNoValue || memberOrNone.Value.SchoolId != notification.DomainEvent.SchoolId)
                throw new InvalidOperationException($"Member (Id:{notification.DomainEvent.MemberId}) not found!");

            if (memberOrNone.Value.Role != SchoolRole.Headmaster)
                throw new InvalidOperationException($"Member (Id:{notification.DomainEvent.MemberId}) is not headmaster!");

            var schoolOrNone = await _schoolRepository.GetByIdAsync(notification.DomainEvent.SchoolId, token);
            if (schoolOrNone.HasNoValue)
                throw new InvalidOperationException($"School (Id:{notification.DomainEvent.SchoolId}) not found!");

            var fundraisers = await _fundraiserRepository.GetBySchoolIdAsync(notification.DomainEvent.SchoolId, token);

            var managedFundraisers = fundraisers
                .Where(f => f.Manager == memberOrNone.Value &&
                            (f.State == State.Open || f.State == State.Stopped) && f.Range == Range.Intraschool);
            
            foreach (var fund in managedFundraisers)
            {
                fund.Suspend(wasManagerHeadmaster: false); 
                await _integrationEventService.AddAndSaveEventAsync(new IntraschoolFundraisersSuspendedApplicationEvent(
                    notification.DomainEvent.MemberId, notification.DomainEvent.SchoolId, fund.Id));
            }

            var suspendedFundraisers = fundraisers
               .Where(f => f.State.HasFlag(State.Suspended) && f.Manager is null)
               .ToList();

            if (suspendedFundraisers.Any())
            {
                var result = Result.Combine(suspendedFundraisers.Select(f => f.ChangeManager(memberOrNone.Value)), "\n");
                if (result.IsFailure)
                    throw new InvalidOperationException(result.Error);
            }
        }
    }
}
