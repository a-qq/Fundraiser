using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;

namespace FundraiserManagement.Application.Members.Commands.AssignStudent
{
    internal sealed class AssignStudentCommand : IInternalCommand
    {
        public MemberId StudentId { get; }
        public GroupId GroupId { get; }

        public AssignStudentCommand(MemberId studentId, GroupId groupId)
        {
            StudentId = studentId;
            GroupId = groupId;
        }
    }

    internal sealed class AssignStudentCommandHandler : IRequestHandler<AssignStudentCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;

        public AssignStudentCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<Result> Handle(AssignStudentCommand request, CancellationToken token)
        {
            var memberOrNone = await _memberRepository.GetByIdAsync(request.StudentId, token);
            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member (Id:{request.StudentId}) not found!");

            var result = memberOrNone.Value.EnrollToGroup(request.GroupId);

            return result;
        }
    }
}