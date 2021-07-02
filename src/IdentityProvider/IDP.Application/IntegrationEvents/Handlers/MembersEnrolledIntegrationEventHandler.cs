using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IdentityModel;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.AddUsers;
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
    internal sealed class MembersEnrolledIntegrationEventHandler : IIntegrationEventHandler<MembersEnrolledIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<MembersEnrolledIntegrationEventHandler> _logger;

        public MembersEnrolledIntegrationEventHandler(
            ISender mediator,
            ILogger<MembersEnrolledIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(MembersEnrolledIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var usersToInsert = new List<UserInsertModel>();
                foreach (var memberData in @event.MembersData)
                {
                    var claimsToAdd = new List<ClaimInsertModel>()
                    {
                        new ClaimInsertModel(JwtClaimTypes.Role, memberData.Role),
                        new ClaimInsertModel(CustomClaimTypes.SchoolId, @event.SchoolId),
                        new ClaimInsertModel(JwtClaimTypes.GivenName, memberData.FirstName),
                        new ClaimInsertModel(JwtClaimTypes.FamilyName, memberData.LastName),
                        new ClaimInsertModel(JwtClaimTypes.Gender, memberData.Gender)
                    };

                    if (memberData.GroupId.HasValue)
                        claimsToAdd.Add(new ClaimInsertModel(CustomClaimTypes.GroupId, memberData.GroupId.Value));

                    usersToInsert.Add(new UserInsertModel(memberData.Email, memberData.MemberId, claimsToAdd));
                }

                var command = new AddUsersCommand(usersToInsert);

                var result = await _mediator.Send(new IdentifiedCommand<AddUsersCommand>(command, @event.Id));

                return result;
            }
        }
    }
}