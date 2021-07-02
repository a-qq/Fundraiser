using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using SharedKernel.Infrastructure.Abstractions.Common;

namespace FundraiserManagement.Application.Members.Commands.DeleteMember
{
    internal sealed class DeleteMemberCommand : IInternalCommand
    {
        public MemberId MemberId { get; }

        public DeleteMemberCommand(MemberId memberId)
        {
            MemberId = memberId;
        }
    }

    internal sealed class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IDateTime _dateTimeService;


        public DeleteMemberCommandHandler(
            IMemberRepository memberRepository,
            IDateTime dateTimeService)
        {
            _memberRepository = memberRepository;
            _dateTimeService = dateTimeService;
        }


        public async Task<Result> Handle(DeleteMemberCommand request, CancellationToken token)
        {
            var memberOrNone = await _memberRepository.GetByIdAsync(request.MemberId, token);
            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member (Id:{request.MemberId}) not found!");

            var result = memberOrNone.Value.Delete(_dateTimeService.Now);

            return result;
        }
    }
}