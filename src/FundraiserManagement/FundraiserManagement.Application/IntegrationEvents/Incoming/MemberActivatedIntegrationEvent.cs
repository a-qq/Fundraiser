using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.CreateMember;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Threading.Tasks;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class MemberActivatedIntegrationEvent : IntegrationEvent
    {
        public MemberId MemberId { get; }
        public SchoolId SchoolId { get; }
        public SchoolRole Role { get; }
        public Gender Gender { get; }
        public string Email { get; }
        public GroupId? GroupId { get; }
        public bool IsFormTutor { get; }
        public bool IsTreasurer { get; }

        public MemberActivatedIntegrationEvent(MemberId memberId, SchoolId schoolId, SchoolRole role,
            Gender gender, string email, GroupId? groupId, bool isFormTutor, bool isTreasurer)
        {
            MemberId = memberId;
            SchoolId = schoolId;
            Role = role;
            Gender = gender;
            Email = email;
            GroupId = groupId;
            IsFormTutor = isFormTutor;
            IsTreasurer = isTreasurer;
        }
    }

    internal sealed class MemberActivatedIntegrationEventHandler : IIntegrationEventHandler<MemberActivatedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<MemberActivatedIntegrationEvent> _logger;

        public MemberActivatedIntegrationEventHandler(
            ISender mediator,
            ILogger<MemberActivatedIntegrationEvent> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(MemberActivatedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, AppName, @event);

                var command = new CreateMemberCommand(@event.MemberId, @event.SchoolId, @event.Role, @event.Gender,
                    @event.Email, @event.GroupId, @event.IsFormTutor, @event.IsTreasurer);

                var result = await _mediator.Send(new IdentifiedCommand<CreateMemberCommand>(command, @event.Id));

                return result;
            }
        }
    }
}