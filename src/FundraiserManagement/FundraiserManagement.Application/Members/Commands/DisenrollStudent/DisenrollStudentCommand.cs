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

namespace FundraiserManagement.Application.Members.Commands.DisenrollStudent
{
    internal sealed class DisenrollStudentCommand : IInternalCommand
    {
        public MemberId MemberId { get; }

        public DisenrollStudentCommand(MemberId memberId)
        {
            MemberId = memberId;
        }
    }

    internal sealed class DisenrollStudentCommandHandler : IRequestHandler<DisenrollStudentCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;

        public DisenrollStudentCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }


        public async Task<Result> Handle(DisenrollStudentCommand request, CancellationToken token)
        {
            var memberOrNone = await _memberRepository.GetByIdAsync(request.MemberId, token);
            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member (Id:{request.MemberId}) not found!");

            var result = memberOrNone.Value.DisenrollFromGroup();

            return result;
        }
    }
}