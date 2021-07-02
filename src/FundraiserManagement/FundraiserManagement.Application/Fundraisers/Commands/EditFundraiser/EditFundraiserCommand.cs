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
using SharedKernel.Domain.Errors;
using SharedKernel.Infrastructure.Errors;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Auth;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FMD = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;

namespace FundraiserManagement.Application.Fundraisers.Commands.EditFundraiser
{
    [Authorize(Policy = PolicyNames.CanModifyFundraiser)]
    public sealed class EditFundraiserCommand : IUserCommand<FundraiserDto>, IFundraiserAuthorizationRequest,
        IFundraiserTypeAuthorizationRequest
    {
        public Guid SchoolId { get; }
        public Guid FundraiserId { get; }
        public string Name { get; }
        public string Description { get; }
        public Guid? GroupId { get; }
        public FMD.Range Range { get; }
        public FMD.Type Type { get; }
        public decimal Goal { get; }
        public bool IsShared { get; }
        public Guid ManagerId { get; }

        public EditFundraiserCommand(Guid schoolId, Guid fundraiserId, string name, string description,
            Guid? groupId, FMD.Range range, FMD.Type type, decimal goal, bool isShared, Guid managerId)
        {
            SchoolId = schoolId;
            FundraiserId = fundraiserId;
            Name = name;
            Description = description;
            GroupId = groupId;
            Range = range;
            Type = type;
            Goal = goal;
            IsShared = isShared;
            ManagerId = managerId;
        }
    }

    internal sealed class
        EditFundraiserCommandHandler : IRequestHandler<EditFundraiserCommand, Result<FundraiserDto, RequestError>>
    {
        private readonly IFundraiserRepository _fundraiserRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;

        public EditFundraiserCommandHandler(IFundraiserRepository fundraiserRepository, IMemberRepository memberRepository, IMapper mapper)
        {
            _fundraiserRepository = Guard.Against.Null(fundraiserRepository, nameof(fundraiserRepository));
            _memberRepository = Guard.Against.Null(memberRepository, nameof(memberRepository));
            _mapper = Guard.Against.Null(mapper, nameof(mapper));
        }


        public async Task<Result<FundraiserDto, RequestError>> Handle(EditFundraiserCommand request, CancellationToken token)
        {
            var fundraiserId = new FundraiserId(request.FundraiserId);
            var managerId = new MemberId(request.ManagerId);

            var name = Name.Create(request.Name).Value;
            var description = Description.Create(request.Description).Value;
            GroupId? groupId = null;
            if (request.GroupId.HasValue)
                groupId = new GroupId(Guard.Against.Default(request.GroupId.Value, nameof(request.GroupId)));
            var schoolId = new SchoolId(request.SchoolId);
            var goal = Goal.Create(request.Goal, request.IsShared).Value;

            if (Fundraiser.Validate(groupId, request.Range, request.Type).IsFailure)
                throw new InvalidOperationException(nameof(EditFundraiserCommandHandler));

            var fundraiserOrNone = await _fundraiserRepository.GetByIdWithManagerAsync(schoolId, fundraiserId, token);
            if (fundraiserOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(fundraiserId, nameof(Fundraiser));

            Result<bool, Error> result;
            if (fundraiserOrNone.Value.Manager.Id != request.ManagerId)
            {
                var prevalidation =
                    fundraiserOrNone.Value.CanBeEdited(name, description, goal, groupId, request.Range, request.Type);

                if (prevalidation.IsFailure)
                    return SharedRequestError.General.BusinessRuleViolation(prevalidation.Error);

                var managerOrNone = await _memberRepository.GetByIdAsync(managerId, schoolId, token);
                if (managerOrNone.HasNoValue)
                    return SharedRequestError.General.NotFound(managerId, nameof(Member));

                result = fundraiserOrNone.Value.Edit(name, description, goal, groupId, request.Range, request.Type,
                    managerOrNone.Value);

            }
            else
                result = fundraiserOrNone.Value.Edit(name, description, goal, groupId, request.Range, request.Type);

            if (result.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(result.Error);

            var fundraiserDto = _mapper.Map<FundraiserDto>(fundraiserOrNone.Value);

            return fundraiserDto;
        }
    }
}