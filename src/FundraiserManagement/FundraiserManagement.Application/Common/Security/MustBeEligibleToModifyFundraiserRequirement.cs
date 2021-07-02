using Ardalis.GuardClauses;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Fundraisers.Queries.GetFundraiserAuthorizationData;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using SharedKernel.Domain.Constants;
using SharedKernel.Domain.EnumeratedEntities;
using SharedKernel.Infrastructure.Extensions;
using System;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Auth;
using Gender = SharedKernel.Domain.Constants.Gender;
using Range = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.Range;
using Type = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.Type;

namespace FundraiserManagement.Application.Common.Security
{
    internal sealed class MustBeEligibleToModifyFundraiserRequirement : IAuthorizationRequirement
    {
    }

    internal sealed class MustBeEligibleToModifyFundraiserRequirementHandler : 
        AuthorizationHandler<MustBeEligibleToModifyFundraiserRequirement>
    {
        private readonly ISender _mediator;

        public MustBeEligibleToModifyFundraiserRequirementHandler(ISender mediator)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            MustBeEligibleToModifyFundraiserRequirement requirement)
        {
            {
                if (!(context.Resource is IFundraiserAuthorizationRequest request))
                    throw new InvalidOperationException(context.Resource.GetGenericTypeName());

                if (context.User.IsInRole(Administrator.RoleName))
                {
                    context.Succeed(requirement);
                    return;
                }

                var fundraiserAuthDtoOrNone = await _mediator.Send(
                        new GetFundraiserAuthorizationDataQuery(request.SchoolId, request.FundraiserId));

                if (fundraiserAuthDtoOrNone.HasNoValue)
                {
                    context.Succeed(requirement);
                    return;
                }

                var fundraiser = fundraiserAuthDtoOrNone.Value;

                if (context.User.IsInRole(Role.Headmaster.ToString()))
                {
                    if (fundraiser.Type == Type.TeacherDay)
                    { 
                        context.Fail();
                    }
                    else context.Succeed(requirement);

                    return;
                }

                if (fundraiser.GroupId.HasValue && fundraiser.Range == Range.Intragroup)
                {
                    if (!context.User.IsInGroupRole())
                    {
                        context.Fail();
                        return;
                    }

                    if (context.User.Subject() != fundraiser.ManagerId && !context.User.IsInRole(GroupRoles.FormTutor))
                    {
                        context.Fail();
                        return;
                    }

                    if (fundraiser.Type == Type.MenDay && !context.User.HasGender(Gender.Female) ||
                        fundraiser.Type == Type.WomanDay && !context.User.HasGender(Gender.Male))
                    {
                        context.Fail();
                        return;
                    }
                }

                if (fundraiser.Range == Range.Intergroup &&
                    (!context.User.IsInRole(GroupRoles.FormTutor) ||
                     !context.User.IsInGroup(fundraiser.GroupId.Value))) 
                {
                    context.Fail();
                    return;
                }

                if (fundraiser.Range == Range.Intraschool)
                {
                    context.Fail();
                    return;
                }

                context.Succeed(requirement);
            }
        }
    }
}