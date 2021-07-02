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

namespace FundraiserManagement.Application.Members.Commands.DivestHeadmaster
{
    internal sealed class DivestHeadmasterCommand : IInternalCommand
    {
        public MemberId MemberId { get; }

        public DivestHeadmasterCommand(MemberId memberId)
        {
            MemberId = memberId;
        }
    }

    internal sealed class DivestHeadmasterCommandHandler : IRequestHandler<DivestHeadmasterCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;

        public DivestHeadmasterCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }
        public async Task<Result> Handle(DivestHeadmasterCommand request, CancellationToken token)
        {
            var memberOrNone = await _memberRepository.GetByIdAsync(request.MemberId, token);
            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member (Id:{request.MemberId}) not found!");

            var result = memberOrNone.Value.DivestFromHeadmaster();

            return result;
        }
    }
}