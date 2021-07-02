using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Fundraisers.Commands.OpenFundraiser;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Fundraisers.Commands.ResumeIntraschoolFundraisersManagedByHeadmaster;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Local
{
    internal sealed class IntraschoolFundraisersSuspendedApplicationEvent : IntegrationEvent
    {
        public MemberId HeadmasterId { get; }
        public SchoolId SchoolId { get; }
        public FundraiserId FundraiserId { get; }

        public IntraschoolFundraisersSuspendedApplicationEvent(
            MemberId headmasterId, SchoolId schoolId, FundraiserId fundraiserId)
        { 
            HeadmasterId = headmasterId;
            SchoolId = schoolId;
            FundraiserId = fundraiserId;
        }
    }

    internal sealed class IntraschoolFundraisersSuspendedApplicationEventHandler : IIntegrationEventHandler<IntraschoolFundraisersSuspendedApplicationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<IntraschoolFundraisersSuspendedApplicationEvent> _logger;

        public IntraschoolFundraisersSuspendedApplicationEventHandler(
            ISender mediator,
            ILogger<IntraschoolFundraisersSuspendedApplicationEvent> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(IntraschoolFundraisersSuspendedApplicationEvent @event)
        {
            using (LogContext.PushProperty("ApplicationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling application event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new ResumeIntraschoolFundraiserManagedByPromotedHeadmasterCommand(
                    @event.HeadmasterId, @event.SchoolId, @event.FundraiserId, @event.Id.ToString());

                var result = await _mediator.Send(
                    new IdentifiedCommand<ResumeIntraschoolFundraiserManagedByPromotedHeadmasterCommand>(command, @event.Id));

                return result;
            }
        }
    }
}