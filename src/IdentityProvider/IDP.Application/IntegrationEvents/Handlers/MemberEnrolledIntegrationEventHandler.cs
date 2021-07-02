using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IdentityModel;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.AddUser;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Domain.Utils;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Collections.Generic;
using System.Threading.Tasks;
using static IDP.Application.MediatorModule;

namespace IDP.Application.IntegrationEvents.Handlers
{
    internal sealed class MemberEnrolledIntegrationEventHandler : IIntegrationEventHandler<MemberEnrolledIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<MemberEnrolledIntegrationEventHandler> _logger;

        public MemberEnrolledIntegrationEventHandler(
            ISender mediator,
            ILogger<MemberEnrolledIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(MemberEnrolledIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var claimsToAdd = new List<ClaimInsertModel>()
                {
                    new ClaimInsertModel(JwtClaimTypes.Role, @event.Role),
                    new ClaimInsertModel(CustomClaimTypes.SchoolId, @event.SchoolId),
                    new ClaimInsertModel(JwtClaimTypes.GivenName, @event.FirstName),
                    new ClaimInsertModel(JwtClaimTypes.FamilyName, @event.LastName),
                    new ClaimInsertModel(JwtClaimTypes.Gender, @event.Gender)
                };

                var command = new AddUserCommand(@event.Email, @event.MemberId, claimsToAdd);

                var result =
                    await _mediator.Send(new IdentifiedCommand<AddUserCommand>(command, @event.Id));

                return result;
            }
        }
    }
}