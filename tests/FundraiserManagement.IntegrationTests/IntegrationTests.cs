using Autofac;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.MemberAggregate;
using FundraiserManagement.Infrastructure.Persistence;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using SharedKernel.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using SharedKernel.Domain.Constants;
using SharedKernel.Domain.ValueObjects;
using Xunit;
using Range = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.Range;
using Type = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.Type;

namespace FundraiserManagement.IntegrationTests
{
    public class IntegrationTests : IClassFixture<CompositionRootFixture>
    {
        private readonly CompositionRootFixture _compositionRoot;

        public IntegrationTests(CompositionRootFixture compositionRoot)
        {
            _compositionRoot = compositionRoot;

            ClearDatabase();
        }

        private void ClearDatabase()
        {
            const string sql =
                "DELETE FROM fund.Fundraisers; " +
                "DELETE FROM fund.[Members]; " +
                "DELETE FROM idreq.requests; " +
                "DELETE FROM bus.IntegrationEventLog;";


            using (var connection = new SqlConnection(_compositionRoot.ConnectionString))
            {
                var command = new SqlCommand(sql, connection)
                {
                    CommandType = CommandType.Text
                };
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        protected async Task<Member> CreateMember(
            SchoolId schoolId,
            SchoolRole role = SchoolRole.Headmaster,
            GroupId? groupId = null,
            Gender gender = Gender.Male,
            string email = "test@o2.pl",
            bool isTreasurer = false,
            bool isFormTutor = false,
            bool isArchived = false)
        {

            var member = new Member(MemberId.New(), schoolId, gender, role, Email.Create(email).Value);
            Result result = Result.Success();

            if (isFormTutor || isTreasurer || !(groupId is null))
            {
                if (groupId is null || isArchived)
                    throw new InvalidOperationException(nameof(CreateMember));

                if (isFormTutor)
                    result = member.PromoteToFormTutor(groupId.Value);

                else
                {
                    result = member.EnrollToGroup(groupId.Value);
                    if (isTreasurer)
                        result = Result.Combine(member.PromoteToTreasurer(), result);
                }
            }

            if (isArchived)
                result = member.Archive();

            if (result.IsFailure)
                throw new InvalidOperationException(result.Error);

            await using (var scope = _compositionRoot.BeginLifetimeScope())
            {
                var context = scope.Resolve<FundraiserContext>();
                var repository = scope.Resolve<IMemberRepository>();
                repository.Add(member);
                await context.SaveChangesAsync();
            }

            return member;
        }

        protected async Task<IReadOnlyCollection<Member>> CreateStudents(int count,
            SchoolId schoolId, GroupId? groupId = null, Gender gender = Gender.Male)
        {
            var students = new List<Member>();
            for (var i = 0; i <= count; i++)
            {
                var student = new Member(MemberId.New(), schoolId, gender, SchoolRole.Student, Email.Create($"test-{Guid.NewGuid()}@o2.pl").Value);
                if (groupId.HasValue)
                    student.EnrollToGroup(groupId.Value);
                students.Add(student);
            }

            await using (var scope = _compositionRoot.BeginLifetimeScope())
            {
                var context = scope.Resolve<FundraiserContext>();
                var repository = scope.Resolve<IMemberRepository>();
                foreach (var student in students)
                    repository.Add(student);

                await context.SaveChangesAsync();
            }

            return students;
        }


        protected async Task<Fundraiser> CreateFundraiser(Member manager,
            SchoolId schoolId,
            GroupId? groupId = null,
            Range range = Range.Intraschool,
            Type type = Type.Normal,
            decimal goal = 10000m,
            bool isGoalShared = false,
            string name = "Fundraiser test title",
            string description = null)
        {
            var fundraiser = Fundraiser.Create(
                Name.Create(name).Value,
                Description.Create(description).Value,
                Goal.Create(goal, isGoalShared).Value,
                schoolId, groupId, manager, range, type).Value;

            await using (var scope = _compositionRoot.BeginLifetimeScope())
            {
                var context = scope.Resolve<FundraiserContext>();
                var fundraiserRepository = scope.Resolve<IFundraiserRepository>();
                fundraiserRepository.Add(fundraiser);
                await context.SaveChangesAsync();
            }

            return fundraiser;
        }

        protected async Task<Maybe<Member>> QueryMember(MemberId memberId, SchoolId schoolId)
        {
            await using (var scope = _compositionRoot.BeginLifetimeScope())
            {
                var memberRepository = scope.Resolve<IMemberRepository>();
                var member = await memberRepository.GetByIdAsync(memberId, schoolId);
                return member;
            }
        }

        protected async Task<Maybe<Fundraiser>> QueryFundraiser(FundraiserId fundraiserId, SchoolId schoolId)
        {
            await using (var scope = _compositionRoot.BeginLifetimeScope())
            {
                var fundraiserRepository = scope.Resolve<IFundraiserRepository>();
                var fundraiser = await fundraiserRepository.GetByIdWithParticipantsAsync(schoolId, fundraiserId);
                return fundraiser;
            }
        }

        protected void ReplaceCurrentUser(
            string userId = "3f0b0a5f-7aa6-46f4-9247-3ef9a9df2407",
            string role = "Administrator",
            string schoolId = null,
            string groupId = null,
            string groupRole = null)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.NotBefore, "1622747447"),
                new Claim(JwtClaimTypes.Expiration, "9999999999"),
                new Claim(JwtClaimTypes.Issuer, "https://localhost:44309"),
                new Claim(JwtClaimTypes.Audience, "fundraiserapi"),
                new Claim(JwtClaimTypes.Audience, "https://localhost:44309/resources"),
                new Claim(JwtClaimTypes.ClientId, "fundraiserapi_swagger"),
                new Claim(JwtClaimTypes.Subject, userId),
                new Claim(JwtClaimTypes.AuthenticationTime, "1622743448"),
                new Claim(JwtClaimTypes.IdentityProvider, "local"),
                new Claim(JwtClaimTypes.Role, role),
                new Claim(JwtClaimTypes.JwtId, "5CB887B330AB296FCCF46CC49AF2649D"),
                new Claim(JwtClaimTypes.SessionId, "7896DCF199D0D75E28E39B9CECCEDF80"),
                new Claim(JwtClaimTypes.IssuedAt, "1622747447"),
                new Claim(JwtClaimTypes.Scope, "fundraiserapi.fullaccess"),
                new Claim(JwtClaimTypes.AuthenticationMethod, "pwd")
            };
            if (!string.IsNullOrWhiteSpace(schoolId))
                claims.Add(new Claim(CustomClaimTypes.SchoolId, schoolId));

            if (!string.IsNullOrWhiteSpace(groupId))
                claims.Add(new Claim(CustomClaimTypes.GroupId, groupId));

            if (!string.IsNullOrWhiteSpace(groupRole))
                claims.Add(new Claim(JwtClaimTypes.Role, groupRole));

            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(claims))
            };

            using (var scope = _compositionRoot.BeginLifetimeScope())
            {
                var stub = scope.Resolve<Mock<IHttpContextAccessor>>();

                stub.SetupGet(x => x.HttpContext).Returns(context);
            }
        }

        protected async Task<TResult> Execute<TResult>(ICommand<TResult> command)
            where TResult : IResult
        {
            using (var scope = _compositionRoot.BeginLifetimeScope())
            {
                var mediator = scope.Resolve<IMediator>();
                return await mediator.Send(command);
            }
        }

        protected async Task<TResult> Execute<TResult>(IQuery<TResult> query)
        {
            using (var scope = _compositionRoot.BeginLifetimeScope())
            {
                var mediator = scope.Resolve<IMediator>();
                return await mediator.Send(query);
            }
        }
    }
}