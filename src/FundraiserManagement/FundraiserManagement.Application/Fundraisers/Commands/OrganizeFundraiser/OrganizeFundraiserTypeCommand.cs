using Ardalis.GuardClauses;
using AutoMapper;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Dtos;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Common.Security;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using SharedKernel.Infrastructure.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Auth;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FMD = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using PolicyNames = FundraiserManagement.Application.Common.Security.PolicyNames;

namespace FundraiserManagement.Application.Fundraisers.Commands.OrganizeFundraiser
{
    [Authorize(Policy = PolicyNames.CanCreateFundraiser)]
    public sealed class OrganizeFundraiserTypeCommand : IUserCommand<FundraiserDto>, IFundraiserTypeAuthorizationRequest
    {
        public string Name { get; }
        public string Description { get; }
        public Guid? GroupId { get; }
        public Guid SchoolId { get; }
        public FMD.Range Range { get; }
        public FMD.Type Type { get; }
        public decimal Goal { get; }
        public bool IsShared { get; }
        public Guid ManagerId { get; }

        public OrganizeFundraiserTypeCommand(string name, string description,
            Guid? groupId, Guid schoolId, FMD.Range range, FMD.Type type,
            decimal goal, bool isShared, Guid managerId)
        {
            Name = name;
            Description = description;
            GroupId = groupId;
            SchoolId = schoolId;
            Range = range;
            Type = type;
            Goal = goal;
            IsShared = isShared;
            ManagerId = managerId;
        }
    }

    internal sealed class OrganizeFundraiserCommandHandler : IRequestHandler
        <OrganizeFundraiserTypeCommand, Result<FundraiserDto, RequestError>>
    {
        private readonly IFundraiserRepository _fundraiserRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;

        public OrganizeFundraiserCommandHandler(
            IFundraiserRepository fundraiserRepository,
            IMemberRepository memberRepository,
            IMapper mapper)
        {
            _fundraiserRepository = fundraiserRepository;
            _memberRepository = memberRepository;
            _mapper = mapper;
        }

        public async Task<Result<FundraiserDto, RequestError>> Handle(
            OrganizeFundraiserTypeCommand request, CancellationToken token)
        {
            var name = Name.Create(request.Name).Value;
            var description = Description.Create(request.Description).Value;
            GroupId? groupId = null;
            if (request.GroupId.HasValue)
                groupId = new GroupId(request.GroupId.Value);
            var schoolId = new SchoolId(request.SchoolId);
            var managerId = new MemberId(request.ManagerId);
            Goal goal = Goal.Create(request.Goal, request.IsShared).Value;

            var memberOrNone = await _memberRepository
                .GetByIdAsync(managerId, schoolId, token);

            if (memberOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(managerId, nameof(Member));

            var result = Fundraiser.Create(name, description, goal, schoolId,
                groupId, memberOrNone.Value, request.Range, request.Type);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            _fundraiserRepository.Add(result.Value);

            var fundraiserDto = _mapper.Map<FundraiserDto>(result.Value);

            return fundraiserDto;
        }
    }
}