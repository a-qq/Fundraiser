using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Domain.MemberAggregate;
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
    internal sealed class MustBeEligibleToCreateFundraiserRequirement : IAuthorizationRequirement
    {
    }

    internal sealed class MustBeEligibleToCreateFundraiserRequirementHandler :
        AuthorizationHandler<MustBeEligibleToCreateFundraiserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            MustBeEligibleToCreateFundraiserRequirement requirement)
        {
            if (!(context.Resource is IFundraiserTypeAuthorizationRequest request))
                throw new InvalidOperationException(context.Resource.GetGenericTypeName());

            if (context.User.IsInRole(Administrator.RoleName))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Role.Headmaster.ToString()))
            {
                if (request.Type == Type.TeacherDay &&
                    request.Range == Range.Intragroup &&
                    request.GroupId.HasValue)
                {
                    context.Fail();
                }
                else context.Succeed(requirement);

                return Task.CompletedTask;
            }

            if (request.GroupId.HasValue && request.Range == Range.Intragroup)
            {
                if (!context.User.IsInGroupRole())
                {
                    context.Fail();
                    return Task.CompletedTask;
                }

                if (request.Type == Type.MenDay && !context.User.HasGender(Gender.Female) ||
                    request.Type == Type.WomanDay && !context.User.HasGender(Gender.Male))
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
            }

            if (request.GroupId.HasValue && request.Type == Type.Normal &&
                request.Range == Range.Intergroup && !context.User.IsInRole(GroupRoles.FormTutor))
            {

                context.Fail();
                return Task.CompletedTask;
            }

            if (!request.GroupId.HasValue && request.Range == Range.Intraschool && request.Type == Type.Normal)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}