using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Fundraisers.Queries.GetFundraiserAuthorizationData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using SharedKernel.Domain.Constants;
using SharedKernel.Domain.EnumeratedEntities;
using SharedKernel.Infrastructure.Extensions;
using System;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Auth;
using Type = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.Type;

namespace FundraiserManagement.Application.Common.Security
{
    internal sealed class CurrentUserMustBeTreasurerOrSuperiorOfModifiedFundraiserRequirement : IAuthorizationRequirement
    {
    }

    internal sealed class CurrentUserMustBeTreasurerOrSuperiorOfModifiedFundraiserRequirementHandler
    : AuthorizationHandler<CurrentUserMustBeTreasurerOrSuperiorOfModifiedFundraiserRequirement>
    {
        private readonly ISender _mediator;

        public CurrentUserMustBeTreasurerOrSuperiorOfModifiedFundraiserRequirementHandler(ISender mediator)
        {
            _mediator = mediator;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CurrentUserMustBeTreasurerOrSuperiorOfModifiedFundraiserRequirement requirement)
        {
            if (!(context.Resource is IFundraiserAuthorizationRequest request))
                throw new InvalidOperationException(context.Resource.GetGenericTypeName());

            if (context.User.IsInRole(SchoolRole.Headmaster.ToString()) ||
                context.User.IsInRole(Administrator.RoleName))
            {
                context.Succeed(requirement);
                return;
            }

            var fundraiserAuthDtoOrNone =
                await _mediator.Send(new GetFundraiserAuthorizationDataQuery(request.SchoolId, request.FundraiserId));

            if (fundraiserAuthDtoOrNone.HasValue)
            { 
                if(fundraiserAuthDtoOrNone.Value.Type == Type.TeacherDay &&
                   context.User.IsInRole(SchoolRole.Teacher.ToString()))
                {
                    context.Fail();
                    return;
                }

                if (!fundraiserAuthDtoOrNone.Value.GroupId.HasValue &&
                    context.User.Subject() != fundraiserAuthDtoOrNone.Value.ManagerId)
                {
                    context.Fail();
                    return;
                }

                if(fundraiserAuthDtoOrNone.Value.GroupId.HasValue &&
                   (!context.User.IsInGroup(fundraiserAuthDtoOrNone.Value.GroupId.Value) ||
                    context.User.Subject() != fundraiserAuthDtoOrNone.Value.ManagerId &&
                    !context.User.IsInRole(GroupRoles.FormTutor)))
                {
                    context.Fail();
                    return;
                }
            }

            context.Succeed(requirement);
        }
    }
}